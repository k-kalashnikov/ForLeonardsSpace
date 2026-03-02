namespace Masofa.Common.Models.CropMonitoring
{
    /// <summary>
    /// Финансовые условия по полю
    /// </summary>
    public class FieldFinancialCondition : BaseEntity
    {
        /// <summary>
        /// Идентификатор поля
        /// </summary>
        public Guid FieldId { get; set; }

        /// <summary>
        /// Дотации - ссылка на справочник FinancialCondition
        /// </summary>
        public Guid FinancialConditionId { get; set; }

        /// <summary>
        /// Дата начала
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Дата окончания
        /// </summary>
        public DateOnly EndDate { get; set; }

        /// <summary>
        /// Сумма
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Цель
        /// </summary>
        public string? Purpose { get; set; }
    }

    public class FieldFinancialConditionHistory : BaseHistoryEntity<FieldFinancialCondition> { }
}

