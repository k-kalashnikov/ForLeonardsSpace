using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Виды пестицидов и агрохимикатов
    /// </summary>
    /// <remarks>
    /// Справочник видов пестицидов и агрохимикатов
    /// </remarks>
    public class PesticideType : NamedDictionaryItem
    {
        /// <summary>
        /// Международный код
        /// </summary>
        public string? IntCode { get; set; }

        /// <summary>
        /// Пестициды и агрохимикаты
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<Pesticide> Pesticides { get; set; } = [];
    }

    public class PesticideTypeHistory : BaseHistoryEntity<PesticideType> { }
}
