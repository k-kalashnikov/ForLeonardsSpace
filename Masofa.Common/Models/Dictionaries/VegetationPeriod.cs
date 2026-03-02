using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Периоды вегетации
    /// </summary>
    /// <remarks>
    /// Справочник периодов вегетации
    /// </remarks>
    public class VegetationPeriod : NamedDictionaryItem
    {
        /// <summary>
        /// Идентификатор региона
        /// </summary>
        public Guid? RegionId { get; set; }

        /// <summary>
        /// Идентификатор сорта
        /// </summary>
        public Guid? VarietyId { get; set; }

        /// <summary>
        /// Идентификатор класса сорта
        /// </summary>
        public Guid? ClassId { get; set; }

        /// <summary>
        /// День начала периода
        /// </summary>
        public int? DayStart { get; set; }

        /// <summary>
        /// День окончания периода
        /// </summary>
        public int? DayEnd { get; set; }

        /// <summary>
        /// Идентификатор региона
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public Region? Region { get; set; }

        /// <summary>
        /// Идентификатор сорта
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public Variety? Variety { get; set; }

        /// <summary>
        /// Влияние солнечной радиации
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<SolarRadiationInfluence> SolarRadiationInfluences { get; set; } = [];
    }

    public class VegetationPeriodHistory : BaseHistoryEntity<VegetationPeriod> { }
}
