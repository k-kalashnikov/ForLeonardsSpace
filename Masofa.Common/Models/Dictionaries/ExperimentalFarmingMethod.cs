using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Виды эксперем-го земледелия_Д
    /// </summary>
    /// <remarks>
    /// Справочник видов эксперементальных способов земледелия
    /// </remarks>
    public class ExperimentalFarmingMethod : BaseDictionaryItem
    {
        /// <summary>
        /// Наименование
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Идентификатор культуры
        /// </summary>
        public Guid? CropId { get; set; }

        /// <summary>
        /// Идентификатор культуры
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public Crop? Crop { get; set; }
    }

    public class ExperimentalFarmingMethodHistory : BaseHistoryEntity<ExperimentalFarmingMethod> { }
}
