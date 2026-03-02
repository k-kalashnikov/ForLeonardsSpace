using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Parse.Landsat.STAC.ST
{
    /// <summary>
    /// Представляет объект STAC для данных температуры поверхности Landsat (ST)
    /// </summary>
    public class StSurfaceTemperatureStacFeature : StacFeature
    {
       
        /// <summary>
        /// Получает ресурс lwir11 (B10), который содержит данные о температуре поверхности
        /// </summary>
        [JsonIgnore]
        public StacAsset? Lwir11Asset => Assets.ContainsKey("lwir11") ? Assets["lwir11"] : null;

        /// <summary>
        /// Получает ресурс TRAD, который содержит данные о тепловом излучении
        /// </summary>
        [JsonIgnore]
        public StacAsset? TradAsset => Assets.ContainsKey("TRAD") ? Assets["TRAD"] : null;

        /// <summary>
        /// Получает ресурс URAD, который содержит данные о восходящем излучении
        /// </summary>
        [JsonIgnore]
        public StacAsset? UradAsset => Assets.ContainsKey("URAD") ? Assets["URAD"] : null;

        /// <summary>
        /// Получает ресурс DRAD, который содержит данные о нисходящем излучении
        /// </summary>
        [JsonIgnore]
        public StacAsset? DradAsset => Assets.ContainsKey("DRAD") ? Assets["DRAD"] : null;

        /// <summary>
        /// Получает ресурс ATRAN, который содержит данные о пропускании атмосферы
        /// </summary>
        [JsonIgnore]
        public StacAsset? AtranAsset => Assets.ContainsKey("ATRAN") ? Assets["ATRAN"] : null;

        /// <summary>
        /// Получает ресурс EMIS, который содержит данные об излучательной способности
        /// </summary>
        [JsonIgnore]
        public StacAsset? EmisAsset => Assets.ContainsKey("EMIS") ? Assets["EMIS"] : null;

        /// <summary>
        /// Получает ресурс EMSD, который содержит данные о стандартном отклонении излучательной способности
        /// </summary>
        [JsonIgnore]
        public StacAsset? EmsdAsset => Assets.ContainsKey("EMSD") ? Assets["EMSD"] : null;

        /// <summary>
        /// Получает ресурс CDIST, который содержит данные о расстоянии до облаков
        /// </summary>
        [JsonIgnore]
        public StacAsset? CdistAsset => Assets.ContainsKey("CDIST") ? Assets["CDIST"] : null;

        /// <summary>
        /// Получает ресурс qa, который содержит данные оценки качества температуры поверхности
        /// </summary>
        [JsonIgnore]
        public StacAsset? QaAsset => Assets.ContainsKey("qa") ? Assets["qa"] : null;

        /// <summary>
        /// Получает ресурс qa_pixel, который содержит данные оценки качества пикселей
        /// </summary>
        [JsonIgnore]
        public StacAsset? QaPixelAsset => Assets.ContainsKey("qa_pixel") ? Assets["qa_pixel"] : null;

        /// <summary>
        /// Получает ресурс qa_radsat, который содержит данные оценки качества радиометрического насыщения
        /// </summary>
        [JsonIgnore]
        public StacAsset? QaRadsatAsset => Assets.ContainsKey("qa_radsat") ? Assets["qa_radsat"] : null;
    }
}
