using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Landsat
{
    /// <summary>
    /// Сущность для хранения ассетов качества данных Surface Reflectance (SR) в базе данных
    /// </summary>
    [PartitionedTable]
    [Table("SrQualityAssets")]
    public class SrQualityAssetEntity : SpecializedAssetEntity
    {
        /// <summary>
        /// Тип ассета качества (например, "qa_pixel", "qa_radsat", "qa_aerosol")
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string QualityType { get; set; } = default!;

        /// <summary>
        /// Описание битовых полей, сохраненное в виде JSON
        /// </summary>
        [Column(TypeName = "text")]
        public string BitfieldsJson { get; set; } = default!;

        /// <summary>
        /// Пространственное разрешение (GSD) в метрах
        /// </summary>
        public int SpatialResolution { get; set; }

        /// <summary>
        /// Создает новый экземпляр SrQualityAssetEntity на основе ассета STAC
        /// </summary>
        /// <param name="stacAssetId">Идентификатор ассета STAC</param>
        /// <param name="stacFeatureId">Идентификатор объекта STAC</param>
        /// <param name="qualityType">Тип ассета качества</param>
        /// <param name="bitfields">Битовые поля в виде объекта</param>
        /// <param name="spatialResolution">Пространственное разрешение в метрах</param>
        /// <returns>Новый экземпляр SrQualityAssetEntity</returns>
        public static SrQualityAssetEntity Create(
            Guid stacAssetId,
            Guid stacFeatureId,
            string qualityType,
            object bitfields,
            int spatialResolution)
        {
            return new SrQualityAssetEntity
            {
                StacAssetId = stacAssetId,
                StacFeatureId = stacFeatureId,
                QualityType = qualityType,
                BitfieldsJson = Newtonsoft.Json.JsonConvert.SerializeObject(bitfields),
                SpatialResolution = spatialResolution
            };
        }
    }

    public class SrQualityAssetEntityHistory : BaseHistoryEntity<SrQualityAssetEntity> { }
}
