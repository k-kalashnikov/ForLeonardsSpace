using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Вегетационные индексы
    /// </summary>
    /// <remarks>
    /// Справочник вегетационных индексов
    /// </remarks>
    public class VegetationIndex : BaseDictionaryItem
    {
        /// <summary>
        /// Название индекса
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public LocalizationString Descriptions { get; set; } = new LocalizationString();

        /// <summary>
        /// Индексы периодов развития
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [SwaggerIgnore]
        public ICollection<CropPeriodVegetationIndex> CropPeriodVegetationIndexes { get; set; } = [];
    }

    public class VegetationIndexHistory : BaseHistoryEntity<VegetationIndex> { }
}
