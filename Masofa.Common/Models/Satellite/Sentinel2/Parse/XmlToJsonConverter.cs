using System.Text.Json;
using System.Xml.Linq;

namespace Masofa.Common.Models.Satellite.Parse.Sentinel
{
    /// <summary>
    /// Утилита для конвертации XML в JSON
    /// </summary>
    public static class XmlToJsonConverter
    {
        /// <summary>
        /// Конвертирует XML строку в JSON строку
        /// </summary>
        /// <param name="xmlContent">XML содержимое</param>
        /// <returns>JSON строка</returns>
        public static string ConvertXmlToJson(string xmlContent)
        {
            if (string.IsNullOrWhiteSpace(xmlContent))
                throw new ArgumentException("XML-содержимое не может быть пустым");

            try
            {
                var xDocument = XDocument.Parse(xmlContent);
                var jsonObject = ConvertXElementToJson(xDocument.Root);
                return Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка при конвертации XML в JSON: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Конвертирует XML стрим в JSON строку
        /// </summary>
        /// <param name="xmlStream">XML стрим</param>
        /// <returns>JSON строка</returns>
        public static async Task<string> ConvertXmlStreamToJsonAsync(Stream xmlStream)
        {
            if (xmlStream == null)
                throw new ArgumentNullException(nameof(xmlStream));

            using var reader = new StreamReader(xmlStream);
            var xmlContent = await reader.ReadToEndAsync();
            return ConvertXmlToJson(xmlContent);
        }

        /// <summary>
        /// Рекурсивно конвертирует XElement в JSON объект
        /// </summary>
        /// <param name="element">XElement для конвертации</param>
        /// <returns>JSON объект</returns>
        private static object ConvertXElementToJson(XElement element)
        {
            if (element == null)
                return null;

            // Если элемент имеет только текстовое содержимое без дочерних элементов
            if (!element.HasElements)
            {
                var value = element.Value?.Trim();
                
                // Пытаемся парсить как число
                if (int.TryParse(value, out var intValue))
                    return intValue;
                
                if (double.TryParse(value, out var doubleValue))
                    return doubleValue;
                
                if (bool.TryParse(value, out var boolValue))
                    return boolValue;
                
                return value ?? string.Empty;
            }

            // Если элемент имеет атрибуты, создаем объект с атрибутами
            if (element.HasAttributes)
            {
                var result = new Dictionary<string, object>();
                
                // Добавляем атрибуты
                foreach (var attribute in element.Attributes())
                {
                    result[attribute.Name.LocalName] = attribute.Value;
                }
                
                // Добавляем содержимое элемента
                if (!string.IsNullOrWhiteSpace(element.Value?.Trim()))
                {
                    result["value"] = element.Value.Trim();
                }
                
                // Добавляем дочерние элементы
                var childGroups = element.Elements().GroupBy(e => e.Name.LocalName);
                foreach (var group in childGroups)
                {
                    if (group.Count() == 1)
                    {
                        result[group.Key] = ConvertXElementToJson(group.First());
                    }
                    else
                    {
                        result[group.Key] = group.Select(ConvertXElementToJson).ToList();
                    }
                }
                
                return result;
            }

            // Если элемент имеет только дочерние элементы
            var childGroups2 = element.Elements().GroupBy(e => e.Name.LocalName);
            var result2 = new Dictionary<string, object>();
            
            foreach (var group in childGroups2)
            {
                if (group.Count() == 1)
                {
                    result2[group.Key] = ConvertXElementToJson(group.First());
                }
                else
                {
                    result2[group.Key] = group.Select(ConvertXElementToJson).ToList();
                }
            }
            
            return result2;
        }
    }
} 