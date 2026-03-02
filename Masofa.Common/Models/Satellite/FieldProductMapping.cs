using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Masofa.Common.Models.Satellite;

namespace Masofa.Common.Models.Satellite
{
    /// <summary>
    /// Маппинг полей к спутниковым продуктам для быстрого поиска
    /// </summary>
    [Table("Field_Product_Mapping")]
    public class FieldProductMapping : BaseEntity
    {
        /// <summary>
        /// Идентификатор поля
        /// </summary>
        [Required]
        public Guid FieldId { get; set; }

        /// <summary>
        /// Идентификатор продукта
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string ProductId { get; set; } = default!;

        /// <summary>
        /// Тип спутника (Landsat, Sentinel2)
        /// </summary>
        [Required]
        public ProductSourceType SatelliteType { get; set; }
    }
}
