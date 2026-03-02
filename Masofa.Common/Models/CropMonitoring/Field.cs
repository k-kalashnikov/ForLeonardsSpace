using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.CropMonitoring
{
    /// <summary>
    /// Представляет информацию о поле (земельном участке) в аграрной системе.
    /// </summary>
    public partial class Field : BaseEntity
    {
        /// <summary>
        /// Название или имя поля.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Площадь поля в гектарах.
        /// </summary>
        public double? FieldArea { get; set; }


        /// <summary>
        /// Идентификатор региона, к которому относится поле.
        /// Ссылается на <see cref="Dictionaries.Region">словари <\see>
        /// </summary>
        public Guid? RegionId { get; set; }

        /// <summary>
        /// Идентификатор типа земельного участка.
        /// Ссылается на <see cref="Dictionaries.FieldType">словари</see>
        /// </summary>
        public Guid? FieldTypeId { get; set; }

        /// <summary>
        /// Внешний идентификатор поля для интеграции с внешними системами.
        /// </summary>
        public string? ExternalId { get; set; }

        /// <summary>
        /// Идентификатор типа почвы на данном поле.
        /// Ссылается на <see cref="Dictionaries.SoilType">словари <\see>
        /// </summary>
        public Guid? SoilTypeId { get; set; }

        /// <summary>
        /// Идентификатор агроклиматической зоны.
        /// Ссылается на <see cref="Dictionaries.AgroclimaticZone">словари<\see>
        /// </summary>
        public Guid? AgroclimaticZoneId { get; set; }

        /// <summary>
        /// Дополнительный комментарий или описание поля.
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// Идентификатор сельхозпроизводителя, которому принадлежит поле.
        /// Ссылается на <see cref="Dictionaries.Person">словари</see>.
        /// </summary>
        public Guid? AgricultureProducerId { get; set; }

        /// <summary>
        /// Идентификатор типа орошения, применяемого на поле.
        /// Ссылается на <see cref="Dictionaries.IrrigationMethod">словари</see>
        /// </summary>
        public Guid? IrrigationTypeId { get; set; }

        /// <summary>
        /// Идентификатор источника орошения (например, водоём, река, скважина).
        /// Ссылается на <see cref="Dictionaries.IrrigationSource">словари</see>
        /// </summary>
        public Guid? IrrigationSourceId { get; set; }

        /// <summary>
        /// Признак использования водосберегающих технологий на поле.
        /// </summary>
        public bool? WaterSaving { get; set; }

        /// <summary>
        /// Индекс качества почвы или её характеристика (например, плодородие).
        /// </summary>
        public double? SoilIndex { get; set; }

        /// <summary>
        /// Балл бонитет.
        /// Количественный показатель качества природных объектов (почв, лесов)
        /// 100 баллов - самый плодородный тип
        /// 1 балл - самый не плодородный тип
        /// </summary>
        public double? BonitetScore { get; set; }

        /// <summary>
        /// Классификатор.
        /// </summary>
        public double? Classifier { get; set; }

        /// <summary>
        /// Высота над уровнем моря в метрах.
        /// </summary>
        public double? AltitudeAboveSeaLevel { get; set; }

        /// <summary>
        /// Координаты границ поля.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Polygon? Polygon { get; set; }

        /// <summary>
        /// Признак контроля или мониторинга поля.
        /// Ссылается на <see cref="Dictionaries.IrrigationSource">словари</see>
        /// </summary>
        public bool? Control { get; set; }

        [NotMapped]
        public double? CenterX => (Polygon == null || Polygon.IsEmpty) ? null : (double.IsFinite(Polygon.Centroid?.X ?? double.NaN) ? Polygon.Centroid!.X : (double?)null);
        [NotMapped]
        public double? CenterY => (Polygon == null || Polygon.IsEmpty) ? null : (double.IsFinite(Polygon.Centroid?.Y ?? double.NaN) ? Polygon.Centroid!.Y : (double?)null);
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
    }

    public class FieldHistory : BaseHistoryEntity<Field> { }
}
