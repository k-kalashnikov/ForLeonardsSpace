using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Регионы
    /// </summary>
    /// <remarks>
    /// Справочник регионов
    /// </remarks>
    public class Region : NamedDictionaryItem
    {
        /// <summary>
        /// Идентификатор региона
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Уровень административной единицы
        /// </summary>
        public int? Level { get; set; }

        /// <summary>
        /// Условное обозначение
        /// </summary>
        public string? NameMhobt { get; set; }

        /// <summary>
        /// Условное обозначение аббревиатура
        /// </summary>
        public LocalizationString ShortNames { get; set; } = new LocalizationString();

        /// <summary>
        /// Начало периода актуальности
        /// </summary>
        public DateTime? ActiveFrom { get; set; }

        /// <summary>
        /// Окончание периода актуальности
        /// </summary>
        public DateTime? ActiveTo { get; set; }

        /// <summary>
        /// Площадь региона
        /// </summary>
        public decimal? RegionSquare { get; set; }

        /// <summary>
        /// Наименование админ ед
        /// </summary>
        public LocalizationString AdminNames { get; set; } = new LocalizationString();

        /// <summary>
        /// Население
        /// </summary>
        public decimal? Population { get; set; }

        /// <summary>
        /// Идентификатор агроклиматической зоны
        /// </summary>
        public Guid? AgroclimaticZoneId { get; set; }

        /// <summary>
        /// Идентификатор полигона
        /// </summary>
        public Guid? RegionMapId { get; set; }

        /// <summary>
        /// Идентификатор типа региона
        /// </summary>
        public Guid? RegionTypeId { get; set; }

        /// <summary>
        /// Идентификатор агроклиматической зоны
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public AgroclimaticZone? AgroclimaticZone { get; set; }

        /// <summary>
        /// Идентификатор полигона
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public RegionMap? RegionMap { get; set; }

        /// <summary>
        /// Идентификатор типа региона
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public RegionType? RegionType { get; set; }

        /// <summary>
        /// Юридические лица, для которых этот регион является основным
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<Firm> Firms { get; set; } = [];

        /// <summary>
        /// Климатические нормы данного региона
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<ClimaticStandard> ClimaticStandards { get; set; } = [];

        /// <summary>
        /// Периоды развития культур в данном регионе
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<CropPeriod> CropPeriods { get; set; } = [];

        /// <summary>
        /// Физические лица, для которых этот регион является основным регионом ведения деятельности
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<Person> Persons { get; set; } = [];

        /// <summary>
        /// Погодные условия провайдеров для данного региона
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<ProviderWeatherCondition> ProviderWeatherConditions { get; set; } = [];

        /// <summary>
        /// Влияние солнечной радиации в данном регионе
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<SolarRadiationInfluence> SolarRadiationInfluences { get; set; } = [];

        /// <summary>
        /// Сорта
        /// </summary>
        /// <remarks>Районирование сорта</remarks>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<Variety> Varieties { get; set; } = [];

        /// <summary>
        /// Периоды вегетации в данном регионе
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<VegetationPeriod> VegetationPeriods { get; set; } = [];

        /// <summary>
        /// Погодные станции в данном регионе
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<WeatherStation> WeatherStations { get; set; } = [];
    }

    public class RegionHistory : BaseHistoryEntity<Region> { }
}
