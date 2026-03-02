using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Сорта
    /// </summary>
    /// <remarks>
    /// Справочник сортов
    /// </remarks>
    public class Variety : NamedDictionaryItem
    {
        /// <summary>
        /// Идентификатор культуры
        /// </summary>
        public Guid CropId { get; set; }

        /// <summary>
        /// Наименование на латыни
        /// </summary>
        public string? NameLa { get; set; }

        /// <summary>
        /// Срок созревания в днях
        /// </summary>
        public int? RipeningPeriod { get; set; }

        /// <summary>
        /// Средняя урожайность (ц/Га)
        /// </summary>
        public decimal? AverageYield { get; set; }

        /// <summary>
        /// Идентификатор культуры
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public Crop Crop { get; set; }

        /// <summary>
        /// Агротех мероприятия
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<AgrotechnicalMeasure> AgrotechnicalMeasures { get; set; } = [];

        /// <summary>
        /// Периоды развития культур
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<CropPeriod> CropPeriods { get; set; } = [];

        /// <summary>
        /// Болезни растений
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<Disease> Diseases { get; set; } = [];

        /// <summary>
        /// Дополнительные характеристики сорта
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<VarietyFeature> Features { get; set; } = [];

        /// <summary>
        /// Регионы 
        /// </summary>
        /// <remarks>Районирование сорта</remarks>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<Region> Regions { get; set; } = [];

        /// <summary>
        /// Влияние солнечной радиации
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<SolarRadiationInfluence> SolarRadiationInfluences { get; set; } = [];

        /// <summary>
        /// Периоды вегетации
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<VegetationPeriod> VegetationPeriods { get; set; } = [];
    }

    public class VarietyHistory : BaseHistoryEntity<Variety> { }
}
