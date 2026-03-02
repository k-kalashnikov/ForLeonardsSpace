using Masofa.Common.Models;

namespace Masofa.Common.Models.SystemMetadata
{
    /// <summary>
    /// Информация о модуле с локализованными названиями
    /// </summary>
    public class LocalizedModuleInfo
    {
        /// <summary>
        /// Оригинальное название модуля (ключ)
        /// </summary>
        public string ModuleName { get; set; } = string.Empty;

        /// <summary>
        /// Локализованные названия модуля для всех поддерживаемых языков
        /// </summary>
        public LocalizationString LocalizedNames { get; set; } = new LocalizationString();
    }
}

