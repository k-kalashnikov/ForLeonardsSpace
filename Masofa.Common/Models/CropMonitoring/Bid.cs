using Masofa.Common.Models.MasofaAnaliticReport;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Masofa.Common.Models.CropMonitoring
{
    /// <summary>
    /// Заявка
    /// </summary>
    public class Bid : BaseEntity
    {
        /// <summary>
        /// Идентификатор родительской заявки
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Идентификатор заявки - ссылка на <see cref="Common.Models.Dictionaries.BidType">словари</see>
        /// </summary>
        public Guid BidTypeId { get; set; }

        /// <summary>
        /// Идентификатор бригадира - ссылка на <see cref="Common.Models.Identity.User">пользователей</see>
        /// </summary>
        public Guid? ForemanId { get; set; }

        /// <summary>
        /// Идентификатор полевого работника - ссылка на <see cref="Common.Models.Identity.User">пользователей</see>
        /// </summary>
        public Guid? WorkerId { get; set; }

        /// <summary>
        /// Дата начала выполнения заявки
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Срок выполнения
        /// </summary>
        public DateTime DeadlineDate { get; set; }

        /// <summary>
        /// Дата завершения заявки
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Идентификатор поля - ссылка на <see cref="CropMonitoring.Field">поле</see> 
        /// </summary>
        public Guid? FieldId { get; set; }

        /// <summary>
        /// Идентификатор региона - ссылка на <see cref="Common.Models.Dictionaries.Region">словари</see>
        /// </summary>
        public Guid? RegionId { get; set; }

        /// <summary>
        /// Идентификатор культуры - ссылка на <see cref="Common.Models.Dictionaries.Crop">словари</see>
        /// </summary>
        public Guid CropId { get; set; }

        /// <summary>
        /// Идентификатор сорта культуры - ссылка на <see cref="Common.Models.Dictionaries.Variety">словари</see>
        /// </summary>
        public Guid? VarietyId { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Широта
        /// </summary>
        public double Lat { get; set; }

        /// <summary>
        /// Долгота
        /// </summary>
        public double Lng { get; set; }

        /// <summary>
        /// Номер заявки
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Number { get; set; }

        /// <summary>
        /// Дата посева?
        /// </summary>
        public DateTime? FieldPlantingDate { get; set; }

        /// <summary>
        /// Заказчик
        /// </summary>
        public string? Customer { get; set; }

        /// <summary>
        /// Ссылка на <see cref="Common.Models.SystemCrical.FileStorageItem">файл результата</see> 
        /// </summary>
        public Guid? FileResultId { get; set; }

        /// <summary>
        /// Ссылка на <see cref="Common.Models.CropMonitoring.BidTemplate">Шаблон</see>  
        /// </summary>
        public Guid BidTemplateId { get; set; }

        /// <summary>
        /// Валидность заявки
        /// </summary>
        public bool IsUnvalidBid { get; set; }

        /// <summary>
        /// Айди задачи Qwen
        /// </summary>
        public Guid? QwenTaskId { get; set; }
        public Guid? QwenExpressTaskId { get; set; }
        public DateTime? QwenExpressAnalysisStart { get; set; }
        public DateTime? QwenExpressAnalysisEnd { get; set; }

        /// <summary>
        /// Результат Express анализа (сырой JSON целиком: { items, summary, ... }).
        /// Храним как есть, без преобразований.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string? QwenExpressResultJson { get; set; }

        /// <summary>
        /// Результат Qwen сервиса по поиску аномалий
        /// </summary>
        [NotMapped]
        public QwenJobResultResponse? QwenResults { get; set; }

        /// <summary>
        /// Джейсон результата задачи Qwen по поиску аномалий
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string? QwenResultJson
        {
            get
            {
                if (QwenResults?.Items == null || QwenResults.Items.Count == 0)
                    return null;

                return JsonSerializer.Serialize(QwenResults, (JsonSerializerOptions)null);
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    QwenResults = JsonSerializer.Deserialize<QwenJobResultResponse>(value, (JsonSerializerOptions)null);
                }
                else
                {
                    QwenResults = new QwenJobResultResponse
                    {
                        Items = new List<QwenImageAnalysisResult>() // <— обязательно
                    };
                }
            }
        }

        /// <summary>
        /// Время начала анализа 
        /// </summary>
        public DateTime? QwenAnalysisStart { get; set; }

        /// <summary>
        /// Время окончания анализа
        /// </summary>
        public DateTime? QwenAnalysisEnd { get; set; }

        /// <summary>
        /// Статус заявки
        /// </summary>
        public BidStateType BidState { get; set; } = BidStateType.New;

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Polygon? Polygon { get; set; }

        public string? PolygonJson
        {
            get
            {
                var poly = Polygon;
                if (poly == null || poly.IsEmpty)
                    return null;

                return poly.AsText();
            }
        }
    }

    public enum BidStateType
    {
        New = 0,
        Active = 1,
        InProgress = 2,
        Finished = 3,
        Rejected = 4,
        Canceled = 5,
        QwenAnalysisStart = 6,
        QwenAnalysisEnd = 7
    }

    public class BidHistory : BaseHistoryEntity<Bid> { }

    // Для получения результата — мы получаем массив объектов
    public class QwenImageAnalysisResult
    {
        [JsonPropertyName("archive_name")]
        public string ArchiveName { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("plant_type")]
        public string PlantType { get; set; }

        [JsonPropertyName("anomaly_presence")]
        public bool AnomalyPresence { get; set; }

        [JsonPropertyName("anomaly_description")]
        public string? AnomalyDescription { get; set; }

        [JsonPropertyName("problem_type")]
        public string? ProblemType { get; set; }

        [JsonPropertyName("anomaly1")]
        public string? Anomaly1 { get; set; }

        [JsonPropertyName("anomaly2")]
        public string? Anomaly2 { get; set; }

        [JsonPropertyName("classification_description")]
        public string? Classification_description { get; set; }

        [JsonPropertyName("recommendations")]
        public string? Recommendations { get; set; }
    }
}
