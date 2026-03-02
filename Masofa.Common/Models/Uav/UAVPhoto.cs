using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Masofa.Common.Models.Uav
{
    public enum SurveyStatus
    {
        New = 0,
        Processing = 1,
        Ready = 2,
        Error = 3
    }

    /// <summary>
    /// Карточка аэросъемки (Вылет).
    /// </summary>
    public class UAVFlyPath : BaseEntity
    {
        public string? Comment { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Geometry? FlyPath { get; set; }

        [NotMapped]
        public string? FlyPathJson
        {
            get => FlyPath?.IsEmpty == false ? FlyPath.AsText() : null;
            set { }
        }

        public SurveyStatus ProcessingStatus { get; set; }
        public Guid? DataTypeId { get; set; }
        public Guid? CameraTypeId { get; set; }
    }

    public class UAVPhotoCollection : BaseEntity
    {
        public Guid UAVFlyPathId { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Point? Point { get; set; }

        [NotMapped]
        public string? PointJson
        {
            get => Point?.IsEmpty == false ? Point.AsText() : null;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Point = null;
                    return;
                }
                try
                {
                    var reader = new NetTopologySuite.IO.WKTReader();
                    var geometry = reader.Read(value);
                    if (geometry is Point point)
                    {
                        Point = point;
                        Point.SRID = 4326;
                    }
                    else Point = null;
                }
                catch { Point = null; }
            }
        }
        public bool AnalysisOnly { get; set; }
        public Guid? PreviewFileStorageId { get; set; }
    }

    public class UAVPhoto : BaseEntity
    {
        public string Title { get; set; }
        public Guid FileStorageId { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public DateOnly OriginalDate { get; set; }
        public string? Comment { get; set; }
        public Guid UAVPhotoCollectionId { get; set; }
    }

    /// <summary>
    /// Связь снимков с бизнесом (поля, сезоны).
    /// </summary>
    public class UAVPhotoCollectionRelation : BaseEntity
    {
        public Guid? RegionId { get; set; }
        public Guid? SeasonId { get; set; }
        public Guid? FieldId { get; set; }
        public Guid? CropId { get; set; }
        public Guid? FirmId { get; set; }
        public Guid UAVPhotoCollectionId { get; set; }
    }
}