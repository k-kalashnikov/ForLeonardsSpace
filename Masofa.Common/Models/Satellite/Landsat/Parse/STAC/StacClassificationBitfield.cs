using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Parse.Landsat.STAC
{
    /// <summary>
    /// Представляет битовое поле в секции classification:bitfields ресурса в объекте STAC
    /// </summary>
    public class StacClassificationBitfield
    {
        /// <summary>
        /// Название битового поля
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        /// <summary>
        /// Описание битового поля
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = default!;

        /// <summary>
        /// Битовое смещение поля
        /// </summary>
        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        /// <summary>
        /// Битовая длина поля
        /// </summary>
        [JsonPropertyName("length")]
        public int Length { get; set; }

        /// <summary>
        /// Классы (возможные значения) битового поля
        /// </summary>
        [JsonPropertyName("classes")]
        public List<StacClassificationBitfieldClass> Classes { get; set; } = default!;
    }

    /// <summary>
    /// Представляет класс (возможное значение) битового поля в секции classification:bitfields ресурса в объекте STAC
    /// </summary>
    public class StacClassificationBitfieldClass : BaseEntity
    {
        /// <summary>
        /// Название класса
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        /// <summary>
        /// Описание класса
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = default!;

        /// <summary>
        /// Значение класса
        /// </summary>
        [JsonPropertyName("value")]
        public int Value { get; set; }
    }
}
