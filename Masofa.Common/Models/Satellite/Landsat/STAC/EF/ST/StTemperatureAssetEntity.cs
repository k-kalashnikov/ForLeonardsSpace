using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Landsat
{
    /// <summary>
    /// Сущность для хранения ассетов температуры поверхности Surface Temperature (ST) в базе данных
    /// </summary>
    [PartitionedTable]
    [Table("StTemperatureAssets")]
    public class StTemperatureAssetEntity : SpecializedAssetEntity
    {
        /// <summary>
        /// Тип ассета температуры (например, "lwir11" для B10)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string TemperatureType { get; set; } = default!;

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
        /// Единица измерения физических величин (например, "K" для Кельвинов)
        /// </summary>
        [MaxLength(50)]
        public string? Unit { get; set; }

        /// <summary>
        /// Создает новый экземпляр StTemperatureAssetEntity на основе ассета STAC
        /// </summary>
        /// <param name="stacAssetId">Идентификатор ассета STAC</param>
        /// <param name="stacFeatureId">Идентификатор объекта STAC</param>
        /// <param name="temperatureType">Тип ассета температуры</param>
        /// <param name="centerWavelength">Центральная длина волны в микрометрах</param>
        /// <param name="spatialResolution">Пространственное разрешение в метрах</param>
        /// <returns>Новый экземпляр StTemperatureAssetEntity</returns>
        public static StTemperatureAssetEntity Create(
            Guid stacAssetId,
            Guid stacFeatureId,
            string temperatureType,
            double centerWavelength,
            int spatialResolution)
        {
            return new StTemperatureAssetEntity
            {
                StacAssetId = stacAssetId,
                StacFeatureId = stacFeatureId,
                TemperatureType = temperatureType,
                CenterWavelength = centerWavelength,
                SpatialResolution = spatialResolution
            };
        }
    }

    public class StTemperatureAssetEntityHistory : BaseHistoryEntity<StTemperatureAssetEntity> { }
}
