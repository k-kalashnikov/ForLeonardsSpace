using Masofa.Common.Attributes;
using Masofa.Common.Models.SystemCrical;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Notifications
{
    /// <summary>
	/// Сущность электронного письма
	/// </summary>
    [Table("EmailMessages")]
    [PartitionedTable]
    public class EmailMessage : BaseEntity
    {
        /// <summary>
        /// Отправитель
        /// </summary>
        public string Sender { get; set; } = string.Empty;

        /// <summary>
        /// Получатели
        /// </summary>
        public List<string> Recipients { get; set; } = [];

        /// <summary>
        /// Получатели копии
        /// </summary>
        public List<string>? CarbonCopy { get; set; }

        /// <summary>
        /// Тело письма
        /// </summary>
        public string? Body { get; set; }

        /// <summary>
        /// Тема письма
        /// </summary>
        public string? Subject { get; set; }

        /// <summary>
        /// Прикрепленные файлы
        /// </summary>
        [NotMapped]
        public List<FileStorageItem>? Attachments { get; set; }
    }

    public class EmailMessageHistory : BaseHistoryEntity<EmailMessage> { }
}
