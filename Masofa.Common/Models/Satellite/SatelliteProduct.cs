using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite
{
    public class SatelliteProduct : BaseEntity
    {
        /// <summary>
        /// Ключ продукта//снимка в источнике данных
        /// </summary>
        [Required]
        public string? ProductId { get; set; }

        /// <summary>
        /// Тип источника данных
        /// </summary>
        public ProductSourceType ProductSourceType { get; set; }

        /// <summary>
        /// Тип для определения серилизации мета-данных
        /// </summary>
        [NotMapped]
        public Type ProductType { get; }

        /// <summary>
        /// Путь к медиа-данным в FileStorage
        /// </summary>
        public Guid MediadataPath { get; set; }

        /// <summary>
        /// Путь к сжатому снимку (PreviewImage) в FileStorage
        /// </summary>
        public Guid? PreviewImagePath { get; set; }

        public DateTime? OriginDate { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public NetTopologySuite.Geometries.Polygon? Polygon { get; set; }

        [NotMapped]
        public string? PolygonJson
        {
            get
            {
                var poly = Polygon;
                if (poly == null || poly.IsEmpty)
                {
                    return null;
                }

                return poly.AsText();
            }
        }
    }

    /// <summary>
    /// Тип источника данных
    /// </summary>
    public enum ProductSourceType
    {
        Custom = 0,
        Sentinel2 = 1,
        Landsat = 2
    }

    //public enum ProductQueueStatusType
    //{
    //    New = 0,
    //    MetadataLoaded = 1,
    //    MediaLoaded = 2,
    //    Parsed = 3,
    //    PreviewGenerated = 4,
    //    ArviTiff = 5,
    //    EviTiff = 6,
    //    GndviTiff = 7,
    //    MndwiTiff = 8,
    //    NdmiTiff = 9,
    //    NdviTiff = 10,
    //    OrviTiff = 11,
    //    OsaviTiff = 12,
    //    ArviDb = 13,
    //    EviDb = 14,
    //    GndviDb = 15,
    //    MndwiDb = 16,
    //    NdmiDb = 17,
    //    NdviDb = 18,
    //    OrviDb = 19,
    //    OsaviDb = 20,
    //    GeoserverImported = 21,
    //    GeoserverImportedIndex = 22
    //}

    public enum ProductQueueStatusType
    {
        New = 0,
        MetadataLoaded = 1,
        MediaLoaded = 2,
        Parsed = 3,
        IndicesComplite = 5,
        PreviewGenerated = 4,
        GeoserverImported = 6,
        GeoserverImportedIndex = 7,
    }

    public class SatelliteProductHistory : BaseHistoryEntity<SatelliteProduct> { }
}
