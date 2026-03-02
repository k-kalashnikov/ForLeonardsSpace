using System.Text.Json.Serialization;
using Masofa.Common.Models.Satellite.Auxiliary;

namespace Masofa.Common.Models.Satellite.Parse.Sentinel.Quality
{
    /// <summary>
    /// JSON модель для Product Quality Metadata (плоская структура от XmlToJsonConverter)
    /// </summary>
    public class SentinelProductQualityMetadataRaw
    {
        [JsonPropertyName("xmlns")]
        public string? Xmlns { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }

        /// <summary>
        /// Основная обертка Product Quality Metadata
        /// </summary>
        [JsonPropertyName("Earth_Explorer_Header")]
        public EarthExplorerHeader? EarthExplorerHeader { get; set; }

        [JsonPropertyName("Data_Block")]
        public DataBlock? DataBlock { get; set; }

        /// <summary>
        /// Удобный геттер для получения Date
        /// </summary>
        public string Date => DataBlock?.Report?.Date ?? string.Empty;
    }

    /// <summary>
    /// Альтернативная модель для плоской структуры JSON от XmlToJsonConverter
    /// </summary>
    public class SentinelProductQualityMetadataFlat
    {
        [JsonPropertyName("xmlns")]
        public string? Xmlns { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }

        /// <summary>
        /// Извлекает данные из плоской строки value
        /// </summary>
        public ParsedQualityData? ParsedData => QualityDataParser.ParseValueString(Value);
    }

    /// <summary>
    /// Распарсенные данные из строки value
    /// </summary>
    public class ParsedQualityData
    {
        public string? FileName { get; set; }
        public string? FileDescription { get; set; }
        public string? Mission { get; set; }
        public string? FileClass { get; set; }
        public string? FileType { get; set; }
        public string? ValidityStart { get; set; }
        public string? ValidityStop { get; set; }
        public string? FileVersion { get; set; }
        public string? System { get; set; }
        public string? Creator { get; set; }
        public string? CreatorVersion { get; set; }
        public string? CreationDate { get; set; }
        public string? GippVersion { get; set; }
        public string? GlobalStatus { get; set; }
        public string? Date { get; set; }
        public string? ParentId { get; set; }
        public string? Name { get; set; }
        public string? Version { get; set; }
        public List<Check>? Checks { get; set; }
    }

    /// <summary>
    /// Основная структура Earth Explorer Header
    /// </summary>
    public class EarthExplorerHeader
    {
        /// <summary>
        /// Секция Fixed Header
        /// </summary>
        [JsonPropertyName("Fixed_Header")]
        public FixedHeader? FixedHeader { get; set; }
    }

    /// <summary>
    /// Секция Fixed Header
    /// </summary>
    public class FixedHeader
    {
        [JsonPropertyName("File_Name")]
        public string? FileName { get; set; }

        [JsonPropertyName("File_Description")]
        public string? FileDescription { get; set; }

        [JsonPropertyName("Notes")]
        public string? Notes { get; set; }

        [JsonPropertyName("Mission")]
        public string? Mission { get; set; }

        [JsonPropertyName("File_Class")]
        public string? FileClass { get; set; }

        [JsonPropertyName("File_Type")]
        public string? FileType { get; set; }

        [JsonPropertyName("Validity_Period")]
        public ValidityPeriod? ValidityPeriod { get; set; }

        [JsonPropertyName("File_Version")]
        public object? FileVersion { get; set; }

        [JsonPropertyName("Source")]
        public Source? Source { get; set; }
    }

    /// <summary>
    /// Секция Validity Period
    /// </summary>
    public class ValidityPeriod
    {
        [JsonPropertyName("Validity_Start")]
        public string? ValidityStart { get; set; }

        [JsonPropertyName("Validity_Stop")]
        public string? ValidityStop { get; set; }
    }

    /// <summary>
    /// Секция Source
    /// </summary>
    public class Source
    {
        [JsonPropertyName("System")]
        public string? System { get; set; }

        [JsonPropertyName("Creator")]
        public string? Creator { get; set; }

        [JsonPropertyName("Creator_Version")]
        public string? CreatorVersion { get; set; }

        [JsonPropertyName("Creation_Date")]
        public string? CreationDate { get; set; }
    }

