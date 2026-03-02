using Masofa.Common.Helper;
using Masofa.Common.Models.Dictionaries;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.SystemCrical
{
    /// <summary>
    /// Тег
    /// </summary>
    /// <remarks>
    /// Тег может быть связан с любым объектом системы, имеющим идентификатор типа Guid
    /// </remarks>
    public class Tag : BaseDictionaryItem
    {
        /// <summary>
        /// Отображаемое в UI имя тега
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string? Description { get; set; }

        [NotMapped]
        public int RelationCount { get; set; } = 0;
    }

    public class TagRelation : BaseEntity
    {
        /// <summary>
        /// Тип владельца
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [NotMapped]
        public Type OwnerType
        {
            get
            {
                return TypeHelper.GetTypeFromAllAssemblies(OwnerTypeFullName);
            }
        }

        /// <summary>
        /// Владелец
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Тип владельца
        /// </summary>
        public string OwnerTypeFullName { get; set; }

        public Guid TagId { get; set; }
    }

    public class TagRelationHistory : BaseHistoryEntity<TagRelation> { }
}
