using Masofa.Common.Attributes;
using Masofa.Common.Models.Satellite.Parse.Landsat.STAC;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Landsat
{
    /// <summary>
    /// Сущность для хранения ассетов STAC в базе данных
    /// </summary>
    [PartitionedTable]
    [Table("StacAssets")]
    public class StacAssetEntity : BaseEntity
    {
        /// <summary>
        /// Идентификатор объекта STAC
        /// </summary>
        public Guid StacFeatureId { get; set; }

        /// <summary>
        /// Ключ ассета в словаре ассетов
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string AssetKey { get; set; } = default!;

        /// <summary>
        /// Заголовок ассета
        /// </summary>
        [MaxLength(255)]
        public string Title { get; set; } = default!;

        /// <summary>
        /// Описание ассета (опционально)
        /// </summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// MIME-тип ассета
        /// </summary>
        [MaxLength(100)]
        public string Type { get; set; } = default!;

        /// <summary>
        /// Роли ассета, сохраненные в виде JSON
        /// </summary>
        [Column(TypeName = "text")]
        public string RolesJson { get; set; } = default!;

        /// <summary>
        /// URL ассета
        /// </summary>
        [Required]
        [MaxLength(2000)]
        public string Href { get; set; } = default!;

        /// <summary>
        /// Альтернативные хранилища ассета, сохраненные в виде JSON (опционально)
        /// </summary>
        [Column(TypeName = "text")]
        public string? AlternateJson { get; set; }

        /// <summary>
        /// Информация о полосах ассета, сохраненная в виде JSON (опционально)
        /// </summary>
        [Column(TypeName = "text")]
        public string? EoBandsJson { get; set; }

        /// <summary>
        /// Информация о битовых полях классификации ассета, сохраненная в виде JSON (опционально)
        /// </summary>
        [Column(TypeName = "text")]
        public string? ClassificationBitfieldsJson { get; set; }

        /// <summary>
        /// Размер ассета в байтах (опционально)
        /// </summary>
        public long? SizeInBytes { get; set; }

        /// <summary>
        /// Контрольная сумма ассета (опционально)
        /// </summary>
        [MaxLength(255)]
        public string? Checksum { get; set; }

        /// <summary>
        /// Алгоритм контрольной суммы (опционально)
        /// </summary>
        [MaxLength(50)]
        public string? ChecksumAlgorithm { get; set; }

        /// <summary>
        /// Локальный путь к файлу ассета (опционально)
        /// </summary>
        [MaxLength(2000)]
        public string? LocalPath { get; set; }

        /// <summary>
        /// Статус загрузки ассета (опционально)
        /// </summary>
        [MaxLength(50)]
        public string? DownloadStatus { get; set; }

        /// <summary>
        /// Преобразует пару ключ-значение из словаря ассетов в StacAssetEntity
        /// </summary>
        /// <param name="assetKey">Ключ ассета</param>
        /// <param name="stacAsset">Объект StacAsset</param>
        /// <param name="stacFeatureId">Идентификатор объекта STAC</param>
        /// <returns>Объект StacAssetEntity</returns>
        public static StacAssetEntity FromStacAsset(string assetKey, StacAsset stacAsset, Guid stacFeatureId)
        {
            return new StacAssetEntity
            {
                StacFeatureId = stacFeatureId,
                AssetKey = assetKey,
                Title = stacAsset.Title,
                Description = stacAsset.Description,
                Type = stacAsset.Type,
                RolesJson = Newtonsoft.Json.JsonConvert.SerializeObject(stacAsset.Roles),
                Href = stacAsset.Href,
                AlternateJson = stacAsset.Alternate != null ? Newtonsoft.Json.JsonConvert.SerializeObject(stacAsset.Alternate) : null,
                EoBandsJson = stacAsset.EoBands != null ? Newtonsoft.Json.JsonConvert.SerializeObject(stacAsset.EoBands) : null,
                ClassificationBitfieldsJson = stacAsset.ClassificationBitfields != null ? Newtonsoft.Json.JsonConvert.SerializeObject(stacAsset.ClassificationBitfields) : null
            };
        }
    }

    public class StacAssetEntityHistory : BaseHistoryEntity<StacAssetEntity> { }
}