    /// <summary>
    /// Секция Data Block
    /// </summary>
    public class DataBlock
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("report")]
        public Report? Report { get; set; }
    }

    /// <summary>
    /// Секция Report
    /// </summary>
    public class Report
    {
        [JsonPropertyName("gippVersion")]
        public string? GippVersion { get; set; }

        [JsonPropertyName("globalStatus")]
        public string? GlobalStatus { get; set; }

        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("checkList")]
        public CheckList? CheckList { get; set; }
    }

    /// <summary>
    /// Секция Check List
    /// </summary>
    public class CheckList
    {
        [JsonPropertyName("parentID")]
        public string? ParentId { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("version")]
        public string? Version { get; set; }

        [JsonPropertyName("item")]
        public string? Item { get; set; }

        [JsonPropertyName("check")]
        public List<Check>? Checks { get; set; }
    }

    /// <summary>
    /// Статический класс для парсинга строки value
    /// </summary>
    public static class QualityDataParser
    {
        /// <summary>
        /// Парсит строку value и извлекает структурированные данные
        /// </summary>
        /// <param name="valueString">Строка value из JSON</param>
        /// <returns>Распарсенные данные или null</returns>
        public static ParsedQualityData? ParseValueString(string? valueString)
        {
            if (string.IsNullOrWhiteSpace(valueString))
                return null;

            var result = new ParsedQualityData();

            try
            {
                // Парсим строку по известным паттернам Sentinel Quality
                // Формат: "filenameReport produced by OLQC-SC...UTC=dateUTC=date..."
                
                // Извлекаем имя файла (до первого "Report")
                var reportIndex = valueString.IndexOf("Report");
                if (reportIndex > 0)
                {
                    result.FileName = valueString.Substring(0, reportIndex).Trim();
                }

                // Извлекаем описание (между "Report produced by" и миссией)
                var reportStart = valueString.IndexOf("Report produced by");
                if (reportStart >= 0)
                {
                    var missionStart = valueString.IndexOf("S2B", reportStart);
                    if (missionStart > reportStart)
                    {
                        result.FileDescription = valueString.Substring(reportStart, missionStart - reportStart).Trim();
                    }
                }

                // Извлекаем миссию (S2B)
                if (valueString.Contains("S2B"))
                {
                    result.Mission = "S2B";
                }

                // Извлекаем класс файла (OPER)
                if (valueString.Contains("OPER"))
                {
                    result.FileClass = "OPER";
                }

                // Извлекаем тип файла (REP_OLQCPA)
                if (valueString.Contains("REP_OLQCPA"))
                {
                    result.FileType = "REP_OLQCPA";
                }

                // Извлекаем даты UTC=
                var utcPatterns = ExtractUtcDates(valueString);
                if (utcPatterns.Count >= 2)
                {
                    result.ValidityStart = utcPatterns[0];
                    result.ValidityStop = utcPatterns[1];
                }
                if (utcPatterns.Count >= 3)
                {
                    result.CreationDate = utcPatterns[2];
                }

                // Извлекаем версию файла (число после дат)
                var versionMatch = System.Text.RegularExpressions.Regex.Match(valueString, @"UTC=\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\d+)");
                if (versionMatch.Success && versionMatch.Groups.Count > 1)
                {
                    result.FileVersion = versionMatch.Groups[1].Value;
                }

                // Извлекаем системную информацию
                if (valueString.Contains("OLQC-SC"))
                {
                    result.System = "OLQC-SC";
                    result.Creator = "OLQC-SC";
                }

                // Извлекаем версию создателя
                var versionMatch2 = System.Text.RegularExpressions.Regex.Match(valueString, @"OLQC-SC(\d+\.\d+\.\d+)");
                if (versionMatch2.Success)
                {
                    result.CreatorVersion = versionMatch2.Groups[1].Value;
                }

                // Извлекаем GIPP версию
                var gippMatch = System.Text.RegularExpressions.Regex.Match(valueString, @"GENERAL_QUALITY(\d+\.\d+)");
                if (gippMatch.Success)
                {
                    result.GippVersion = gippMatch.Groups[1].Value;
                }

                // Извлекаем статус
                if (valueString.Contains("PASSED"))
                {
                    result.GlobalStatus = "PASSED";
                }

                // Извлекаем дату отчета
                var dateMatch = System.Text.RegularExpressions.Regex.Match(valueString, @"(\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}Z)");
                if (dateMatch.Success)
                {
                    result.Date = dateMatch.Groups[1].Value;
                }

                // Извлекаем Parent ID
                var parentMatch = System.Text.RegularExpressions.Regex.Match(valueString, @"S2B_OPER_GIP_OLQCPA_MPC__(\d{8}T\d{6}_V\d{8}T\d{6})");
                if (parentMatch.Success)
                {
                    result.ParentId = "S2B_OPER_GIP_OLQCPA_MPC__" + parentMatch.Groups[1].Value;
                }

                // Извлекаем имя и версию
                if (valueString.Contains("GENERAL_QUALITY"))
                {
                    result.Name = "GENERAL_QUALITY";
                    result.Version = "1.0";
                }

                // Создаем список проверок
                result.Checks = ExtractChecks(valueString);

                return result;
            }
            catch (Exception)
            {
                // В случае ошибки возвращаем null
                return null;
            }
        }

        /// <summary>
        /// Извлекает даты в формате UTC= из строки
        /// </summary>
        private static List<string> ExtractUtcDates(string input)
        {
            var dates = new List<string>();
            var pattern = @"UTC=(\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2})";
            var matches = System.Text.RegularExpressions.Regex.Matches(input, pattern);
            
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    dates.Add(match.Groups[1].Value);
                }
            }
            
            return dates;
        }

        /// <summary>
        /// Извлекает проверки из строки
        /// </summary>
        private static List<Check> ExtractChecks(string input)
        {
            var checks = new List<Check>();
            
            // Паттерны для различных проверок
            var checkPatterns = new[]
            {
                @"NUMBER_OF_TOO_DEGRADED_PACKETS value (\d+) does not exceed the threshold \((\d+)\)",
                @"NUMBER_OF_LOST_PACKETS value (\d+) does not exceed the threshold \((\d+)\)",
                @"Processor version is valid \(([^)]+)\)",
                @"Fake decompressed source frames are valid\.",
                @"Downlink Orbit number \((\d+)\) is in range \[(\d+),(\d+)\]",
                @"Sensing orbit number \((\d+)\) is in range \[(\d+),(\d+)\]",
                @"Metadata file is present\."
            };

            foreach (var pattern in checkPatterns)
            {
                var matches = System.Text.RegularExpressions.Regex.Matches(input, pattern);
                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    var check = new Check
                    {
                        Inspection = "",
                        Message = match.Value,
                        ExtraValues = new ExtraValues()
                    };

                    // Добавляем значения из групп
                    if (match.Groups.Count > 1)
                    {
                        var values = new List<string>();
                        for (int i = 1; i < match.Groups.Count; i++)
                        {
                            values.Add(match.Groups[i].Value);
                        }
                        check.ExtraValues.Value = string.Join(",", values);
                    }

                    checks.Add(check);
                }
            }

            return checks;
        }
    }
} 