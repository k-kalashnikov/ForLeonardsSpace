using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Indices
{
    [PartitionedTable]
    public class AnomalyPoint : BaseIndexPoint
    {
        public AnomalyType? AnomalyType { get; set; }
        public string? Color { get; set; }
        public Guid? AnomalyPolygonId { get; set; }
    }

    [PartitionedTable]
    public class AnomalyPolygon : BaseEntity
    {
        public DateTime OriginalDate { get; set; }

        public ProductSourceType ProductSourceType { get; set; }

        public Guid SatelliteProductId { get; set; }

        public AnomalyType AnomalyType { get; set; }

        public string Color { get; set; }

        public Guid? RegionId { get; set; }

        public Guid? FieldId { get; set; }

        public Guid? SeasonId { get; set; }

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
                        if (geometry is NetTopologySuite.Geometries.Polygon polygon)
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
    }

    /// <summary>
    /// Типы аномалий
    /// </summary>
    public enum AnomalyType
    {
        /// <summary>
        /// Подтопление
        /// </summary>
        Flooding = 1,

        /// <summary>
        /// Переувлажнение
        /// </summary>
        Overwatering = 2,

        /// <summary>
        /// Засуха
        /// </summary>
        Drought = 3,

        /// <summary>
        /// Пропуск Всходов
        /// </summary>
        SkippingOfSeedlings = 4,

        /// <summary>
        /// Раннее Старение
        /// </summary>
        PrematureAging = 5,

        /// <summary>
        /// Питательный Стресс
        /// </summary>
        NutritionalStress = 6
    }
}
