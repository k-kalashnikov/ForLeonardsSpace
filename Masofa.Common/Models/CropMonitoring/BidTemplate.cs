using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.CropMonitoring
{
    /// <summary>
    /// Сущность шаблона заявки, используемая в базе данных UMAPI
    /// </summary>
    public class BidTemplate : BaseEntity
    {
        /// <summary>
        /// Идентификатор культуры - ссылка на <see cref="Common.Models.Dictionaries.Crop">словари</see>
        /// </summary>
        public Guid CropId { get; set; }

        /// <summary>
        /// Версия схемы шаблона
        /// </summary>
        public int SchemaVersion { get; set; }

        /// <summary>
        /// Версия содержимого шаблона
        /// </summary>
        public int ContentVersion { get; set; }

        /// <summary>
        /// Данные шаблона в формате JSON
        /// </summary>
        public string DataJson { get; set; } = null!;

        /// <summary>
        /// Комментарий к шаблону
        /// </summary>
        public string? Comment { get; set; }

        [NotMapped]
        public Masofa.Common.Models.CropMonitoring.BidTemplateSchemaVersion3.BidTemplateSchemaVersion3 Data
        {
            get
            {
                return JsonConvert.DeserializeObject<Masofa.Common.Models.CropMonitoring.BidTemplateSchemaVersion3.BidTemplateSchemaVersion3>(DataJson);
            }

            set
            {
                DataJson = JsonConvert.SerializeObject(value);
            }
        }
    }

    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum BlockType
    {
        [Display(Name = "Steps")]
        Steps,

        [Display(Name = "MeteoData")]
        MeteoData,

        [Display(Name = "GeneralData")]
        GeneralData,

        [Display(Name = "FieldData")]
        FieldData,

        [Display(Name = "FertilizersPesticidesInput")]
        FertilizersPesticidesInput
    }

    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum ControlType
    {
        [Display(Name = "checklist")] Checklist,
        [Display(Name = "dropdown")] Dropdown,
        [Display(Name = "checkbox")] Checkbox,
        [Display(Name = "string")] String,
        [Display(Name = "int")] Int,
        [Display(Name = "decimal")] Decimal,
        [Display(Name = "date")] Date,
        [Display(Name = "datetime")] DateTime,
        [Display(Name = "uuid")] Uuid,
        [Display(Name = "photo")] Photo,
        [Display(Name = "float")] Float
    }

    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum ControlSource
    {
        [Display(Name = "user")] User,
        [Display(Name = "api")] Api,
        [Display(Name = "app")] App
    }

    public class BidTemplateHistory : BaseHistoryEntity<BidTemplate> { }
}
