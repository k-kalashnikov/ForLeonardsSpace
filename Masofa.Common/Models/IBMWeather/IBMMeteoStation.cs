using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.IBMWeather;

/// <summary>
/// Метеостанция IBM Weather API
/// </summary>
[Table("IBMMeteoStations")]
public class IBMMeteoStation : BaseNamedEntity
{
    /// <summary>
    /// Идентификатор региона
    /// </summary>
    public Guid? RegionId { get; set; }

    /// <summary>
    /// Идентификатор агроклиматической зоны
    /// </summary>
    public Guid? AgroclimaticZoneId { get; set; }

    /// <summary>  
    /// Широта/Долгота  
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public NetTopologySuite.Geometries.Point Point { get; set; } = new NetTopologySuite.Geometries.Point(0, 0);

    /// <summary>
    /// Город
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Страна
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Код страны
    /// </summary>
    public string? CountryCode { get; set; }

    /// <summary>
    /// Отображаемое имя
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Административный округ
    /// </summary>
    public string? AdminDistrict { get; set; }

    /// <summary>
    /// Код административного округа
    /// </summary>
    public string? AdminDistrictCode { get; set; }

    /// <summary>
    /// IATA код аэропорта
    /// </summary>
    public string? IataCode { get; set; }

    /// <summary>
    /// ICAO код станции/аэропорта
    /// </summary>
    public string? IcaoCode { get; set; }

    /// <summary>
    /// Идентификатор личной станции (PWS)
    /// </summary>
    public string? PwsId { get; set; }

    /// <summary>
    /// Legacy/внутренний код IBM
    /// </summary>
    public string? LocId { get; set; }

    /// <summary>
    /// Идентификатор места
    /// </summary>
    public string? PlaceId { get; set; }

    /// <summary>
    /// Почтовый ключ
    /// </summary>
    public string? PostalKey { get; set; }

    /// <summary>
    /// Тип станции (city, pws, airport и т.д.)
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Признак активности станции
    /// </summary>
    public bool IsActive { get; set; } = true;

    [NotMapped]
    public string? PointJson
    {
        get
        {
            var point = Point;
            if (point == null || point.IsEmpty)
                return null;

            return point.AsText();
        }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                Point = null;
            }
            else
            {
                try
                {
                    var reader = new NetTopologySuite.IO.WKTReader();
                    var geometry = reader.Read(value);
                    if (geometry is NetTopologySuite.Geometries.Point polygon)
                    {
                        Point = polygon;
                    }
                    else
                    {
                        Point = null;
                    }
                }
                catch
                {
                    Point = null;
                }
            }
        }
    }
}
