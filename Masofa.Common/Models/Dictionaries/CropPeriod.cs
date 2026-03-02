using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Периоды развития культур
    /// </summary>
    /// <remarks>
    /// Справочник периодов развития культур
    /// </remarks>
    public class CropPeriod : NamedDictionaryItem
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
        /// День начала периода
        /// </summary>
        public int? DayStart { get; set; }

        /// <summary>
        /// День окончания периода
        /// </summary>
        public int? DayEnd { get; set; }

        /// <summary>
        /// Сумма активных температур (начало диапазона)
        /// </summary>
        public int? ActiveTemperatureSumStart { get; set; }

        /// <summary>
        /// Сумма активных температур (конец диапазона)
        /// </summary>
        public int? ActiveTemperatureSumEnd { get; set; }

        /// <summary>
        /// Идентификатор региона
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string? ImageSvg { get; set; }

        /// <summary>
        /// Идентификатор региона
        /// </summary>
        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [SwaggerIgnore]
        public Region? Region { get; set; }

        /// <summary>
        /// Идентификатор культуры
        /// </summary>
        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [SwaggerIgnore]
        public Crop? Crop { get; set; }

        /// <summary>
        /// Идентификатор сорта
        /// </summary>
        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [SwaggerIgnore]
        public Variety? Variety { get; set; }

        /// <summary>
        /// Индексы периодов развития
        /// </summary>
        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [SwaggerIgnore]
        public ICollection<CropPeriodVegetationIndex>? CropPeriodVegetationIndexes { get; set; } = [];
    }
    public class CropPeriodHistory : BaseHistoryEntity<CropPeriod> { }
}