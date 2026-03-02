
using Masofa.Common.Enums;
using Masofa.Common.Models.Satellite.Parse.Landsat.STAC;
using Masofa.Common.Models.Satellite.Parse.Landsat.STAC.SR;
using Masofa.Common.Models.Satellite.Parse.Landsat.STAC.ST;

namespace Masofa.Common.Models.Satellite.Landsat
{
    /// <summary>
    /// Класс для преобразования моделей STAC в модели Entity Framework
    /// </summary>
    public static class StacEntityConverter
    {
        /// <summary>
        /// Преобразует объект StacFeature в StacFeatureEntity и связанные сущности
        /// </summary>
        /// <param name="stacFeature">Объект StacFeature</param>
        /// <returns>Кортеж, содержащий StacFeatureEntity и связанные сущности</returns>
        public static (StacFeatureEntity Feature, List<StacLinkEntity> Links, List<StacAssetEntity> Assets) ConvertToEntities(StacFeature stacFeature, ProductTypeEnum productType)
        {
            // Создаем основную сущность
            var featureEntity = StacFeatureEntity.FromStacFeature(stacFeature, productType);

            // Создаем сущности для ссылок
            var linkEntities = new List<StacLinkEntity>();
            foreach (var link in stacFeature.Links)
            {
                linkEntities.Add(StacLinkEntity.FromStacLink(link, Guid.Empty)); // ID будет установлен после сохранения featureEntity
            }

            // Создаем сущности для ассетов
            var assetEntities = new List<StacAssetEntity>();
            foreach (var assetKvp in stacFeature.Assets)
            {
                assetEntities.Add(StacAssetEntity.FromStacAsset(assetKvp.Key, assetKvp.Value, Guid.Empty)); // ID будет установлен после сохранения featureEntity
            }

            return (featureEntity, linkEntities, assetEntities);
        }

        /// <summary>
        /// Создает специализированные сущности для ассетов на основе типа продукта
        /// </summary>
        /// <param name="stacFeature">Объект StacFeature</param>
        /// <param name="assetEntities">Список сущностей ассетов</param>
        /// <param name="stacFeatureId">Идентификатор сущности StacFeature</param>
        /// <returns>Список специализированных сущностей ассетов</returns>
        public static List<SpecializedAssetEntity> CreateSpecializedAssetEntities(StacFeature stacFeature, List<StacAssetEntity> assetEntities, Guid stacFeatureId)
        {
            var result = new List<SpecializedAssetEntity>();

            // Обрабатываем ассеты в зависимости от типа объекта
            if (stacFeature is SrSurfaceReflectanceStacFeature srFeature)
            {
                // Обрабатываем ассеты SR
                result.AddRange(CreateSrSpecializedAssetEntities(srFeature, assetEntities, stacFeatureId));
            }
            else if (stacFeature is StSurfaceTemperatureStacFeature stFeature)
            {
                // Обрабатываем ассеты ST
                result.AddRange(CreateStSpecializedAssetEntities(stFeature, assetEntities, stacFeatureId));
            }

            return result;
        }

