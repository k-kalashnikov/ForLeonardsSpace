using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Агротех мероприятия
    /// </summary>
    /// <remarks>
    /// Справочник агротехнических мероприятий
    /// </remarks>
    public class AgrotechnicalMeasure : NamedDictionaryItem
    {
        /// <summary>
        /// Идентификатор региона
        /// </summary>
        public Guid? RegionId { get; set; }

        /// <summary>
        /// Идентификатор культуры
        /// </summary>
        public Guid? CropId { get; set; }

        /// <summary>
        /// Идентификатор сорта
        /// </summary>
        public Guid? VarietyId { get; set; }

        /// <summary>
        /// Культура
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public Crop? Crop { get; set; }

        /// <summary>
        /// Сорт
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public Variety? Variety { get; set; }

        /// <summary>
        /// Начало периода (Дней после посева)
        /// </summary>
        public int DayStart { get; set; }

        /// <summary>
        /// Конец периода (Дней после посева)
        /// </summary>
        public int DayEnd { get; set; }

        /// <summary>
        /// Примечание (локализованное)
        /// </summary>

        public LocalizationString Descriptions { get; set; } = new LocalizationString();

        /// <summary>
        /// Рекомендации по почве (локализованные)
        /// </summary>
        public LocalizationString SoilRecommendations { get; set; } = new LocalizationString();
    }

    public class AgrotechnicalMeasureHistory : BaseHistoryEntity<AgrotechnicalMeasure> { }
}
