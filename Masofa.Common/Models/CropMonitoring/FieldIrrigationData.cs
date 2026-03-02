namespace Masofa.Common.Models.CropMonitoring
{
    /// <summary>
    /// Данные по орошению поля
    /// </summary>
    public class FieldIrrigationData : BaseEntity
    {
        /// <summary>
        /// Идентификатор поля
        /// </summary>
        public Guid FieldId { get; set; }

        /// <summary>
        /// Годовой уровень на площадь
        /// </summary>
        public double? AnnualLevelPerArea { get; set; }

        /// <summary>
        /// Потребность
        /// </summary>
        public double? Requirement { get; set; }

        /// <summary>
        /// Факт
        /// </summary>
        public double? Actual { get; set; }

        /// <summary>
        /// Характеристики воды (соли)
        /// </summary>
        public double? WaterCharacteristics { get; set; }

        /// <summary>
        /// Год, к которому относятся данные
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        public string? Comment { get; set; }
    }

    public class FieldIrrigationDataHistory : BaseHistoryEntity<FieldIrrigationData> { }
}

