using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Карты регионов
    /// </summary>
    /// <remarks>
    /// Справочник карт регионов
    /// </remarks>
    public class RegionMap : BaseDictionaryItem
    {
        /// <summary>
        /// Широта
        /// </summary>
        public decimal? Lat { get; set; }

        /// <summary>
        /// Долгота
        /// </summary>
        public decimal? Lng { get; set; }

        /// <summary>
        /// По оси Х
        /// </summary>
        public int? MozaikX { get; set; }

        /// <summary>
        /// По оси Y
        /// </summary>
        public int? MozaikY { get; set; }

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

        /// <summary>
        /// Массив полигонов в виде текста (JSON)
        /// </summary>
        [NotMapped]
        public List<string>? PolygonArrayJson
        {
            get
            {
                var poly = Polygon;
                if (poly == null || poly.IsEmpty)
                {
                    return null;
                }

                if (Polygon is NetTopologySuite.Geometries.Polygon polygon)
                {
                    return [polygon.AsText()];
                }
                else if (Polygon is NetTopologySuite.Geometries.MultiPolygon multiPolygon)
                {
                    NetTopologySuite.Geometries.Polygon[] polygons = multiPolygon.Geometries.Cast<NetTopologySuite.Geometries.Polygon>().ToArray();
                    List<string> result = [];
                    foreach (var p in polygons)
                    {
                        result.Add(p.AsText());
                    }

                    return result;
                }

                return [poly.AsText()];
            }
            set
            {
                if (value == null)
                {
                    Polygon = null;
                }
                else
                {
                    try
                    {
                        List<NetTopologySuite.Geometries.Polygon> polygons = [];
                        var reader = new NetTopologySuite.IO.WKTReader();
                        foreach (var p in value)
                        {
                            var geometry = reader.Read(p);
                            if (geometry is NetTopologySuite.Geometries.Polygon polygon)
                            {
                                polygons.Add(polygon);
                            }
                            else if (geometry is NetTopologySuite.Geometries.MultiPolygon multiPolygon)
                            {
                                polygons.AddRange(multiPolygon.Geometries.Cast<NetTopologySuite.Geometries.Polygon>().ToArray());
                            }
                        }

                        if (polygons.Count > 0)
                        {
                            var factory = new NetTopologySuite.Geometries.GeometryFactory(new NetTopologySuite.Geometries.PrecisionModel(), 4326);
                            NetTopologySuite.Geometries.MultiPolygon result = factory.CreateMultiPolygon([.. polygons]);
                            Polygon = result;
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

        /// <summary>
        /// Регионы
        /// </summary>
        [NotMapped]
        [ValidateNever]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [SwaggerIgnore]
        public Region? Region { get; set; }
    }

    public class RegionMapHistory : BaseHistoryEntity<RegionMap> { }

    public class RegionMapShort
    {
        public List<string>? PolygonArrayJson { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lng { get; set; }
        public Guid Id { get; set; }

    }
}
