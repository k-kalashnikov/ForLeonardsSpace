using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.CropMonitoring
{
    public class FieldPhoto : BaseEntity
    {
        /// <summary>
        /// Название снимка
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Ссылка на файл в файловом хранилище
        /// </summary>
        public Guid FileStorageId { get; set; }

        /// <summary>
        /// Идентификатор поля
        /// </summary>
        public Guid? FieldId { get; set; }

        /// <summary>
        /// Идентификатор административной единицы (район/город)
        /// </summary>
        public Guid? RegionId { get; set; }

        /// <summary>
        /// Идентификатор родительского региона (область)
        /// </summary>
        public Guid? ParentRegionId { get; set; }

        /// <summary>
        /// Дата и время съемки (UTC)
        /// </summary>
        public DateTime? CaptureDateUtc { get; set; }

        /// <summary>
        /// Описание снимка
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Географическая точка, где был сделан снимок
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [ValidateNever]
        public NetTopologySuite.Geometries.Point? Point { get; set; }

        /// <summary>
        /// Широта точки съемки
        /// </summary>
        [NotMapped]
        public double? Latitude => Point != null && !Point.IsEmpty ? Point.Y : null;

        /// <summary>
        /// Долгота точки съемки
        /// </summary>
        [NotMapped]
        public double? Longitude => Point != null && !Point.IsEmpty ? Point.X : null;

        /// <summary>
        /// JSON-представление точки для десериализации (WKT формат: "POINT(longitude latitude)")
        /// </summary>
        [NotMapped]
        public string? PointJson
        {
            get
            {
                if (Point == null || Point.IsEmpty)
                {
                    return null;
                }

                return Point.AsText();
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
                        if (geometry is NetTopologySuite.Geometries.Point point)
                        {
                            Point = point;
                            Point.SRID = 4326;
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
}
