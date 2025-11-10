using Microsoft.AspNetCore.Http;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace YardManagementApplication.Models
{
    public static class ExcelHelper
    {
        public static async Task<(List<T> ValidRows, List<string> ErrorRows)> ReadExcelToListAsync<T>(object fileInput, string columnSortOrder) where T : new()
        {
            if (fileInput == null) throw new ArgumentNullException(nameof(fileInput));

            IWorkbook workbook;
            Stream stream = null;
            if (fileInput is IFormFile formFile)
            {
                stream = formFile.OpenReadStream();
            }
            else if (fileInput is string path)
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            }
            else if (fileInput is Stream s)
            {
                stream = s;
            }
            else
            {
                throw new ArgumentException("Unsupported file input type.");
            }

            using (stream)
            {
                // Determine file format from filename if available; default to XLSX
                bool isXlsx = false;
                string fileName = fileInput switch
                {
                    IFormFile f => f.FileName,
                    string p => p,
                    _ => null
                };
                if (!string.IsNullOrEmpty(fileName))
                {
                    isXlsx = fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    isXlsx = true;
                }

                workbook = isXlsx ? new XSSFWorkbook(stream) as IWorkbook : new HSSFWorkbook(stream);

                ISheet sheet = workbook.GetSheetAt(0);
                if (sheet == null || sheet.PhysicalNumberOfRows < 1)
                    return (new List<T>(), new List<string> { "Excel sheet is empty or missing." });

                // Get writable public instance properties of T
                var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                     .Where(p => p.CanWrite)
                                     .ToList();

                // Sort properties alphabetically if columnSortOrder is specified
                if (!string.IsNullOrEmpty(columnSortOrder))
                {
                    if (columnSortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase))
                        props = props.OrderBy(p => p.Name).ToList();
                    else if (columnSortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
                        props = props.OrderByDescending(p => p.Name).ToList();
                }

                // Read header row and map columns to properties
                IRow headerRow = sheet.GetRow(sheet.FirstRowNum);
                int cellCount = headerRow.LastCellNum;

                var columnPropertyMap = new Dictionary<int, PropertyInfo>();

                if (string.IsNullOrEmpty(columnSortOrder))
                {
                    // Match columns to properties by index in natural order
                    int mapCount = Math.Min(cellCount, props.Count);
                    for (int i = 0; i < mapCount; i++)
                        columnPropertyMap[i] = props[i];
                }
                else
                {
                    // Match columns header text to properties by name (case-insensitive)
                    for (int i = 0; i < cellCount; i++)
                    {
                        ICell cell = headerRow.GetCell(i);
                        if (cell == null) continue;
                        string colName = cell.StringCellValue?.Trim();
                        if (string.IsNullOrEmpty(colName)) continue;
                        var matchedProp = props.FirstOrDefault(p => string.Equals(p.Name, colName, StringComparison.OrdinalIgnoreCase));
                        if (matchedProp != null)
                            columnPropertyMap[i] = matchedProp;
                    }
                }

                var validList = new List<T>();
                var errorRows = new List<string>();

                // Iterate data rows
                for (int r = sheet.FirstRowNum + 1; r <= sheet.LastRowNum; r++)
                {
                    IRow row = sheet.GetRow(r);
                    if (row == null) continue;

                    T obj = new T();
                    bool rowValid = true;
                    var rowValues = new List<string>();

                    foreach (var kvp in columnPropertyMap)
                    {
                        int colIndex = kvp.Key;
                        var prop = kvp.Value;
                        ICell cell = row.GetCell(colIndex);
                        object cellVal = null;

                        if (cell != null)
                        {
                            try
                            {
                                cellVal = GetCellValue(cell);
                                if (cellVal != null)
                                {
                                    var converted = ConvertToType(cellVal, prop.PropertyType);
                                    prop.SetValue(obj, converted);
                                }
                                else
                                {
                                    if (!CanAcceptNull(prop.PropertyType))
                                    {
                                        rowValid = false;
                                    }
                                }
                            }
                            catch
                            {
                                rowValid = false;
                            }
                        }
                        else
                        {
                            if (!CanAcceptNull(prop.PropertyType))
                            {
                                rowValid = false;
                            }
                        }
                        rowValues.Add(cell?.ToString() ?? string.Empty);
                    }

                    if (rowValid)
                        validList.Add(obj);
                    else
                        errorRows.Add(string.Join(",", rowValues));
                }

                return (validList, errorRows);
            }
        }

        private static object GetCellValue(ICell cell)
        {
            switch (cell.CellType)
            {
                case CellType.String:
                    return cell.StringCellValue.Trim();
                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                        return cell.DateCellValue;
                    return cell.NumericCellValue;
                case CellType.Boolean:
                    return cell.BooleanCellValue;
                case CellType.Formula:
                    // Evaluate formula string result
                    return cell.StringCellValue;
                case CellType.Blank:
                case CellType.Error:
                default:
                    return null;
            }
        }

        private static object ConvertToType(object value, Type targetType)
        {
            if (value == null) return null;

            Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            try
            {
                if (underlyingType.IsEnum)
                {
                    if (value is string s)
                        return Enum.Parse(underlyingType, s, true);
                    else
                        return Enum.ToObject(underlyingType, value);
                }

                if (underlyingType == typeof(Guid))
                {
                    if (value is string s)
                        return Guid.Parse(s);
                    return value;
                }

                if (value is string strVal && underlyingType != typeof(string))
                {
                    if (string.IsNullOrWhiteSpace(strVal)) return null;
                    if (underlyingType == typeof(int) && int.TryParse(strVal, out var iv)) return iv;
                    if (underlyingType == typeof(double) && double.TryParse(strVal, out var dv)) return dv;
                    if (underlyingType == typeof(decimal) && decimal.TryParse(strVal, out var decv)) return decv;
                    if (underlyingType == typeof(bool) && bool.TryParse(strVal, out var bv)) return bv;
                    if (underlyingType == typeof(DateTime) && DateTime.TryParse(strVal, out var dtv)) return dtv;
                }

                return Convert.ChangeType(value, underlyingType);
            }
            catch
            {
                throw;
            }
        }

        private static bool CanAcceptNull(Type type)
        {
            if (!type.IsValueType) return true;
            if (Nullable.GetUnderlyingType(type) != null) return true;
            return false;
        }
    }

}
