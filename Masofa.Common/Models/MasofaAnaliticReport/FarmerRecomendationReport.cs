using DocumentFormat.OpenXml.Office.CoverPageProps;
using Masofa.Common.Models.CropMonitoring;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Masofa.Common.Models.MasofaAnaliticReport
{
    public class FarmerRecomendationReport : BaseEntity
    {
        public LocalizationFileStorageItem LocalizationFile { get; set; }
        public Guid FileStorageItemId { get; set; }
        public Guid FieldId { get; set; }
        public Guid SeasonId { get; set; }
        public string? QwenJobId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string? SeasonJson
        {
            get
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(Seasons);
            }
            set
            {
                Seasons = Newtonsoft.Json.JsonConvert.DeserializeObject<Season>(value);
            }
        }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string? FieldJson
        {
            get
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(Fields);
            }
            set
            {
                Fields = Newtonsoft.Json.JsonConvert.DeserializeObject<Field>(value);
            }
        }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string? HeaderJson
        {
            get
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(Header);
            }
            set
            {
                Header = Newtonsoft.Json.JsonConvert.DeserializeObject<HeaderVm>(value);
            }
        }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string? SoilJson
        {
            get
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(Soil);
            }
            set
            {
                Soil = Newtonsoft.Json.JsonConvert.DeserializeObject<SoilVm>(value);
            }
        }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string? CalendarJson
        {
            get
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(Calendar);
            }
            set
            {
                Calendar = Newtonsoft.Json.JsonConvert.DeserializeObject<CalendarVm>(value);
            }
        }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string? IrrigationJson
        {
            get
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(Irrigation);
            }
            set
            {
                Irrigation = Newtonsoft.Json.JsonConvert.DeserializeObject<IrrigationVm>(value);
            }
        }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string? WeatherJson
        {
            get
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(Weather);
            }
            set
            {
                Weather = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherBlockVm>(value);
            }
        }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string? MonitoringJson
        {
            get
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(Monitoring);
            }
            set
            {
                Monitoring = Newtonsoft.Json.JsonConvert.DeserializeObject<MonitoringVm>(value);
            }
        }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string? FertilizationJson
        {
            get
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(Fertilization);
            }
            set
            {
                Fertilization = Newtonsoft.Json.JsonConvert.DeserializeObject<FertilizationVm>(value);
            }
        }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string? GrowthStagesJson
        {
            get
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(GrowthStages);
            }
            set
            {
                GrowthStages = Newtonsoft.Json.JsonConvert.DeserializeObject<GrowthStagesVm>(value);
            }
        }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string? IndicesJson
        {
            get
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(Indices);
            }
            set
            {
                Indices = Newtonsoft.Json.JsonConvert.DeserializeObject<IndicesVm>(value);
            }
        }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string? FertPestJson
        {
            get
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(FertPest);
            }
            set
            {
                FertPest = Newtonsoft.Json.JsonConvert.DeserializeObject<FertilizersPesticidesVm>(value);
            }
        }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string? ClimaticSummJson
        {
            get
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(ClimaticSumm);
            }
            set
            {
                ClimaticSumm = Newtonsoft.Json.JsonConvert.DeserializeObject<ClimaticSummVm>(value);
            }
        }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string? BidResultsJson
        {
            get
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(BidResults);
            }
            set
            {
                BidResults = Newtonsoft.Json.JsonConvert.DeserializeObject<BidResultVm>(value);
            }
        }

        /// <summary>
        /// Сырой JSON результата Qwen (колонка в БД)
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string? QwenJobResultJson { get; set; } 

        [NotMapped] private bool _qwenParsed;
        [NotMapped] private QwenJobResultResponse? _qwenObj;

        /// <summary>
        /// Типизированный объект результата Qwen (НЕ хранится в БД)
        /// </summary>
        [NotMapped]
        public QwenJobResultResponse? QwenJobResultResponse
        {
            get
            {
                if (_qwenParsed) return _qwenObj;

                _qwenObj = DeserializeQwen(QwenJobResultJson);
                _qwenParsed = true;
                return _qwenObj;
            }
            private set
            {
                _qwenObj = value;
                _qwenParsed = true;
            }
        }

        /// <summary>
        /// Удобный доступ к items (НЕ хранится в БД)
        /// </summary>
        [NotMapped]
        public List<QwenImageAnalysisResult> QwenItems => QwenJobResultResponse?.Items ?? new();

        /// <summary>
        /// Установить результат Qwen и обновить кеш
        /// </summary>
        public void SetQwenResult(string? json)
        {
            QwenJobResultJson = json;
            QwenJobResultResponse = DeserializeQwen(json);
        }

        private static QwenJobResultResponse? DeserializeQwen(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                return JsonConvert.DeserializeObject<QwenJobResultResponse>(json);
            }
            catch (Newtonsoft.Json.JsonException)
            {
                return null;
            }
            catch (System.Text.Json.JsonException)
            {
                return null;
            }
        }

        [NotMapped]
        public AnomalyPhotoTableVm? AnomalyTable { get; set; }

        [NotMapped]
        public QwenJobResultResponse? QwenJobResultResponses{ get; set; }

        [NotMapped]
        public HeaderVm? Header { get; set; } = new();

        [NotMapped]
        public SoilVm? Soil { get; set; } = new();

        [NotMapped]
        public CalendarVm? Calendar { get; set; } = new();

        [NotMapped]
        public IrrigationVm? Irrigation { get; set; } = new();

        [NotMapped]
        public WeatherBlockVm? Weather { get; set; } = new();

        [NotMapped]
        public MonitoringVm? Monitoring { get; set; } = new();

        [NotMapped]
        public FertilizationVm? Fertilization { get; set; } = new();

        [NotMapped]
        public FertilizersPesticidesVm? FertPest { get; set; } = new();

        [NotMapped]
        public ClimaticSummVm? ClimaticSumm { get; set; } = new();

        [NotMapped]
        public GrowthStagesVm? GrowthStages { get; set; } = new();

        [NotMapped]
        public IndicesVm? Indices { get; set; } = new();

        [NotMapped]
        public Season? Seasons { get; set; } = new();

        [NotMapped]
        public Field? Fields { get; set; } = new();

        [NotMapped]
        public List<AgroOperationRow>? FertilizationOperations { get; set; } = new();

        [NotMapped]
        public CropRotationVm? CropRotation { get; set; } = new();

        [NotMapped]
        public BidResultVm? BidResults { get; set; } = new();

        // SVG
        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string QrSvg { get; set; } = string.Empty;

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string TempChartSvg { get; set; } = string.Empty;

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string RainChartSvg { get; set; } = string.Empty;

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string SolarChartSvg { get; set; } = string.Empty;

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string LogoSvgBase64 { get; set; } = string.Empty;

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string QrSvgBase64 => ConvertToBase64(QrSvg);

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string TempChartSvgBase64 => ConvertToBase64(TempChartSvg);

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string RainChartSvgBase64 => ConvertToBase64(RainChartSvg);

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string SolarChartSvgBase64 => ConvertToBase64(SolarChartSvg);

        [NotMapped]
        public string GrowthStageSvg { get; set; } = string.Empty;

        [NotMapped]
        public string MinistrySvg { get; set; } = string.Empty;

        public static string ConvertToBase64(string svgContent)
        {
            if (string.IsNullOrEmpty(svgContent))
                return string.Empty;
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(svgContent));
        }

        /// <summary>
        /// Статус рекомендации
        /// </summary>
        public FarmerReportStateType ReportState { get; set; } = FarmerReportStateType.New;
    }

    public enum FarmerReportStateType
    {
        New = 0,
        InProgress = 1,
        Finished = 2,
        Failed = 3
    }

    public sealed class QwenJobResultResponse
    {
        [Newtonsoft.Json.JsonProperty("items")]
        public List<QwenImageAnalysisResult>? Items { get; set; }

        [Newtonsoft.Json.JsonProperty("summary")]
        public QwenJobSummary? Summary { get; set; }
    }

    public sealed class QwenJobSummary
    {
        [Newtonsoft.Json.JsonProperty("anomaly_count")]
        public int AnomalyCount { get; set; }

        [Newtonsoft.Json.JsonProperty("total_images")]
        public int TotalImages { get; set; }
    }

    public sealed class AnomalyPhotoTableVm
    {
        public List<AnomalyPhotoTableRow> Rows { get; set; } = new();
    }

    public sealed class AnomalyPhotoTableRow
    {
        public string? PhotoDataUri { get; set; } // data:image/jpeg;base64,...
        public string? Crop { get; set; }         // культура (читаемая)
        public bool AnomalyPresence { get; set; }
        public string? AnomalyDescription { get; set; }
        public string? ProblemType { get; set; }
        public string? Anomaly1 { get; set; }
        public string? Anomaly2 { get; set; }
        public string? ClassificationDescription { get; set; }
        public string? Recommendations { get; set; }
    }

    public sealed class SoilVm
    {
        public string SoilType { get; set; } = string.Empty;
        public List<SoilRow> Rows { get; set; } = new();
        public Dictionary<string, double?> Extras { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }

    public class CropRotationVm
    {
        public List<CropRotationRow> Rows { get; set; } = new();
    }

    public class CropRotationRow
    {
        public string CropName { get; set; } = string.Empty;
        public DateOnly? PlannedSowingDate { get; set; }
        public DateOnly? ActualSowingDate { get; set; }
        public double AreaHa { get; set; }
        public DateOnly? PlannedHarvestDate { get; set; }
        public DateOnly? ActualHarvestDate { get; set; }
        public double YieldPerHectare { get; set; } // урожайность с гектара
        public DateOnly? HarvestDate { get; set; } // дата уборки
    }

    public class AgroOperationRow
    {
        public DateTime Date { get; set; }
        public string OperationName { get; set; } = string.Empty;
        public string FertilizerType { get; set; } = string.Empty;
        public double AmountKgHa { get; set; }
        public string Timing { get; set; } = string.Empty;
    }

    public sealed class SoilRow
    {
        public SoilRow() { }
        public SoilRow(DateOnly analysisDate, double? ph, double? humus, double? n, double? p,
                       double? sand, double? silt, double? clay, double? density,
                       double? cec, double? soc, double? salinity, double? cfvo)
        {
            AnalysisDate = analysisDate; Ph = ph; HumusPercent = humus; N = n; P = p;
            SandPercent = sand; SiltPercent = silt; ClayPercent = clay; Density = density;
            CEC = cec; SOCPercent = soc; Salinity = salinity; CoarseFragmentsVolPercent = cfvo;
        }

        public DateOnly AnalysisDate { get; set; }

        public double? Ph { get; set; }                  // pH(H2O)
        public double? HumusPercent { get; set; }        // %
        public double? N { get; set; }                   // mg/kg
        public double? P { get; set; }                   // mg/kg
        public double? SandPercent { get; set; }         // %
        public double? SiltPercent { get; set; }         // %
        public double? ClayPercent { get; set; }         // %
        public double? Density { get; set; }             // г/см³
        public double? CEC { get; set; }                 // cmol(+)/kg
        public double? SOCPercent { get; set; }          // % (из soc g/kg → %)
        public double? Salinity { get; set; }            // dS/m (как пришло)
        public double? CoarseFragmentsVolPercent { get; set; } // cfvo, %
    }

    public sealed class HeaderVm
    {
        public Guid FieldId { get; set; }
        public string SoilType { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string CropName { get; set; } = string.Empty;
        public string CadastralNumber { get; set; } = string.Empty;
        public string FieldIdText { get; set; } = string.Empty;
        public (double? lng, double? lat) Wgs84 { get; set; }
        public double? AreaHa { get; set; }
        public string ReportNumber { get; set; } = "№000000";
        public DateOnly ReportDate { get; set; }
        public string DeepLink => $"https://app.example/report/{ReportNumber}";
    }

    public sealed class CalendarVm
    {
        public List<CalendarRow> Rows { get; set; } = new();
    }

    public sealed class CalendarRow
    {
        public CalendarRow() { }
        public CalendarRow(string dayOffset, string activity, string? notes, string? rec, string? actualDate)
        {
            DayOffset = dayOffset;
            Activity = activity;
            Notes = notes;
            Recommendation = rec;
            ActualDate = actualDate;
        }
        public string DayOffset { get; set; }
        public string Activity { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? Recommendation { get; set; }
        public string? ActualDate { get; set; }
    }

    public sealed class IrrigationVm
    {
        public string CropName { get; set; } = string.Empty;
        public string SprinklingM3Ha { get; set; } = string.Empty; // дождевание
        public string DripM3Ha { get; set; } = string.Empty;        // капельное
        public string FurrowM3Ha { get; set; } = string.Empty;      // борозды
    }

    public sealed class WeatherBlockVm
    {
        public string Region { get; set; } = string.Empty;
        public double AirTemp { get; set; }
        public double AirHumidity { get; set; }
        public double WindSpeed { get; set; }
        public string WindDir { get; set; } = string.Empty;
        public double PrecipMm { get; set; }
        public double SolarJPerCm2 { get; set; }

        public List<ForecastDay> Next7Days { get; set; } = new();

        public List<(DateTime x, double y)> TempFact { get; set; } = new();
        public List<(DateTime x, double y)> TempNorm { get; set; } = new();

        public List<(DateTime x, double y)> TempSeriesDeviation { get; set; } = new();

        public List<(DateTime x, double y)> RainFact { get; set; } = new();
        public List<(DateTime x, double y)> RainNorm { get; set; } = new();

        public List<(DateTime x, double y)> SolarFact { get; set; } = new();
        public List<(DateTime x, double y)> SolarNorm { get; set; } = new();
    }

    public sealed class ForecastDay
    {
        public DateOnly Date { get; set; }
        public string Icon { get; set; } = "sun";
        public double DayTempC { get; set; }
    }

    public sealed class MonitoringVm
    {
        public string WarningText { get; set; } = string.Empty;
    }

    public sealed class FertilizationVm
    {
        public string Title { get; set; } = string.Empty;
        public FertilizationTable Table { get; set; } = new();
    }

    public sealed class FertilizationTable
    {
        public string Crop { get; set; } = string.Empty; public string Variety { get; set; } = string.Empty;
        public string Manure { get; set; } = string.Empty; public int ManureKgHa { get; set; }
        public int NKgHa { get; set; }
        public int PKgHa { get; set; }
        public int KKgHa { get; set; }
        public string Spring { get; set; } = string.Empty; public string BeforeSowing { get; set; } = string.Empty; public string WithSowing { get; set; } = string.Empty;
        public string Topdress1 { get; set; } = string.Empty; public string Topdress2 { get; set; } = string.Empty; public string Topdress3 { get; set; } = string.Empty;
    }

    public sealed class GrowthStagesVm
    {
        public List<GrowthStageRow> Rows { get; set; } = new();
        public string CurrentPhase { get; set; } = string.Empty;
    }
    public sealed class GrowthStageRow
    {
        public GrowthStageRow() { }
        public GrowthStageRow(string stage, string days, string sum) { Stage = stage; Days = days; ActiveTempSum = sum; }
        public string Stage { get; set; } = string.Empty; public string Days { get; set; } = string.Empty; public string ActiveTempSum { get; set; } = string.Empty;
    }

    public sealed class IndicesVm
    {
        public string Title { get; set; } = string.Empty; public string FieldTitle { get; set; } = string.Empty;
        public string ObservationSummary { get; set; } = string.Empty; public string Recommendations { get; set; } = string.Empty;
        public double NegAreasChangePercent { get; set; }
        public DateTime? ImageDate { get; set; }
        public string Satellite { get; set; } = string.Empty;
        public Guid? ImageFileStorageItemId { get; set; }
        
        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string ImageBase64 { get; set; } = string.Empty;
        
        [NotMapped]
        public List<IndexImageVm> IndexImages { get; set; } = new();
    }

    public sealed class IndexImageVm
    {
        public string IndexType { get; set; } = string.Empty;
        public string IndexName { get; set; } = string.Empty;
        public Guid? ImageFileStorageItemId { get; set; }
        
        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string ImageBase64 { get; set; } = string.Empty;
        
        [NotMapped]
        public List<AnomalyCoordinateVm> AnomalyCoordinates { get; set; } = new();
    }

    public sealed class AnomalyCoordinateVm
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class ClimaticSummVm
    {
        public DateOnly Date { get; set; }
        public double Temp7 { get; set; }
        public double Temp10 { get; set; }
        public double Temp12 { get; set; }
        public double Temp15 { get; set; }
        public double Fallout { get; set; }
        public double Radiation { get; set; }
    }

    public class FertilizersPesticidesVm
    {
        public List<FertilizersPesticidesRows> Rows { get; set; } = new();
    }

    public class FertilizersPesticidesRows
    {
        public DateOnly? Date { get; set; }
        public string? Type { get; set; } = string.Empty;
        public string? Name { get; set; } = string.Empty;
        public decimal? Quantity { get; set; }
        public string? UnitOfMeasurement { get; set; } = string.Empty;
        public string? Crop { get; set; } = string.Empty;
    }

    public class BidResultVm
    {
        public List<BidResultRow> Rows { get; set; } = new();
    }

    public class BidResultRow
    {
        public string WorkPeriod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Crop { get; set; } = string.Empty;
    }

    //[Newtonsoft.Json.JsonConverter(typeof(LocalizationFileConverter))]
    //public struct LocalizationFile
    //{
    //    public LocalizationFile() { }

    //    private static List<string> _langKeys = new List<string>()
    //    {
    //        "ru-RU",
    //        "en-US",
    //        //"ar-LB",
    //        "uz-Latn-UZ"
    //    };

    //    private Dictionary<string, Guid> _values = new Dictionary<string, Guid>();

    //    public static List<string> SupportedLanguages
    //    {
    //        get
    //        {
    //            return _langKeys;
    //        }
    //    }

    //    public Guid this[string key]
    //    {
    //        get
    //        {
    //            if (!_langKeys.Contains(key))
    //            {
    //                throw new ArgumentException($"Language key '{key}' is not supported");
    //            }
    //            return _values.ContainsKey(key) ? _values[key] : Guid.Empty;
    //        }
    //        set
    //        {
    //            if (!_langKeys.Contains(key))
    //            {
    //                throw new ArgumentException($"Language key '{key}' is not supported");
    //            }
    //            if (!_values.ContainsKey(key))
    //            {
    //                _values.Add(key, value);
    //            }
    //            _values[key] = value;
    //        }
    //    }

    //    public string ValuesJson
    //    {
    //        get
    //        {
    //            return Newtonsoft.Json.JsonConvert.SerializeObject(_values);
    //        }
    //        set
    //        {
    //            if (string.IsNullOrWhiteSpace(value))
    //            {
    //                _values = new Dictionary<string, Guid>();
    //                return;
    //            }

    //            var s = value.Trim().TrimStart('\uFEFF');

    //            if (s.StartsWith("{") || s.StartsWith("["))
    //            {
    //                var temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Guid>>(s)
    //                           ?? new Dictionary<string, Guid>();
    //                foreach (var item in temp)
    //                {
    //                    if (_langKeys.Contains(item.Key)) this[item.Key] = item.Value;
    //                }
    //            }
    //            else
    //            {
    //                // строка не JSON — трактуем как значение по умолчанию
    //                // Для Guid по умолчанию используем Guid.Empty, так как нет смысла в "дефолтном" Guid
    //                _values = new Dictionary<string, Guid>();
    //                if (Guid.TryParse(s, out Guid defaultGuid))
    //                {
    //                    _values.Add("ru-RU", defaultGuid);
    //                }
    //                // Если s не является валидным Guid, _values останется пустым
    //            }
    //        }
    //    }

    //    public override string ToString()
    //    {
    //        return ValuesJson;
    //    }

    //    public static implicit operator LocalizationFile(Dictionary<string, Guid> values)
    //    {
    //        var result = new LocalizationFile();
    //        foreach (var item in values)
    //        {
    //            result[item.Key] = item.Value;
    //        }
    //        return result;
    //    }

    //    public static implicit operator string(LocalizationFile localizationFile)
    //    {
    //        return localizationFile.ValuesJson;
    //    }
    //}

    //public class LocalizationFileConverter : Newtonsoft.Json.JsonConverter<LocalizationFile>
    //{
    //    public override LocalizationFile ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, LocalizationFile existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
    //    {
    //        var token = Newtonsoft.Json.Linq.JToken.Load(reader);
    //        var dict = token.ToObject<Dictionary<string, Guid>>() ?? new Dictionary<string, Guid>();
    //        var result = new LocalizationFile();

    //        foreach (var kvp in dict)
    //        {
    //            result[kvp.Key] = kvp.Value;
    //        }

    //        return result;
    //    }

    //    public override void WriteJson(Newtonsoft.Json.JsonWriter writer, LocalizationFile value, Newtonsoft.Json.JsonSerializer serializer)
    //    {
    //        var temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Guid>>(value.ValuesJson);
    //        serializer.Serialize(writer, temp);
    //    }
    //}


    public class FarmerRecomendationReportModelHistory : BaseHistoryEntity<FarmerRecomendationReport> { }
}