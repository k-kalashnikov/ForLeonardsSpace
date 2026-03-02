using Masofa.Common.Models;

namespace Masofa.Common.Models.SystemDocumentation
{
    /// <summary>
    /// Блок системной документации для организации структуры
    /// </summary>
    public class SystemDocumentationBlock : BaseNamedEntity
    {
        /// <summary>
        /// Идентификатор родительского блока (для иерархической структуры)
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Порядок сортировки
        /// </summary>
        public int? OrderCode { get; set; }
    }
}

