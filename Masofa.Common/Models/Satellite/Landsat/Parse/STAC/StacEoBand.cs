using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Parse.Landsat.STAC
{
    /// <summary>
    /// Представляет спектральный канал в секции eo:bands ресурса в объекте STAC
    /// </summary>
    public class StacEoBand
    {
        /// <summary>
        /// Название канала (например, "B1", "B2" и т.д.)
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        /// <summary>
        /// Общепринятое название канала (например, "coastal", "blue", "green" и т.д.)
        /// </summary>
        [JsonPropertyName("common_name")]
        public string CommonName { get; set; } = default!;

        /// <summary>
        /// Пространственное разрешение в метрах
        /// </summary>
        [JsonPropertyName("gsd")]
        public int Gsd { get; set; }

        /// <summary>
        /// Центральная длина волны в микрометрах
        /// </summary>
        [JsonPropertyName("center_wavelength")]
        public double CenterWavelength { get; set; }
    }
}
