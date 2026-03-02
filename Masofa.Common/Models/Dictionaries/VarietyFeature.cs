using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Дополнительные характеристики сорта
    /// </summary>
    /// /// /// <remarks>
    /// Справочник дополнительных характеристик сорта
    /// </remarks>
    public class VarietyFeature : NamedDictionaryItem
    {
        /// <summary>
        /// Сорта, имеющие эту характеристику
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<Variety> Varieties { get; set; } = [];
    }

    public class VarietyFeatureHistory : BaseHistoryEntity<VarietyFeature> { }
}
