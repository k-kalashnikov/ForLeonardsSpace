using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Landsat
{
    /// <summary>
    /// Сущность для хранения вспомогательных ассетов Surface Temperature (ST) в базе данных
    /// </summary>
    [PartitionedTable]
    [Table("StAuxiliaryAssets")]
    public class StAuxiliaryAssetEntity : SpecializedAssetEntity
    {
        /// <summary>
        /// Тип вспомогательного ассета (например, "TRAD", "URAD", "DRAD", "ATRAN", "EMIS", "EMSD", "CDIST")
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string AuxiliaryType { get; set; } = default!;

        /// <summary>
        /// Описание вспомогательного ассета
        /// </summary>
        [MaxLength(1000)]
        public string Description { get; set; } = default!;

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
        /// Создает новый экземпляр StAuxiliaryAssetEntity на основе ассета STAC
        /// </summary>
        /// <param name="stacAssetId">Идентификатор ассета STAC</param>
        /// <param name="stacFeatureId">Идентификатор объекта STAC</param>
        /// <param name="auxiliaryType">Тип вспомогательного ассета</param>
        /// <param name="description">Описание вспомогательного ассета</param>
        /// <param name="spatialResolution">Пространственное разрешение в метрах</param>
        /// <returns>Новый экземпляр StAuxiliaryAssetEntity</returns>
        public static StAuxiliaryAssetEntity Create(
            Guid stacAssetId,
            Guid stacFeatureId,
            string auxiliaryType,
            string description,
            int spatialResolution)
        {
            return new StAuxiliaryAssetEntity
            {
                StacAssetId = stacAssetId,
                StacFeatureId = stacFeatureId,
                AuxiliaryType = auxiliaryType,
                Description = description,
                SpatialResolution = spatialResolution
            };
        }
    }

    public class StAuxiliaryAssetEntityHistory : BaseHistoryEntity<StAuxiliaryAssetEntity> { }
}
