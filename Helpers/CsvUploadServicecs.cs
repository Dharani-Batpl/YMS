using CsvHelper;
using CsvHelper.Configuration;
using FluentValidation;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace YardManagementApplication.Helpers
{
    public class CsvProcessResult<T>
    {
        public List<T> ValidItems { get; } = new();
        public List<InvalidRecord<T>> InvalidItems { get; } = new();

        // NEW: original header names from the uploaded CSV (in file order)
        public List<string> UploadedHeaders { get; internal set; } = new();

        public int ValidCount => ValidItems.Count;
        public int InvalidCount => InvalidItems.Count;
    }

    public class InvalidRecord<T>
    {
        public T? Record { get; set; }
        public List<string> ErrorMessages { get; set; } = new();
    }

    public class CsvUploadService
    {
        /// <summary>
        /// Parses the CSV into T, validates each row (FluentValidation),
        /// and returns ValidItems + InvalidItems. 
        /// </summary>
        public CsvProcessResult<T> ProcessCsvFile<T>(IFormFile file, IValidator<T> validator)
            where T : class, new()
        {
            var result = new CsvProcessResult<T>();

            var cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                BadDataFound = null,
                HeaderValidated = null,
                PrepareHeaderForMatch = args => NormalizeHeader(args.Header),

                // Skip fully blank lines (all columns empty/whitespace)
                ShouldSkipRecord = args =>
                    args.Row?.Parser?.Record != null &&
                    args.Row.Parser.Record.All(s => string.IsNullOrWhiteSpace(s))
            };

            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, cfg);

            // NEW: read and store the original header names as uploaded
            if (csv.Read())
            {
                csv.ReadHeader();
                result.UploadedHeaders = csv.HeaderRecord?.ToList() ?? new List<string>();
            }

            var enumerator = csv.GetRecords<T>().GetEnumerator();
            while (true)
            {
                T record;
                try
                {
                    if (!enumerator.MoveNext()) break;
                    record = enumerator.Current;
                }
                catch (Exception ex) // type conversion / mapping errors
                {
                    result.InvalidItems.Add(new InvalidRecord<T>
                    {
                        Record = default,
                        // No row numbers included in messages
                        ErrorMessages = new List<string> { ex.Message }
                    });
                    continue;
                }

                var validation = validator?.Validate(record);
                if (validation is { IsValid: false })
                {
                    result.InvalidItems.Add(new InvalidRecord<T>
                    {
                        Record = record,
                        ErrorMessages = validation.Errors.Select(e => e.ErrorMessage).ToList()
                    });
                }
                else
                {
                    result.ValidItems.Add(record);
                }
            }

            return result;
        }

        /// <summary>
        /// ONE CSV that contains:
        ///  - validation failures (InvalidRecord&lt;T&gt;),
        ///  - API insert failures (with Error).
        /// Columns: ErrorMessages, then every public readable property of T.
        /// </summary>
        public string CreateUnifiedInvalidCsv<T>(
            IEnumerable<InvalidRecord<T>> validationInvalids,
            IEnumerable<(T Record, string Error)> apiFailures)
        {
            var all = new List<InvalidRecord<T>>();

            // Combine validation and API failures
            if (validationInvalids != null) all.AddRange(validationInvalids);
            if (apiFailures != null)
            {
                foreach (var (rec, err) in apiFailures)
                {
                    all.Add(new InvalidRecord<T>
                    {
                        Record = rec,
                        ErrorMessages = new List<string> { err }
                    });
                }
            }

            if (all.Count == 0) return string.Empty;

            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, new UTF8Encoding(false));
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            // Header 
            csv.WriteField("ErrorMessages");
            foreach (var prop in GetReadableProps(typeof(T)))
                csv.WriteField(prop.Name);
            csv.NextRecord();

            // Rows
            foreach (var inv in all)
            {
                csv.WriteField(string.Join(" | ", inv.ErrorMessages ?? new List<string>()));
                foreach (var prop in GetReadableProps(typeof(T)))
                {
                    var value = inv.Record == null ? null : prop.GetValue(inv.Record);
                    csv.WriteField(value?.ToString());
                }
                csv.NextRecord();
            }

            writer.Flush();
            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }

        /// <summary>
        /// OVERLOAD: same as CreateUnifiedInvalidCsv, but if you pass uploadedHeaders,
        /// the output CSV will include ONLY those columns (in the original order),
        /// plus ErrorMessages.
        /// If uploadedHeaders is null/empty, it falls back to all public properties (original behavior).
        /// </summary>
        public string CreateUnifiedInvalidCsv<T>(
            IEnumerable<InvalidRecord<T>> validationInvalids,
            IEnumerable<(T Record, string Error)> apiFailures,
            IEnumerable<string>? uploadedHeaders)
        {
            var all = new List<InvalidRecord<T>>();
            if (validationInvalids != null) all.AddRange(validationInvalids);
            if (apiFailures != null)
            {
                foreach (var (rec, err) in apiFailures)
                {
                    all.Add(new InvalidRecord<T>
                    {
                        Record = rec,
                        ErrorMessages = new List<string> { err }
                    });
                }
            }

            if (all.Count == 0) return string.Empty;

            // Normalize a string the same way PrepareHeaderForMatch does
            static string Norm(string? s) =>
                (s ?? string.Empty).Trim().Replace("-", "_").Replace(" ", "_").ToLowerInvariant();

            // Map normalized property name -> PropertyInfo
            var props = GetReadableProps(typeof(T));
            var propByNorm = props.ToDictionary(p => Norm(p.Name), p => p);

            // Decide column set
            var headerList = (uploadedHeaders ?? Array.Empty<string>()).ToList();
            var useUploaded = headerList.Count > 0;

            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, new UTF8Encoding(false));
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            // Header
            csv.WriteField("ErrorMessages");
            if (useUploaded)
            {
                foreach (var h in headerList)
                    csv.WriteField(h);
            }
            else
            {
                foreach (var p in props)
                    csv.WriteField(p.Name);
            }
            csv.NextRecord();

            // Rows
            foreach (var inv in all)
            {
                csv.WriteField(string.Join(" | ", inv.ErrorMessages ?? new List<string>()));

                if (useUploaded)
                {
                    foreach (var h in headerList)
                    {
                        var key = Norm(h);
                        object? value = null;
                        if (inv.Record != null && propByNorm.TryGetValue(key, out var pi))
                            value = pi.GetValue(inv.Record);

                        csv.WriteField(value?.ToString());
                    }
                }
                else
                {
                    foreach (var p in props)
                    {
                        var value = inv.Record == null ? null : p.GetValue(inv.Record);
                        csv.WriteField(value?.ToString());
                    }
                }

                csv.NextRecord();
            }

            writer.Flush();
            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }

        /// <summary>Back-compat: legacy method that only exported validation failures.</summary>
        public string CreateInvalidCsvWithErrors<T>(List<InvalidRecord<T>> invalidItems)
        {
            return CreateUnifiedInvalidCsv(
                validationInvalids: invalidItems ?? new List<InvalidRecord<T>>(),
                apiFailures: Array.Empty<(T Record, string Error)>()
            );
        }

        /* -------------------- internals -------------------- */

        private static string NormalizeHeader(string? header) =>
            (header ?? string.Empty).Trim().Replace("-", "_").Replace(" ", "_").ToLowerInvariant();

        // Cache readable props to avoid reflection on every row
        private static readonly Dictionary<Type, PropertyInfo[]> _propsCache = new();

        private static PropertyInfo[] GetReadableProps(Type t)
        {
            if (_propsCache.TryGetValue(t, out var cached)) return cached;
            var props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                         .Where(p => p.CanRead).ToArray();
            _propsCache[t] = props;
            return props;
        }
    }
}
