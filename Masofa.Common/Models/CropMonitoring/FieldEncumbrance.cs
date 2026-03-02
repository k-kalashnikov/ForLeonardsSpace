namespace Masofa.Common.Models.CropMonitoring
{
    /// <summary>
    /// Обременения по полю
    /// </summary>
    public class FieldEncumbrance : BaseEntity
    {
        /// <summary>
        /// Идентификатор поля
        /// </summary>
        public Guid FieldId { get; set; }

        /// <summary>
        /// Дата начала
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Дата окончания
        /// </summary>
        public DateOnly EndDate { get; set; }

        /// <summary>
        /// Тип обременения - ссылка на справочник Encumbrance
        /// </summary>
        public Guid EncumbranceId { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        public string? Comment { get; set; }
    }

    public class FieldEncumbranceHistory : BaseHistoryEntity<FieldEncumbrance> { }
}

