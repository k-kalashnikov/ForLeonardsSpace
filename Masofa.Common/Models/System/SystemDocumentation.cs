using Masofa.Common.Models;
using Masofa.Common.Models.Dictionaries;

namespace Masofa.Common.Models.SystemDocumentation
{
    /// <summary>
    /// Системная документация
    /// </summary>
    public class SystemDocumentation : NamedDictionaryItem
    {
        /// <summary>
        /// Локализованные идентификаторы файлов в хранилище (по языкам)
        /// </summary>
        public LocalizationFileStorageItem FileStorageIds { get; set; }

        /// <summary>
        /// Идентификатор блока документации
        /// </summary>
        public Guid? BlockId { get; set; }
    }

    /// <summary>
    /// История изменений системной документации
    /// </summary>
    public class SystemDocumentationHistory : BaseHistoryEntity<SystemDocumentation> { }
}

