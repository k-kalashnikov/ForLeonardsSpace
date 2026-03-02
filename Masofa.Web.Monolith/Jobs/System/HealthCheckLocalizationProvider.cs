using Masofa.Common.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Masofa.Web.Monolith.Jobs.System
{
    public interface IHealthCheckLocalizationProvider
    {
        LocalizationString GetModuleNames(Type? dbContextType);
        LocalizationString GetModelNames(Type? modelType);
        string ResolveLanguageKey(string? language);
        string GetLocalizedValue(LocalizationString names, string languageKey, string fallback);
        LocalizationString GetMissingTableError(string tableName);
        LocalizationString GetDatabaseMissingError(string dbContextName);
        LocalizationString GetRequestNotRegisteredError(string requestKey, string modelName);
        EmailLocalizationTexts GetEmailTexts(string languageKey);
        string GetEmailSubject(int errorCount, string languageKey);
        string GetErrorsTitle(int errorCount, string languageKey);
    }

    public class HealthCheckLocalizationProvider : IHealthCheckLocalizationProvider
    {
        private static readonly Regex PascalCaseRegex = new Regex("([a-z0-9])([A-Z])", RegexOptions.Compiled);

        private readonly Dictionary<string, LocalizationString> _moduleCache = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, LocalizationString> _modelCache = new(StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<string, LocalizationString> ModuleOverrides = new(StringComparer.OrdinalIgnoreCase)
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
        };

        private static readonly Dictionary<string, LocalizationString> ModelOverrides = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Masofa.Common.Models.Identity.User"] = CreateLocalization("Пользователь", "User", "Foydalanuvchi"),
            ["Masofa.Common.Models.Identity.Role"] = CreateLocalization("Роль", "Role", "Rola"),
            ["Masofa.Common.Models.SystemCrical.SystemBackgroundTask"] = CreateLocalization("Фоновая задача", "Background Task"),
            ["Masofa.Common.Models.SystemCrical.HealthCheckResult"] = CreateLocalization("Результат проверки", "Health Check Result"),
            ["Masofa.Common.Models.SystemCrical.LogMessage"] = CreateLocalization("Системное сообщение", "Log Message"),
        };

        private static readonly Dictionary<string, string> MissingTableTemplates = new()
        {
            ["ru-RU"] = "Таблица {0} отсутствует",
            ["en-US"] = "Table {0} does not exist",
            ["uz-Latn-UZ"] = "Jadval {0} mavjud emas",
            ["uz-Cyrl-UZ"] = "Жадвал {0} мавжуд эмас",
        };

        private static readonly Dictionary<string, string> DatabaseMissingTemplates = new()
        {
            ["ru-RU"] = "База данных для {0} отсутствует",
            ["en-US"] = "Database for {0} does not exist",
            ["uz-Latn-UZ"] = "{0} bazasi mavjud emas",
            ["uz-Cyrl-UZ"] = "{0} базаси мавжуд эмас",
        };

        private static readonly Dictionary<string, Dictionary<string, string>> RequestNotRegisteredTemplates = new(StringComparer.OrdinalIgnoreCase)
        {
            ["get"] = new Dictionary<string, string>
            {
                ["ru-RU"] = "Запрос получения (GET) не зарегистрирован для модели {0}",
                ["en-US"] = "GET request is not registered for model {0}",
                ["uz-Latn-UZ"] = "GET so'rovi {0} modeli uchun ro'yxatdan o'tkazilmagan",
                ["uz-Cyrl-UZ"] = "GET сўрови {0} модели учун рўйхатдан ўтказилмаган",
            },
            ["getbyid"] = new Dictionary<string, string>
            {
                ["ru-RU"] = "Запрос получения по идентификатору не зарегистрирован для модели {0}",
                ["en-US"] = "Get-by-id request is not registered for model {0}",
                ["uz-Latn-UZ"] = "ID bo'yicha so'rov {0} modeli uchun ro'yxatdan o'tkazilmagan",
                ["uz-Cyrl-UZ"] = "ID бўйича сўров {0} модели учун рўйхатдан ўтказилмаган",
            },
            ["update"] = new Dictionary<string, string>
            {
                ["ru-RU"] = "Команда обновления не зарегистрирована для модели {0}",
                ["en-US"] = "Update command is not registered for model {0}",
                ["uz-Latn-UZ"] = "Yangilash buyrug'i {0} modeli uchun ro'yxatdan o'tkazilmagan",
                ["uz-Cyrl-UZ"] = "Янгилаш буйруғи {0} модели учун рўйхатдан ўтказилмаган",
            },
            ["create"] = new Dictionary<string, string>
            {
                ["ru-RU"] = "Команда создания не зарегистрирована для модели {0}",
                ["en-US"] = "Create command is not registered for model {0}",
                ["uz-Latn-UZ"] = "Yaratish buyrug'i {0} modeli uchun ro'yxatdan o'tkazilmagan",
                ["uz-Cyrl-UZ"] = "Яратиш буйруғи {0} модели учун рўйхатдан ўтказилмаган",
            },
            ["delete"] = new Dictionary<string, string>
            {
                ["ru-RU"] = "Команда удаления не зарегистрирована для модели {0}",
                ["en-US"] = "Delete command is not registered for model {0}",
                ["uz-Latn-UZ"] = "O'chirish buyrug'i {0} modeli uchun ro'yxatdan o'tkazilmagan",
                ["uz-Cyrl-UZ"] = "Ўчириш буйруғи {0} модели учун рўйхатдан ўтказилмаган",
            },
        };

        private static readonly Dictionary<string, EmailLocalizationTexts> EmailTexts = new(StringComparer.OrdinalIgnoreCase)
        {
            ["ru-RU"] = new EmailLocalizationTexts("ОК", "Ошибка", "Работает", "Проблемы"),
            ["en-US"] = new EmailLocalizationTexts("OK", "Error", "Healthy", "Issues"),
            ["uz-Latn-UZ"] = new EmailLocalizationTexts("OK", "Xato", "Sog‘lom", "Muammolar"),
            ["uz-Cyrl-UZ"] = new EmailLocalizationTexts("ОК", "Хато", "Соғлом", "Муаммолар"),
        };

        private static readonly Dictionary<string, (string ErrorTemplate, string Success)> EmailSubjects = new(StringComparer.OrdinalIgnoreCase)
        {
            ["ru-RU"] = ("⚠️ Отчет о проверке Masofa — найдено {0} ошибок", "✅ Отчет о проверке Masofa — система работает нормально"),
            ["en-US"] = ("⚠️ Masofa Health Check Report - {0} errors found", "✅ Masofa Health Check Report - All systems operational"),
            ["uz-Latn-UZ"] = ("⚠️ Masofa sog‘liqni tekshirish hisobotida {0} ta xato topildi", "✅ Masofa sog‘liqni tekshirish hisobotida xatolar topilmadi"),
            ["uz-Cyrl-UZ"] = ("⚠️ Masofa саломатлик текшируви ҳисоботида {0} та хато топилди", "✅ Masofa саломатлик текшируви ҳисоботида хатолар топилмади"),
        };

        private static readonly Dictionary<string, string> ErrorsTitleTemplates = new(StringComparer.OrdinalIgnoreCase)
        {
            ["ru-RU"] = "Найденные ошибки ({0})",
            ["en-US"] = "Errors Found ({0})",
            ["uz-Latn-UZ"] = "Topilgan xatolar ({0})",
            ["uz-Cyrl-UZ"] = "Топилган хатолар ({0})",
        };

        public LocalizationString GetModuleNames(Type? dbContextType)
        {
            var key = NormalizeModuleKey(dbContextType);

            if (ModuleOverrides.TryGetValue(key, out var predefined))
            {
                return predefined;
            }

            if (_moduleCache.TryGetValue(key, out var cached))
            {
                return cached;
            }

            var generated = CreateLocalization(CreateDefaultRuName(key), CreateDefaultEnName(key));
            _moduleCache[key] = generated;
            return generated;
        }

        public LocalizationString GetModelNames(Type? modelType)
        {
            var key = modelType?.FullName ?? modelType?.Name ?? "Unknown";

            if (ModelOverrides.TryGetValue(key, out var predefined))
            {
                return predefined;
            }

            if (_modelCache.TryGetValue(key, out var cached))
            {
                return cached;
            }

            var readableName = CreateDefaultEnName(modelType?.Name ?? key);
            var generated = CreateLocalization(readableName, readableName);
            _modelCache[key] = generated;
            return generated;
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

        public string GetLocalizedValue(LocalizationString names, string languageKey, string fallback)
        {
            var values = ExtractValues(names);

            if (values.TryGetValue(languageKey, out var value) && !string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            foreach (var key in new[] { "ru-RU", "en-US", "uz-Latn-UZ", "uz-Cyrl-UZ", "ar-LB" })
            {
                if (values.TryGetValue(key, out var altValue) && !string.IsNullOrWhiteSpace(altValue))
                {
                    return altValue;
                }
            }

            return string.IsNullOrWhiteSpace(fallback) ? "Unknown" : fallback;
        }

        public LocalizationString GetMissingTableError(string tableName)
        {
            return FormatLocalizedMessage(MissingTableTemplates, tableName);
        }

        public LocalizationString GetDatabaseMissingError(string dbContextName)
        {
            return FormatLocalizedMessage(DatabaseMissingTemplates, dbContextName);
        }

        public LocalizationString GetRequestNotRegisteredError(string requestKey, string modelName)
        {
            if (!RequestNotRegisteredTemplates.TryGetValue(requestKey, out var template))
            {
                template = RequestNotRegisteredTemplates["get"];
            }

            return FormatLocalizedMessage(template, modelName);
        }

        public EmailLocalizationTexts GetEmailTexts(string languageKey)
        {
            if (EmailTexts.TryGetValue(languageKey, out var texts))
            {
                return texts;
            }

            return EmailTexts["en-US"];
        }

        public string GetEmailSubject(int errorCount, string languageKey)
        {
            if (!EmailSubjects.TryGetValue(languageKey, out var templates))
            {
                templates = EmailSubjects["en-US"];
            }

            if (errorCount > 0)
            {
                return string.Format(templates.ErrorTemplate, errorCount);
            }

            return templates.Success;
        }

        public string GetErrorsTitle(int errorCount, string languageKey)
        {
            if (!ErrorsTitleTemplates.TryGetValue(languageKey, out var template))
            {
                template = ErrorsTitleTemplates["en-US"];
            }

            return string.Format(template, errorCount);
        }

        private static string NormalizeModuleKey(Type? dbContextType)
        {
            var name = dbContextType?.Name ?? "Unknown";

            if (name.StartsWith("Masofa"))
            {
                name = name.Substring("Masofa".Length);
            }

            if (name.EndsWith("DbContext"))
            {
                name = name.Substring(0, name.Length - "DbContext".Length);
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return "Unknown";
            }

            return name;
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

        private static Dictionary<string, string> ExtractValues(LocalizationString names)
        {
            try
            {
                var json = names.ValuesJson;
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new Dictionary<string, string>();
                }

                return JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ??
                       new Dictionary<string, string>();
            }
            catch
            {
                return new Dictionary<string, string>();
            }
        }

        private static LocalizationString FormatLocalizedMessage(Dictionary<string, string> templates, params object[] args)
        {
            var localization = new LocalizationString();

            foreach (var language in LocalizationString.SupportedLanguages)
            {
                string text;
                if (templates.TryGetValue(language, out var template))
                {
                    text = string.Format(template, args);
                }
                else if (templates.TryGetValue("en-US", out var enTemplate))
                {
                    text = string.Format(enTemplate, args);
                }
                else
                {
                    text = string.Format("{0}", args);
                }

                localization[language] = text;
            }

            return localization;
        }
    }

    public record EmailLocalizationTexts(string OkText, string ErrorText, string HealthyStatusText, string IssuesStatusText);
}

