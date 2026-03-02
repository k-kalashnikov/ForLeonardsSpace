using Masofa.Common.Attributes;
using Masofa.Common.Models.Satellite.Parse.Landsat.STAC;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Landsat
{
    /// <summary>
    /// Сущность для хранения ссылок STAC в базе данных
    /// </summary>
    [PartitionedTable]
    [Table("StacLinks")]
    public class StacLinkEntity : BaseEntity
    {
        /// <summary>
        /// Идентификатор объекта STAC
        /// </summary>
        public Guid StacFeatureId { get; set; }

        /// <summary>
        /// Отношение ссылки (например, "root", "parent", "collection", "self")
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Rel { get; set; } = default!;

        /// <summary>
        /// URL ссылки
        /// </summary>
        [Required]
        [MaxLength(2000)]
        public string Href { get; set; } = default!;

        /// <summary>
        /// Тип ссылки (опционально)
        /// </summary>
        [MaxLength(100)]
        public string? Type { get; set; }

        /// <summary>
        /// Заголовок ссылки (опционально)
        /// </summary>
        [MaxLength(255)]
        public string? Title { get; set; }

        /// <summary>
        /// Преобразует StacLink в StacLinkEntity
        /// </summary>
        /// <param name="stacLink">Объект StacLink</param>
        /// <param name="stacFeatureId">Идентификатор объекта STAC</param>
        /// <returns>Объект StacLinkEntity</returns>
        public static StacLinkEntity FromStacLink(StacLink stacLink, Guid stacFeatureId)
        {
            return new StacLinkEntity
            {
                StacFeatureId = stacFeatureId,
                Rel = stacLink.Rel,
                Href = stacLink.Href
            };
        }
    }

    public class StacLinkEntityHistory : BaseHistoryEntity<StacLinkEntity> { }
}
