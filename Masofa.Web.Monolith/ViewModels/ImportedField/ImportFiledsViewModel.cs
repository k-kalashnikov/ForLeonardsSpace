namespace Masofa.Web.Monolith.ViewModels.ImportedField
{
    /// <summary>
    /// Запрос на импорт полей
    /// </summary>
    public class ImportFiledsViewModel
    {
        /// <summary>
        /// Комментарий
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// Импортируемые файлы
        /// </summary>
        public List<IFormFile> Files { get; set; } = [];
    }
}