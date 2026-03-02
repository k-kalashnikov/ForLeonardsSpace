using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Типы погодных станций
    /// </summary>
    /// <remarks>
    /// Справочник типов погодных станций
    /// </remarks>
    public class WeatherStationType : NamedDictionaryItem
    {
        /// <summary>
        /// Погодные условия провайдеров
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<ProviderWeatherCondition> ProviderWeatherConditions { get; set; } = [];

        /// <summary>
        /// Погодные станции
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<WeatherStation> WeatherStations { get; set; } = [];
    }

    public class WeatherStationTypeHistory : BaseHistoryEntity<WeatherStationType> { }
}
