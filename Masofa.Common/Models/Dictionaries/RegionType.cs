using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Типы регионов
    /// </summary>
    /// <remarks>
    /// Справочник типов регионов
    /// </remarks>
    public class RegionType : NamedDictionaryItem
    {
        /// <summary>
        /// Регионы
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<Region> Regions { get; set; } = [];
    }

    public class RegionTypeHistory : BaseHistoryEntity<RegionType> { }
}
