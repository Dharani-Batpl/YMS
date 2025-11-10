using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace YardManagementApplication.Utils
{
    // ============================================================
    // Common JSON Date Converters
    // ============================================================

    // ----- DateTime (MM/dd/yyyy) -----
    public sealed class JsonDateTimeConverter : JsonConverter<DateTime>
    {
        private readonly string _format;

        public JsonDateTimeConverter(string format = "MM/dd/yyyy")
        {
            _format = format;
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();
            if (string.IsNullOrWhiteSpace(s)) return default;
            return DateTime.Parse(s, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(_format, CultureInfo.InvariantCulture));
    }

    public sealed class JsonNullableDateTimeConverter : JsonConverter<DateTime?>
    {
        private readonly string _format;

        public JsonNullableDateTimeConverter(string format = "MM/dd/yyyy")
        {
            _format = format;
        }

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();
            if (string.IsNullOrWhiteSpace(s)) return null;
            return DateTime.Parse(s, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteStringValue(value.Value.ToString(_format, CultureInfo.InvariantCulture));
            else
                writer.WriteNullValue();
        }
    }

    // ----- DateTimeOffset (MM/dd/yyyy HH:mm:ss - 24 hrs) -----
    public sealed class JsonDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        private readonly string _format;

        public JsonDateTimeOffsetConverter(string format = "MM/dd/yyyy HH:mm:ss")
        {
            _format = format;
        }

        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();
            if (string.IsNullOrWhiteSpace(s)) return default;
            return DateTimeOffset.Parse(s, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(_format, CultureInfo.InvariantCulture));
    }

    public sealed class JsonNullableDateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
    {
        private readonly string _format;

        public JsonNullableDateTimeOffsetConverter(string format = "MM/dd/yyyy HH:mm:ss")
        {
            _format = format;
        }

        public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();
            if (string.IsNullOrWhiteSpace(s)) return null;
            return DateTimeOffset.Parse(s, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteStringValue(value.Value.ToString(_format, CultureInfo.InvariantCulture));
            else
                writer.WriteNullValue();
        }
    }

    public static class AppJson
    {
        /// <summary>
        /// Common default: DateTime = MM/dd/yyyy, DateTimeOffset = MM/dd/yyyy HH:mm:ss (24h)
        /// </summary>
        public static JsonSerializerOptions CreateUiOptions()
        {
            var opts = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = null
            };

            opts.Converters.Add(new JsonDateTimeConverter("MM/dd/yyyy"));
            opts.Converters.Add(new JsonNullableDateTimeConverter("MM/dd/yyyy"));
            opts.Converters.Add(new JsonDateTimeOffsetConverter("MM/dd/yyyy HH:mm:ss"));
            opts.Converters.Add(new JsonNullableDateTimeOffsetConverter("MM/dd/yyyy HH:mm:ss"));

            return opts;
        }

        /// <summary>
        /// Special version: both DateTime and DateTimeOffset as MM/dd/yyyy (no time)
        /// </summary>
        public static JsonSerializerOptions CreateDateOnlyOptions()
        {
            var opts = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = null
            };

            opts.Converters.Add(new JsonDateTimeConverter("MM/dd/yyyy"));
            opts.Converters.Add(new JsonNullableDateTimeConverter("MM/dd/yyyy"));
            opts.Converters.Add(new JsonDateTimeOffsetConverter("MM/dd/yyyy"));
            opts.Converters.Add(new JsonNullableDateTimeOffsetConverter("MM/dd/yyyy"));

            return opts;
        }
    }

    // ============================================================
    // Existing Utility Functions
    // ============================================================
    public static class Utility
    {
        public static List<SelectListItem> PrepareSelectList(IEnumerable<DropdownModel> source)
        {
            return (source ?? Enumerable.Empty<DropdownModel>())
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToList();
        }

        public static List<SelectListItem> GetModelYearList()
        {
            int currentYear = DateTime.Now.Year;
            int totalLetters = 26;
            int endYear = currentYear + 1;
            int startYear = endYear - (totalLetters - 1);

            var modelYearList = new List<SelectListItem>();

            for (int i = 0; i < totalLetters; i++)
            {
                int year = startYear + i;
                char prefix = (char)('A' + i);

                modelYearList.Add(new SelectListItem
                {
                    Value = year.ToString(),
                    Text = $"{prefix}-{year}"
                });
            }

            return modelYearList;
        }
    }
}
