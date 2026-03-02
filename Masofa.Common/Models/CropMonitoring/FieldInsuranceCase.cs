namespace Masofa.Common.Models.CropMonitoring
{
    /// <summary>
    /// Страховые случаи по полю
    /// </summary>
    public class FieldInsuranceCase : BaseEntity
    {
        /// <summary>
        /// Идентификатор поля
        /// </summary>
        public Guid FieldId { get; set; }

        /// <summary>
        /// Дата выплаты
        /// </summary>
        public DateOnly PaymentDate { get; set; }

        /// <summary>
        /// Сумма выплаты
        /// </summary>
        public decimal PaymentAmount { get; set; }

        /// <summary>
        /// Вид ущерба - ссылка на справочник InsuranceCase
        /// </summary>
        public Guid InsuranceCaseId { get; set; }

        /// <summary>
        /// Признак страхового случая
        /// </summary>
        public bool IsInsuranceCase { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        public string? Comment { get; set; }
    }

    public class FieldInsuranceCaseHistory : BaseHistoryEntity<FieldInsuranceCase> { }
}

