using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.IBMWeather;

/// <summary>
/// Погодные данные IBM Weather API
/// </summary>
[PartitionedTable]
[Table("IBMWeatherData")]
public class IBMWeatherData : BaseEntity
{
    /// <summary>
    /// Идентификатор метеостанции IBM
    /// </summary>
    public Guid IBMMeteoStationId { get; set; }

    /// <summary>
    /// Время валидности данных (UTC)
    /// </summary>
    public DateTime ValidTimeUtc { get; set; }

    /// <summary>
    /// Температура
    /// </summary>
    public int? Temperature { get; set; }

    /// <summary>
    /// Влажность
    /// </summary>
    public int? Humidity { get; set; }

    /// <summary>
    /// Скорость ветра
    /// </summary>
    public int? WindSpeed { get; set; }

    /// <summary>
    /// Направление ветра
    /// </summary>
    public int? WindDirection { get; set; }

    /// <summary>
    /// Осадки
    /// </summary>
    public double? Precipitation { get; set; }

    /// <summary>
    /// УФ-индекс
    /// </summary>
    public int? UvIndex { get; set; }

    /// <summary>
    /// Максимальная температура
    /// </summary>
    public int? TemperatureMax { get; set; }

    /// <summary>
    /// Минимальная температура
    /// </summary>
    public int? TemperatureMin { get; set; }

    /// <summary>
    /// День или ночь
    /// </summary>
    public string? DayOrNight { get; set; }

    /// <summary>
    /// Вероятность осадков
    /// </summary>
    public int? PrecipChance { get; set; }

    /// <summary>
    /// Количественный прогноз осадков
    /// </summary>
    public double? Qpf { get; set; }

    /// <summary>
    /// Количественный прогноз снега
    /// </summary>
    public double? QpfSnow { get; set; }

    /// <summary>
    /// Относительная влажность
    /// </summary>
    public int? RelativeHumidity { get; set; }

    /// <summary>
    /// День недели
    /// </summary>
    public string? DayOfWeek { get; set; }

    /// <summary>
    /// Запрошенная широта
    /// </summary>
    public double? RequestedLatitude { get; set; }

    /// <summary>
    /// Запрошенная долгота
    /// </summary>
    public double? RequestedLongitude { get; set; }

    /// <summary>
    /// Идентификатор точки сетки
    /// </summary>
    public string? GridpointId { get; set; }
}
