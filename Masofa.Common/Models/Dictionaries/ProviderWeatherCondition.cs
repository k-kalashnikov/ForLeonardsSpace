using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Погодных усл. провайдеров Х
    /// </summary>
    /// <remarks>
    /// Справочник погодных условий провайдеров
    /// </remarks>
    public class ProviderWeatherCondition : NamedDictionaryItem
    {
        /// <summary>
        /// Идентификатор региона
        /// </summary>
        public Guid? RegionId { get; set; }

        /// <summary>
        /// Широта
        /// </summary>
        public decimal? Lat { get; set; }

        /// <summary>
        /// Долгота
        /// </summary>
        public decimal? Lng { get; set; }

        /// <summary>
        /// Радиус покрытия
        /// </summary>
        public decimal? Radius { get; set; }

        /// <summary>
        /// Тип погодной станции
        /// </summary>
        public Guid? WeatherStationTypeId { get; set; }

        /// <summary>
        /// Регион
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public Region? Region { get; set; }

        /// <summary>
        /// Тип погодной станции
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public WeatherStationType? WeatherStationType { get; set; }
    }

    public class ProviderWeatherConditionHistory : BaseHistoryEntity<ProviderWeatherCondition> { }
}
