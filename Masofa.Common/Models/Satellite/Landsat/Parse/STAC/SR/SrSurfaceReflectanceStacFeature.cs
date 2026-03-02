using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Parse.Landsat.STAC.SR
{
    /// <summary>
    /// Представляет объект STAC для данных отражательной способности поверхности Landsat (SR)
    /// </summary>
    public class SrSurfaceReflectanceStacFeature : StacFeature
    {

        /// <summary>
        /// Получает ресурс прибрежного/аэрозольного канала (B1)
        /// </summary>
        [JsonIgnore]
        public StacAsset? CoastalAsset => Assets.ContainsKey("coastal") ? Assets["coastal"] : null;

        /// <summary>
        /// Получает ресурс синего канала (B2)
        /// </summary>
        [JsonIgnore]
        public StacAsset? BlueAsset => Assets.ContainsKey("blue") ? Assets["blue"] : null;

        /// <summary>
        /// Получает ресурс зеленого канала (B3)
        /// </summary>
        [JsonIgnore]
        public StacAsset? GreenAsset => Assets.ContainsKey("green") ? Assets["green"] : null;

        /// <summary>
        /// Получает ресурс красного канала (B4)
        /// </summary>
        [JsonIgnore]
        public StacAsset? RedAsset => Assets.ContainsKey("red") ? Assets["red"] : null;

        /// <summary>
        /// Получает ресурс ближнего инфракрасного канала 0.8 (B5)
        /// </summary>
        [JsonIgnore]
        public StacAsset? Nir08Asset => Assets.ContainsKey("nir08") ? Assets["nir08"] : null;

        /// <summary>
        /// Получает ресурс коротковолнового инфракрасного канала 1.6 (B6)
        /// </summary>
        [JsonIgnore]
        public StacAsset? Swir16Asset => Assets.ContainsKey("swir16") ? Assets["swir16"] : null;

        /// <summary>
        /// Получает ресурс коротковолнового инфракрасного канала 2.2 (B7)
        /// </summary>
        [JsonIgnore]
        public StacAsset? Swir22Asset => Assets.ContainsKey("swir22") ? Assets["swir22"] : null;

        /// <summary>
        /// Получает ресурс канала анализа качества аэрозолей
        /// </summary>
        [JsonIgnore]
        public StacAsset? QaAerosolAsset => Assets.ContainsKey("qa_aerosol") ? Assets["qa_aerosol"] : null;

        /// <summary>
        /// Получает ресурс канала оценки качества пикселей
        /// </summary>
        [JsonIgnore]
        public StacAsset? QaPixelAsset => Assets.ContainsKey("qa_pixel") ? Assets["qa_pixel"] : null;

        /// <summary>
        /// Получает ресурс канала оценки качества радиометрического насыщения
        /// </summary>
        [JsonIgnore]
        public StacAsset? QaRadsatAsset => Assets.ContainsKey("qa_radsat") ? Assets["qa_radsat"] : null;
    }
}
