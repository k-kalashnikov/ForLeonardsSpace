using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Влияние солнечной радиации
    /// </summary>
    /// <remarks>
    /// Справочник данных по солнечной активности и её влиянию на урожайность культур
    /// </remarks>
    public class SolarRadiationInfluence : BaseDictionaryItem
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
        /// Начало периода вегетации
        /// </summary>
        public int? DayStart { get; set; }

        /// <summary>
        /// Окончание периода вегетации
        /// </summary>
        public int? DayEnd { get; set; }

        /// <summary>
        /// Оптимальное значение накопленной солнечной радиации
        /// </summary>
        public decimal? RadNorm { get; set; }

        /// <summary>
        /// Идентификатор периода вегетации
        /// </summary>
        public Guid? VegetationPeriodId { get; set; }

        /// <summary>
        /// Идентификатор культуры
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public Crop? Crop { get; set; }

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
        /// Идентификатор периода вегетации
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public VegetationPeriod? VegetationPeriod { get; set; }
    }

    public class SolarRadiationInfluenceHistory : BaseHistoryEntity<SolarRadiationInfluence> { }
}
