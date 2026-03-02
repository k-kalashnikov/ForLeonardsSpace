using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Виды деятельности юридических и физических лиц
    /// </summary>
    /// <remarks>
    /// Справочник видов деятельности юридических и физических лиц
    /// </remarks>
    public class BusinessType : NamedDictionaryItem
    {
        /// <summary>
        /// Физические лица
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<Person> Persons { get; set; } = [];

        /// <summary>
        /// Юридические лица
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<Firm> Firms { get; set; } = [];
    }

    public class BusinessTypeHistory : BaseHistoryEntity<BusinessType> { }
}