        /// <summary>
        /// Создает специализированные сущности для ассетов SR
        /// </summary>
        /// <param name="srFeature">Объект SrSurfaceReflectanceStacFeature</param>
        /// <param name="assetEntities">Список сущностей ассетов</param>
        /// <param name="stacFeatureId">Идентификатор сущности StacFeature</param>
        /// <returns>Список специализированных сущностей ассетов SR</returns>
        private static List<SpecializedAssetEntity> CreateSrSpecializedAssetEntities(SrSurfaceReflectanceStacFeature? srFeature, List<StacAssetEntity> assetEntities, Guid stacFeatureId)
        {
            if (srFeature == null)
                return new List<SpecializedAssetEntity>();

            var result = new List<SpecializedAssetEntity>();

            // Обрабатываем каналы и ассеты качества
            foreach (var assetEntity in assetEntities)
            {
                // Получаем соответствующий ассет из JSON
                if (srFeature.Assets.TryGetValue(assetEntity.AssetKey, out var asset))
                {
                    // Проверяем, является ли ассет каналом (имеет eo:bands)
                    if (asset.EoBands != null && asset.EoBands.Count > 0)
                    {
                        var band = asset.EoBands[0]; // Берем первый элемент массива
                        
                        // Извлекаем номер канала из имени (например, "B1" -> 1)
                        int bandNumber = 0;
                        if (band.Name != null && band.Name.StartsWith("B") && int.TryParse(band.Name.Substring(1), out int number))
                        {
                            bandNumber = number;
                        }
                        
                        result.Add(SrBandAssetEntity.Create(
                            assetEntity.Id,
                            stacFeatureId,
                            bandNumber,
                            band.CommonName ?? string.Empty,
                            band.CenterWavelength,
                            band.Gsd
                        ));
                    }
                    // Проверяем, является ли ассет ассетом качества
                    else if (assetEntity.AssetKey.ToLower().Contains("qa"))
                    {
                        // Пытаемся получить разрешение из JSON
                        int resolution = 0;
                        if (asset.EoBands?.Any() == true)
                        {
                            resolution = asset.EoBands[0].Gsd;
                        }
                        
                        result.Add(SrQualityAssetEntity.Create(
                            assetEntity.Id,
                            stacFeatureId,
                            assetEntity.AssetKey,
                            asset.ClassificationBitfields ?? new object(), // Используем битовые поля из JSON, если они есть
                            resolution
                        ));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Создает специализированные сущности для ассетов ST
        /// </summary>
        /// <param name="stFeature">Объект StSurfaceTemperatureStacFeature</param>
        /// <param name="assetEntities">Список сущностей ассетов</param>
        /// <param name="stacFeatureId">Идентификатор сущности StacFeature</param>
        /// <returns>Список специализированных сущностей ассетов ST</returns>
        private static List<SpecializedAssetEntity> CreateStSpecializedAssetEntities(StSurfaceTemperatureStacFeature? stFeature, List<StacAssetEntity> assetEntities, Guid stacFeatureId)
        {
            if (stFeature == null)
                return new List<SpecializedAssetEntity>();

            var result = new List<SpecializedAssetEntity>();

            // Обрабатываем ассеты
            foreach (var assetEntity in assetEntities)
            {
                // Получаем соответствующий ассет из JSON
                if (stFeature.Assets.TryGetValue(assetEntity.AssetKey, out var asset))
                {
                    // Проверяем, является ли ассет температурным каналом (lwir11)
                    if (assetEntity.AssetKey.ToLower() == "lwir11" && asset.EoBands != null && asset.EoBands.Count > 0)
                    {
                        var band = asset.EoBands[0];
                        result.Add(StTemperatureAssetEntity.Create(
                            assetEntity.Id,
                            stacFeatureId,
                            assetEntity.AssetKey,
                            band.CenterWavelength,
                            band.Gsd
                        ));
                    }
                    // Проверяем, является ли ассет вспомогательным (TRAD, URAD, и т.д.)
                    else if (new[] { "trad", "urad", "drad", "atran", "emis", "emsd", "cdist" }.Contains(assetEntity.AssetKey.ToLower()))
                    {
                        // Пытаемся получить разрешение из JSON
                        int resolution = 0;
                        if (asset.EoBands?.Any() == true)
                        {
                            resolution = asset.EoBands[0].Gsd;
                        }
                        
                        result.Add(StAuxiliaryAssetEntity.Create(
                            assetEntity.Id,
                            stacFeatureId,
                            assetEntity.AssetKey,
                            asset.Description ?? assetEntity.AssetKey,
                            resolution
                        ));
                    }
                    // Проверяем, является ли ассет ассетом качества
                    else if (assetEntity.AssetKey.ToLower().Contains("qa"))
                    {
                        // Пытаемся получить разрешение из JSON
                        int resolution = 0;
                        if (asset.EoBands?.Any() == true)
                        {
                            resolution = asset.EoBands[0].Gsd;
                        }
                        
                        result.Add(StQualityAssetEntity.Create(
                            assetEntity.Id,
                            stacFeatureId,
                            assetEntity.AssetKey,
                            asset.ClassificationBitfields ?? new object(), // Используем битовые поля из JSON, если они есть
                            resolution
                        ));
                    }
                }
            }

            return result;
        }

    }
}
