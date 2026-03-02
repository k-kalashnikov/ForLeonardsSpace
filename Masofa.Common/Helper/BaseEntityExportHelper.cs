using ClosedXML.Excel;
using Masofa.Common.Models;
using System.Reflection;
using System.Text;

namespace Masofa.Common.Helper
{
    /// <summary>
    /// Provides utility methods for converting a collection of entities of type <typeparamref name="TModel"/>  to a CSV
    /// (Comma-Separated Values) format.
    /// </summary>
    /// <remarks>This class is designed to generate a CSV representation of a list of entities, including
    /// their public  properties. Properties marked with <see cref="Newtonsoft.Json.JsonIgnoreAttribute"/> or  <see
    /// cref="System.Text.Json.Serialization.JsonIgnoreAttribute"/> are excluded from the output.  For properties of
    /// type <c>LocalizationString</c>, the CSV will include separate columns for each  supported language.</remarks>
    /// <typeparam name="TModel">The type of the entity, which must inherit from <see cref="BaseEntity"/>.</typeparam>
    public static class BaseEntityExportHelper<TModel>
        where TModel : BaseEntity
    {
        public static byte[] ToCSV(List<TModel> models)
        {
            if (models == null || models.Count == 0)
            {
                return [];
            }

            var type = typeof(TModel);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(p => !p.GetCustomAttributes<Newtonsoft.Json.JsonIgnoreAttribute>().Any() &&
                                        !p.GetCustomAttributes<System.Text.Json.Serialization.JsonIgnoreAttribute>().Any())
                            .ToList();
            if (props == null)
            {
                return [];
            }

            var headers = GetHeaders(props);

            var lines = new List<string>
            {
                string.Join(";", headers.Select(EscapeCsv))
            };

            foreach (var model in models)
            {
                var values = new List<string>();
                foreach (var prop in props)
                {
                    if (prop.PropertyType == typeof(LocalizationString))
                    {
                        var locStr = (LocalizationString)prop.GetValue(model);
                        foreach (var lang in LocalizationString.SupportedLanguages)
                        {
                            values.Add(EscapeCsv(locStr[lang]));
                        }
                    }
                    else
                    {
                        values.Add(EscapeCsv(prop.GetValue(model)?.ToString() ?? ""));
                    }
                }
                lines.Add(string.Join(";", values));
            }

            return Encoding.UTF8.GetBytes(string.Join("\n", lines));
        }

        public static byte[] ToExcel(List<TModel> models)
        {
            if (models == null || models.Count == 0)
            {
                return [];
            }

            var type = typeof(TModel);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(p => !p.GetCustomAttributes<Newtonsoft.Json.JsonIgnoreAttribute>().Any() &&
                                        !p.GetCustomAttributes<System.Text.Json.Serialization.JsonIgnoreAttribute>().Any())
                            .ToList();
            if (props == null)
            {
                return [];
            }

            using var workbook = new XLWorkbook();
            var ws = workbook.AddWorksheet(type.Name);

            var headers = GetHeaders(props);

            var row = 1;
            for (var i = 0; i < headers.Count; i++)
            {
                ws.Cell(row, i + 1).Value = headers[i];
            }

            var lastRow = 0;
            var lastColumn = 0;

            row = 2;
            foreach (var model in models)
            {
                var cellIdx = 1;
                for (var i = 0; i < props.Count; i++)
                {
                    var prop = props[i];
                    if (prop.PropertyType == typeof(LocalizationString))
                    {
                        var locStr = (LocalizationString)prop.GetValue(model);
                        foreach (var lang in LocalizationString.SupportedLanguages)
                        {
                            ws.Cell(row, cellIdx++).Value = EscapeCsv(locStr[lang]);
                        }
                    }
                    else
                    {
                        ws.Cell(row, cellIdx++).Value = EscapeCsv(prop.GetValue(model)?.ToString() ?? "");
                    }
                }
                lastRow = Math.Max(lastRow, row);
                lastColumn = Math.Max(lastColumn, cellIdx - 1);
                row++;
            }

            var lastCell = ws.Cell(lastRow, lastColumn);
            var range = ws.Range(ws.Cell(1, 1), lastCell);

            var table = range.CreateTable();
            table.ShowAutoFilter = true;
            table.Theme = XLTableTheme.TableStyleMedium4;

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private static string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
        }

        private static List<string> GetHeaders(List<PropertyInfo> props)
        {
            var headers = new List<string>();
            foreach (var prop in props)
            {
                if (prop.PropertyType == typeof(LocalizationString))
                {
                    foreach (var lang in LocalizationString.SupportedLanguages)
                    {
                        headers.Add($"{prop.Name}_{lang.Replace('-', '_')}");
                    }
                }
                else
                {
                    headers.Add(prop.Name);
                }
            }

            return headers;
        }
    }

    //public class CsvMapWithJsonIgnore<TModel> : ClassMap<TModel> where TModel : class
    //{
    //    public CsvMapWithJsonIgnore()
    //    {
    //        var type = typeof(TModel);
    //        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

    //        foreach (var prop in properties)
    //        {
    //            // Проверяем наличие [JsonIgnore] из System.Text.Json
    //            var hasSystemTextJsonIgnore = prop.GetCustomAttribute<System.Text.Json.Serialization.JsonIgnoreAttribute>() != null;

    //            // Проверяем наличие [JsonIgnore] из Newtonsoft.Json
    //            var hasNewtonsoftJsonIgnore = prop.GetCustomAttribute<Newtonsoft.Json.JsonIgnoreAttribute>() != null;

    //            if (hasSystemTextJsonIgnore || hasNewtonsoftJsonIgnore)
    //            {
    //                // Игнорируем свойство в CSV
    //                Map(type, prop).Ignore();
    //                continue;
    //            }

    //            if (prop.PropertyType == typeof(LocalizationString))
    //            {
    //                var propName = prop.Name;
    //                foreach (var lang in LocalizationString.SupportedLanguages)
    //                {
    //                    var csvHeaderName = $"{propName}_{lang}";
    //                    Map()
    //                        .Name(csvHeaderName)
    //                        .Convert(args =>
    //                        {
    //                            return (LocalizationString)prop.GetValue()
    //                        });
    //                }

    //                continue;
    //            }
    //            Map(type, prop);
    //        }
    //    }
    //}
}
