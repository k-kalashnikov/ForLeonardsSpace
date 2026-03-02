using System.ComponentModel.DataAnnotations.Schema;
using Masofa.Common.Models;

namespace Masofa.Common.Models.IBMWeather;

/// <summary>
/// Weather alert flood information entity
/// </summary>
[Table("IBMWeatherAlertFloodInfos")]
public class IBMWeatherAlertFloodInfo : BaseEntity
{
    /// <summary>
    /// Идентификатор алерта
    /// </summary>
    public Guid WeatherAlertId { get; set; }

    /// <summary>
    /// Время пика наводнения (локальное)
    /// </summary>
    public DateTime? FloodCrestTimeLocal { get; set; }

    /// <summary>
    /// Часовой пояс времени пика наводнения
    /// </summary>
    public string? FloodCrestTimeLocalTimeZone { get; set; }

    /// <summary>
    /// Время окончания наводнения (локальное)
    /// </summary>
    public DateTime? FloodEndTimeLocal { get; set; }

    /// <summary>
    /// Часовой пояс времени окончания наводнения
    /// </summary>
    public string? FloodEndTimeLocalTimeZone { get; set; }

    /// <summary>
    /// Немедленная причина наводнения
    /// </summary>
    public string? FloodImmediateCause { get; set; }

    /// <summary>
    /// Код немедленной причины наводнения
    /// </summary>
    public string? FloodImmediateCauseCode { get; set; }

    /// <summary>
    /// Идентификатор локации наводнения
    /// </summary>
    public string? FloodLocationId { get; set; }

    /// <summary>
    /// Название локации наводнения
    /// </summary>
    public string? FloodLocationName { get; set; }

    /// <summary>
    /// Статус записи наводнения
    /// </summary>
    public string? FloodRecordStatus { get; set; }

    /// <summary>
    /// Код статуса записи наводнения
    /// </summary>
    public string? FloodRecordStatusCode { get; set; }

    /// <summary>
    /// Серьезность наводнения
    /// </summary>
    public string? FloodSeverity { get; set; }

    /// <summary>
    /// Код серьезности наводнения
    /// </summary>
    public string? FloodSeverityCode { get; set; }

    /// <summary>
    /// Время начала наводнения (локальное)
    /// </summary>
    public DateTime? FloodStartTimeLocal { get; set; }

    /// <summary>
    /// Часовой пояс времени начала наводнения
    /// </summary>
    public string? FloodStartTimeLocalTimeZone { get; set; }
}
