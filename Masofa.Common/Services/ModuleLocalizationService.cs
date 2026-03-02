using Masofa.Common.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

namespace Masofa.Common.Services
{
    /// <summary>
    /// Сервис для локализации названий модулей системы
    /// </summary>
    public interface IModuleLocalizationService
    {
        /// <summary>
        /// Получает локализованное название модуля
        /// </summary>
        /// <param name="moduleName">Название модуля</param>
        /// <returns>Локализованная строка с переводами для всех поддерживаемых языков</returns>
        LocalizationString GetModuleLocalization(string moduleName);

        /// <summary>
        /// Получает локализованное значение для конкретного языка
        /// </summary>
        /// <param name="moduleName">Название модуля</param>
        /// <param name="languageKey">Ключ языка (ru-RU, en-US, и т.д.)</param>
        /// <param name="fallback">Значение по умолчанию, если перевод не найден</param>
        /// <returns>Локализованное значение</returns>
        string GetLocalizedValue(string moduleName, string languageKey, string? fallback = null);

        /// <summary>
        /// Разрешает ключ языка из различных форматов
        /// </summary>
        /// <param name="language">Язык в любом формате (ru, ru-RU, en, en-US, и т.д.)</param>
        /// <returns>Стандартизированный ключ языка</returns>
        string ResolveLanguageKey(string? language);
    }

    /// <summary>
    /// Реализация сервиса локализации модулей
    /// </summary>
    public class ModuleLocalizationService : IModuleLocalizationService
    {
        private static readonly Regex PascalCaseRegex = new Regex("([a-z0-9])([A-Z])", RegexOptions.Compiled);

        private static readonly Dictionary<string, LocalizationString> ModuleOverrides = new Dictionary<string, LocalizationString>(StringComparer.OrdinalIgnoreCase)
        {
            ["Common"] = CreateLocalization("Общие сервисы", "Common Services", "Umumiy xizmatlar", "Умумий хизматлар", "الخدمات العامة"),
            ["CommonHistory"] = CreateLocalization("История общих сервисов", "Common Services History", "Umumiy xizmatlar tarixi"),
            ["Identity"] = CreateLocalization("Идентификация и пользователи", "Identity & Users", "Identifikatsiya va foydalanuvchilar"),
            ["IdentityHistory"] = CreateLocalization("История идентификации", "Identity History", "Identifikatsiya tarixi"),
            ["Dictionaries"] = CreateLocalization("Справочники", "Dictionaries", "Lug'atlar"),
            ["DictionariesHistory"] = CreateLocalization("История справочников", "Dictionaries History", "Lug'atlar tarixi"),
            ["CropMonitoring"] = CreateLocalization("Мониторинг полей", "Crop Monitoring", "Dalalarni monitoring qilish"),
            ["CropMonitoringHistory"] = CreateLocalization("История мониторинга полей", "Crop Monitoring History", "Dalalar monitoringi tarixi"),
            ["Weather"] = CreateLocalization("Погода", "Weather", "Ob-havo"),
            ["WeatherHistory"] = CreateLocalization("История погоды", "Weather History", "Ob-havo tarixi"),
            ["WeatherReport"] = CreateLocalization("Отчёты о погоде", "Weather Reports", "Ob-havo hisobotlari"),
            ["WeatherReportHistory"] = CreateLocalization("История погодных отчётов", "Weather Reports History", "Ob-havo hisobotlari tarixi"),
            ["WeatherReportsHistory"] = CreateLocalization("История погодных отчётов", "Weather Reports History", "Ob-havo hisobotlari tarixi"),
            ["Era"] = CreateLocalization("ERA5 Погода", "ERA5 Weather", "ERA5 ob-havosi"),
            ["IBMWeather"] = CreateLocalization("IBM Погода", "IBM Weather", "IBM ob-havosi"),
            ["Ugm"] = CreateLocalization("Узгидромет", "UGM Weather", "O'zgidromet"),
            ["Indices"] = CreateLocalization("Вегетационные индексы", "Vegetation Indices", "Vegetatsiya indekslari"),
            ["Landsat"] = CreateLocalization("Спутник Landsat", "Landsat Satellite", "Landsat sun'iy yo'ldoshi"),
            ["Sentinel"] = CreateLocalization("Спутник Sentinel", "Sentinel Satellite", "Sentinel sun'iy yo'ldoshi"),
            ["Tile"] = CreateLocalization("Тайлы карт", "Map Tiles", "Xarita tayllari"),
            ["TileHistory"] = CreateLocalization("История тайлов", "Tile History", "Tayllar tarixi"),
            ["UAV"] = CreateLocalization("БПЛА", "UAV", "Uchuvchisiz apparatlar"),
            ["AnaliticReport"] = CreateLocalization("Аналитические отчёты", "Analytic Reports", "Analitik hisobotlar"),
            ["AnaliticReportHistory"] = CreateLocalization("История аналитических отчётов", "Analytic Reports History", "Analitik hisobotlar tarixi"),
            ["LandsatHistory"] = CreateLocalization("История Landsat", "Landsat History", "Landsat tarixi"),
            ["SentinelHistory"] = CreateLocalization("История Sentinel", "Sentinel History", "Sentinel tarixi"),
            ["EraHistory"] = CreateLocalization("История ERA5", "ERA5 History", "ERA5 tarixi"),
            ["IBMWeatherHistory"] = CreateLocalization("История IBM погоды", "IBM Weather History", "IBM ob-havo tarixi"),
            ["UgmHistory"] = CreateLocalization("История Узгидромет", "UGM History", "O'zgidromet tarixi"),
            ["IndicesHistory"] = CreateLocalization("История индексов", "Indices History", "Indekslar tarixi"),
            // Дополнительные модули для поддержки тикетов
            ["Authentication"] = CreateLocalization("Аутентификация", "Authentication", "Autentifikatsiya"),
            ["Dashboard"] = CreateLocalization("Панель управления", "Dashboard", "Boshqaruv paneli"),
            ["Reports"] = CreateLocalization("Отчёты", "Reports", "Hisobotlar"),
            ["User Management"] = CreateLocalization("Управление пользователями", "User Management", "Foydalanuvchilarni boshqarish"),
            ["Settings"] = CreateLocalization("Настройки", "Settings", "Sozlamalar"),
            ["Other"] = CreateLocalization("Другое", "Other", "Boshqa"),
        };

