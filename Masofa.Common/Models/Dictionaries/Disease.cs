using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Болезни растений
    /// </summary>
    /// <remarks>
    /// Справочник болезней растений
    /// </remarks>
    public class Disease : NamedDictionaryItem
    {
        /// <summary>
        /// Идентификатор культуры
        /// </summary>
        public Guid? CropId { get; set; }

        /// <summary>
        /// Идентификатор сорта
        /// </summary>
        public Guid? VarietyId { get; set; }

        /// <summary>
        /// Наименование на латыни
        /// </summary>
        public string? NameLa { get; set; }

        /// <summary>
        /// Идентификатор культуры
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public Crop? Crop { get; set; }

        /// <summary>
        /// Идентификатор сорта
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public Variety? Variety { get; set; }
    }

    public class DiseaseHistory : BaseHistoryEntity<Disease> { }
}
