using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Indices
{
    public class BaseIndex
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public DateTime CreateAt { get; set; }

        public ProductSourceType ProductSourceType { get; set; }

        public Guid SatelliteProductId { get; set; }
    }



    public class BaseIndexPoint : BaseIndex
    {
        [NotMapped]
        public virtual float Value { get; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public NetTopologySuite.Geometries.Point Point { get; set; }

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
                        if (geometry is Point point)
                        {
                            Point = point;
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

        public Guid? RegionId { get; set; }
        public Guid? FieldId { get; set; }
        public Guid? SeasonId { get; set; }
    }

    public class BaseIndexPolygon : BaseIndex
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public NetTopologySuite.Geometries.Polygon Polygon { get; set; }

        [NotMapped]
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
                        if (geometry is Polygon polygon)
                        {
                            Polygon = polygon;
                        }
                        else
                        {
                            Polygon = null;
                        }
                    }
                    catch
                    {
                        Polygon = null;
                    }
                }
            }
        }

        public Guid FileStorageItemId { get; set; }
        public bool IsColored { get; set; } = true;

        /// <summary>
        /// Путь к сжатому снимку (PreviewImage) в FileStorage
        /// </summary>
        public Guid? PreviewImagePath { get; set; }

        [NotMapped]
        public List<Masofa.Common.Models.CropMonitoring.Field> Fields { get; set; } = new List<CropMonitoring.Field>();

        [NotMapped]
        public List<Masofa.Common.Models.CropMonitoring.Season> Seasons { get; set; } = new List<CropMonitoring.Season>();

        [NotMapped]
        public List<Masofa.Common.Models.Dictionaries.Region> Regions { get; set; } = new List<Dictionaries.Region>();
    }
}
