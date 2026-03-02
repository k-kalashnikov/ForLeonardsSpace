using System.Globalization;

namespace Masofa.Common.Helper
{
    /// <summary>
    /// Утилитный класс для безопасной работы с датами в контексте PostgreSQL
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Форматы дат, используемые в Sentinel XML файлах
        /// </summary>
        private static readonly string[] SentinelDateTimeFormats = new[]
        {
            "yyyy-MM-ddTHH:mm:ss.fffZ",           // ISO 8601 с миллисекундами и UTC
            "yyyy-MM-ddTHH:mm:ssZ",                // ISO 8601 без миллисекунд и UTC
            "yyyy-MM-ddTHH:mm:ss.fff",            // ISO 8601 с миллисекундами без UTC
            "yyyy-MM-ddTHH:mm:ss",                // ISO 8601 без UTC
            "yyyy-MM-dd HH:mm:ss.fff",            // Стандартный формат с миллисекундами
            "yyyy-MM-dd HH:mm:ss",                // Стандартный формат
            "yyyy-MM-dd",                         // Только дата
            "yyyy/MM/dd HH:mm:ss",                // Альтернативный формат
            "MM/dd/yyyy HH:mm:ss",                // Американский формат
            "dd.MM.yyyy HH:mm:ss"                 // Европейский формат
        };

        /// <summary>
        /// Префиксы, которые нужно удалить из дат Sentinel
        /// </summary>
        private static readonly string[] SentinelDateTimePrefixes = new[]
        {
            "UTC=",
            "UTC:",
            "UTC ",
            "GMT=",
            "GMT:",
            "GMT "
        };

        /// <summary>
        /// Безопасно парсит строку в DateTime с обработкой ошибок
        /// </summary>
        /// <param name="dateTimeString">Строка с датой</param>
        /// <param name="defaultValue">Значение по умолчанию при ошибке парсинга</param>
        /// <returns>DateTime в UTC или значение по умолчанию</returns>
        public static DateTime SafeParseDateTime(string? dateTimeString, DateTime defaultValue = default)
        {
            if (string.IsNullOrWhiteSpace(dateTimeString))
                return defaultValue == default ? DateTime.UtcNow : defaultValue;

            // Очищаем строку от префиксов Sentinel
            var cleanedString = CleanSentinelDateTimeString(dateTimeString);

            // Пробуем стандартные форматы Sentinel
            foreach (var format in SentinelDateTimeFormats)
            {
                if (DateTime.TryParseExact(cleanedString, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var result))
                {
                    return EnsureUtcDateTime(result);
                }
            }

            // Пробуем стандартный парсинг
            if (DateTime.TryParse(cleanedString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var standardResult))
            {
                return EnsureUtcDateTime(standardResult);
            }

            // Если ничего не сработало, возвращаем значение по умолчанию
            return defaultValue == default ? DateTime.UtcNow : defaultValue;
        }

        /// <summary>
        /// Безопасно парсит DateTime? с обработкой null значений
        /// </summary>
        /// <param name="dateTime">Nullable DateTime</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>DateTime в UTC или значение по умолчанию</returns>
        public static DateTime SafeParseDateTime(DateTime? dateTime, DateTime defaultValue = default)
        {
            if (dateTime == null || dateTime == default)
                return defaultValue == default ? DateTime.UtcNow : defaultValue;

            return EnsureUtcDateTime(dateTime.Value);
        }

        /// <summary>
        /// Обеспечивает, что DateTime находится в UTC формате
        /// </summary>
        /// <param name="dateTime">Исходная дата</param>
        /// <returns>DateTime в UTC</returns>
        public static DateTime EnsureUtcDateTime(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime;

            if (dateTime.Kind == DateTimeKind.Local)
                return dateTime.ToUniversalTime();

            // Если Kind == Unspecified, считаем что это UTC
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        /// <summary>
        /// Безопасно парсит XML DateTime строку
        /// </summary>
        /// <param name="xmlDateTime">XML DateTime строка</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>DateTime в UTC</returns>
        public static DateTime ParseXmlDateTime(string? xmlDateTime, DateTime defaultValue = default)
        {
            if (string.IsNullOrWhiteSpace(xmlDateTime))
                return defaultValue == default ? DateTime.UtcNow : defaultValue;

            // Очищаем строку от префиксов Sentinel
            var cleanedString = CleanSentinelDateTimeString(xmlDateTime);

            // XML DateTime обычно в формате ISO 8601
            if (DateTime.TryParse(cleanedString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var result))
            {
                return EnsureUtcDateTime(result);
            }

            return defaultValue == default ? DateTime.UtcNow : defaultValue;
        }

        /// <summary>
        /// Проверяет, является ли дата валидной для PostgreSQL
        /// </summary>
        /// <param name="dateTime">Проверяемая дата</param>
        /// <returns>true если дата валидна</returns>
        public static bool IsValidForPostgres(DateTime dateTime)
        {
            // PostgreSQL поддерживает даты от 4713 BC до 294276 AD
            // Но для практических целей ограничимся разумными пределами
            var minDate = new DateTime(1900, 1, 1);
            var maxDate = new DateTime(2100, 12, 31);
            
            return dateTime >= minDate && dateTime <= maxDate;
        }

        /// <summary>
        /// Безопасно извлекает дату из JSON метаданных с fallback вариантами
        /// </summary>
        /// <param name="primaryDate">Основная дата</param>
        /// <param name="fallbackDate">Резервная дата</param>
        /// <param name="defaultDate">Дата по умолчанию</param>
        /// <returns>DateTime в UTC</returns>
        public static DateTime ExtractDateWithFallback(DateTime? primaryDate, DateTime? fallbackDate = null, DateTime? defaultDate = null)
        {
            // Пробуем основную дату
            if (primaryDate.HasValue && primaryDate.Value != default && IsValidForPostgres(primaryDate.Value))
            {
                return EnsureUtcDateTime(primaryDate.Value);
            }

            // Пробуем резервную дату
            if (fallbackDate.HasValue && fallbackDate.Value != default && IsValidForPostgres(fallbackDate.Value))
            {
                return EnsureUtcDateTime(fallbackDate.Value);
            }

            // Возвращаем дату по умолчанию
            var result = defaultDate ?? DateTime.UtcNow;
            return EnsureUtcDateTime(result);
        }

        /// <summary>
        /// Очищает строку даты от префиксов Sentinel (UTC=, GMT= и т.д.)
        /// </summary>
        /// <param name="dateTimeString">Исходная строка</param>
        /// <returns>Очищенная строка</returns>
        private static string CleanSentinelDateTimeString(string dateTimeString)
        {
            if (string.IsNullOrWhiteSpace(dateTimeString))
                return dateTimeString;

            var cleaned = dateTimeString.Trim();

            // Удаляем префиксы Sentinel
            foreach (var prefix in SentinelDateTimePrefixes)
            {
                if (cleaned.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    cleaned = cleaned.Substring(prefix.Length).Trim();
                    break;
                }
            }

            return cleaned;
        }
    }
}
