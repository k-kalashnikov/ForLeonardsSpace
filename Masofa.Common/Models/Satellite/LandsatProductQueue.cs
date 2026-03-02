namespace Masofa.Common.Models.Satellite
{
    /// <summary>
    /// Модель элемента очереди загрузки продукта с Copernicus => Sentinel2
    /// </summary>
    public class LandsatProductQueue : BaseEntity
    {
        /// <summary>
        /// Первичный ключ продукта//снимка на Copernicus
        /// </summary>
        public string? ProductId { get; set; }

        /// <summary>
        /// Дата создания записи
        /// </summary>
        public DateTime CreateAt { get; set; }

        /// <summary>
        /// Статус элемента очереди загрузки
        /// </summary>
        public ProductQueueStatusType QueueStatus { get; set; }
        public ProductQueueStatusType Status { get; set; }
    }

    public class LandsatProductQueueHistory : BaseHistoryEntity<LandsatProductQueue> { }
}
