using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Landsat
{
    /// <summary>
    /// Сущность для хранения ассетов спектральных каналов Surface Reflectance (SR) в базе данных
    /// </summary>
    [PartitionedTable]
    [Table("SrBandAssets")]
    public class SrBandAssetEntity : SpecializedAssetEntity
    {
        /// <summary>
        /// Номер канала (например, 1, 2, 3, 4, 5, 6, 7)
        /// </summary>
        public int BandNumber { get; set; }

        /// <summary>
        /// Общее название канала (например, "coastal", "blue", "green", "red", "nir", "swir16", "swir22")
        /// </summary>
        [MaxLength(50)]
        public string CommonName { get; set; } = default!;

        /// <summary>
        /// Центральная длина волны в микрометрах
        /// </summary>
        public double CenterWavelength { get; set; }

        /// <summary>
        /// Пространственное разрешение (GSD) в метрах
        /// </summary>
        public int SpatialResolution { get; set; }

        /// <summary>
        /// Минимальное значение пикселя
        /// </summary>
        public int? MinPixelValue { get; set; }

        /// <summary>
        /// Максимальное значение пикселя
        /// </summary>
        public int? MaxPixelValue { get; set; }

        /// <summary>
        /// Коэффициент масштабирования для преобразования значений пикселей в физические величины
        /// </summary>
        public double? ScaleFactor { get; set; }

        /// <summary>
        /// Смещение для преобразования значений пикселей в физические величины
        /// </summary>
        public double? Offset { get; set; }

        /// <summary>
        /// Единица измерения физических величин
        /// </summary>
        [MaxLength(50)]
        public string? Unit { get; set; }

        /// <summary>
        /// Создает новый экземпляр SrBandAssetEntity на основе ассета STAC
        /// </summary>
        /// <param name="stacAssetId">Идентификатор ассета STAC</param>
        /// <param name="stacFeatureId">Идентификатор объекта STAC</param>
        /// <param name="bandNumber">Номер канала</param>
        /// <param name="commonName">Общее название канала</param>
        /// <param name="centerWavelength">Центральная длина волны в микрометрах</param>
        /// <param name="spatialResolution">Пространственное разрешение в метрах</param>
        /// <returns>Новый экземпляр SrBandAssetEntity</returns>
        public static SrBandAssetEntity Create(
            Guid stacAssetId,
            Guid stacFeatureId,
            int bandNumber,
            string commonName,
            double centerWavelength,
            int spatialResolution)
        {
            return new SrBandAssetEntity
            {
                StacAssetId = stacAssetId,
                StacFeatureId = stacFeatureId,
                BandNumber = bandNumber,
                CommonName = commonName,
                CenterWavelength = centerWavelength,
                SpatialResolution = spatialResolution
            };
        }
    }

    public class SrBandAssetEntityHistory : BaseHistoryEntity<SrBandAssetEntity> { }
}