        private readonly Dictionary<string, LocalizationString> _moduleCache = new Dictionary<string, LocalizationString>(StringComparer.OrdinalIgnoreCase);

        public LocalizationString GetModuleLocalization(string moduleName)
        {
            if (string.IsNullOrWhiteSpace(moduleName))
            {
                return CreateLocalization("Неизвестный модуль", "Unknown Module", "Noma'lum modul");
            }

            var normalizedKey = NormalizeModuleKey(moduleName);

            // Проверяем предопределенные переводы
            if (ModuleOverrides.TryGetValue(normalizedKey, out var predefined))
            {
                return predefined;
            }

            // Проверяем кэш
            if (_moduleCache.TryGetValue(normalizedKey, out var cached))
            {
                return cached;
            }

            // Генерируем переводы на основе названия
            var generated = CreateLocalization(
                CreateDefaultRuName(normalizedKey),
                CreateDefaultEnName(normalizedKey)
            );
            _moduleCache[normalizedKey] = generated;
            return generated;
        }

        public string GetLocalizedValue(string moduleName, string languageKey, string? fallback = null)
        {
            var localization = GetModuleLocalization(moduleName);
            var resolvedKey = ResolveLanguageKey(languageKey);

            // Пытаемся получить значение для запрошенного языка
            var values = ExtractValues(localization);
            if (values.TryGetValue(resolvedKey, out var value) && !string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            // Пытаемся найти значение на любом доступном языке
            foreach (var key in new[] { "ru-RU", "en-US", "uz-Latn-UZ", "uz-Cyrl-UZ", "ar-LB" })
            {
                if (values.TryGetValue(key, out var altValue) && !string.IsNullOrWhiteSpace(altValue))
                {
                    return altValue;
                }
            }

            return string.IsNullOrWhiteSpace(fallback) ? moduleName : fallback;
        }

        public string ResolveLanguageKey(string? language)
        {
            if (string.IsNullOrWhiteSpace(language))
            {
                return "en-US";
            }

            return language.ToLowerInvariant() switch
            {
                "ru" or "ru-ru" => "ru-RU",
                "en" or "en-us" => "en-US",
                "uz" or "uz-latn" or "uz-latn-uz" => "uz-Latn-UZ",
                "uz-cyrl" or "uz-cyrl-uz" => "uz-Cyrl-UZ",
                "ar" or "ar-lb" => "ar-LB",
                _ => "en-US"
            };
        }

        private static string NormalizeModuleKey(string moduleName)
        {
            if (string.IsNullOrWhiteSpace(moduleName))
            {
                return "Unknown";
            }

            var name = moduleName.Trim();

            // Убираем префикс Masofa если есть
            if (name.StartsWith("Masofa", StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring("Masofa".Length);
            }

            // Убираем суффикс DbContext если есть
            if (name.EndsWith("DbContext", StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(0, name.Length - "DbContext".Length);
            }

            return string.IsNullOrWhiteSpace(name) ? "Unknown" : name;
        }

        private static LocalizationString CreateLocalization(string ru, string en, string? uzLatn = null, string? uzCyrl = null, string? ar = null)
        {
            var localization = new LocalizationString();
            localization["ru-RU"] = ru;
            localization["en-US"] = en;
            localization["uz-Latn-UZ"] = uzLatn ?? en;
            localization["uz-Cyrl-UZ"] = uzCyrl ?? (uzLatn ?? en);
            localization["ar-LB"] = ar ?? en;
            return localization;
        }

        private static string CreateDefaultEnName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "Unknown";
            }

            var cleaned = PascalCaseRegex.Replace(name, "$1 $2");
            return cleaned.Replace("_", " ").Trim();
        }

        private static string CreateDefaultRuName(string name)
        {
            // Пока нет отдельных переводов — используем те же читаемые названия
            return CreateDefaultEnName(name);
        }

        private static Dictionary<string, string> ExtractValues(LocalizationString localization)
        {
            try
            {
                var json = localization.ValuesJson;
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new Dictionary<string, string>();
                }

                return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ??
                       new Dictionary<string, string>();
            }
            catch
            {
                return new Dictionary<string, string>();
            }
        }
    }
}

