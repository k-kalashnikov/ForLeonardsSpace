using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Агроклиматические зоны
    /// </summary>
    /// <remarks>
    /// Справочник агроклиматических зон
    /// </remarks>
    public class AgroclimaticZone : NamedDictionaryItem
    {
        /// <summary>
        /// Регионы, относящиеся к этой агроклиматической зоне
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<Region> Regions { get; set; } = [];

        /// <summary>
        /// Полигон
        /// </summary>
        [SwaggerIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public NetTopologySuite.Geometries.Geometry? Polygon { get; set; }

        /// <summary>
        /// Полигон в виде текста (JSON)
        /// </summary>
        public string? PolygonJson
        {
            get
            {
                var poly = Polygon;
                if (poly == null || poly.IsEmpty)
                    return null;

                return poly.AsText();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Polygon = null;
                }
                else
                {
                    try
                    {
                        var reader = new NetTopologySuite.IO.WKTReader();
                        var geometry = reader.Read(value);
                        if (geometry is NetTopologySuite.Geometries.Polygon polygon)
                        {
                            NetTopologySuite.Geometries.MultiPolygon mpoly = polygon.Factory.CreateMultiPolygon([polygon]);
                            Polygon = mpoly;
                        }
                        else if (geometry is NetTopologySuite.Geometries.MultiPolygon multiPolygon)
                        {
                            Polygon = multiPolygon;
                        }
                        else
                        {
                            Polygon = null;
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }
    }

    public class AgroclimaticZoneHistory : BaseHistoryEntity<AgroclimaticZone> { }
}

