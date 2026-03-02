namespace Masofa.Common.Models.Dictionaries
{
	/// <summary>
	/// Основная сущность словарей
	/// </summary>
    public class BaseDictionaryItem : BaseEntity
    {
        /// <summary>
		/// Публикация
		/// </summary>
        public bool Visible { get; set; }

        /// <summary>
		/// Код сортировки
		/// </summary>
        public string? OrderCode { get; set; }

        /// <summary>
		/// Дополнительная информация
		/// </summary>
        public string? ExtData { get; set; }

        /// <summary>
		/// Комментарий
		/// </summary>
        public string? Comment { get; set; }
    }
}
