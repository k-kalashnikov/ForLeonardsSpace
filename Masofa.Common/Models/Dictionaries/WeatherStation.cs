using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Погодные станции
    /// </summary>
    /// <remarks>
    /// Справочник погодных станций
    /// </remarks>
    public class WeatherStation : NamedDictionaryItem
    {
        /// <summary>
        /// Идентификатор компании-владельца станции
        /// </summary>
        public Guid? FirmId { get; set; }

        /// <summary>
        /// Идентификатор региона
        /// </summary>
        public Guid? RegionId { get; set; }

        /// <summary>
        /// Идентификатор класса
        /// </summary>
        public Guid? ClassId { get; set; }

        /// <summary>
        /// Признак автоматизированной передачи данных
        /// </summary>
        public bool? IsAuto { get; set; }

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
        /// Тип станции (станция/пост)
        /// </summary>
        public Guid? WeatherStationTypeId { get; set; }

        /// <summary>
        /// Идентификатор компании-владельца станции
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public Firm? Firm { get; set; }

        /// <summary>
        /// Идентификатор региона
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public Region? Region { get; set; }

        /// <summary>
        /// Тип станции (станция/пост)
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public WeatherStationType? WeatherStationType { get; set; }
    }

    public class WeatherStationHistory : BaseHistoryEntity<WeatherStation> { }
}
