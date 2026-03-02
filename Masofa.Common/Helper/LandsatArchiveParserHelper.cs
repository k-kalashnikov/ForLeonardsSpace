using Masofa.Common.Models.Satellite.Landsat;
using Masofa.Common.Models.Satellite.Parse.Landsat.MTL;
using Masofa.Common.Models.Satellite.Parse.Landsat.STAC;
using Masofa.Common.Models.Satellite.Parse.Landsat.STAC.SR;
using Masofa.Common.Models.Satellite.Parse.Landsat.STAC.ST;
using System.Text.Json;

namespace Masofa.Common.Helper
{
    // DTO модели для возврата результатов парсинга
    public class LandsatParseResult
    {
        public LandsatSourceMetadataEntity SourceMetadata { get; set; }
        public ProductContentsEntity? ProductContents { get; set; }
        public ImageAttributesEntity? ImageAttributes { get; set; }
        public ProjectionAttributesEntity? ProjectionAttributes { get; set; }
        public Level2ProcessingRecordEntity? Level2ProcessingRecord { get; set; }
        public Level2SurfaceReflectanceParametersEntity? Level2SurfaceReflectanceParameters { get; set; }
        public Level2SurfaceTemperatureParametersEntity? Level2SurfaceTemperatureParameters { get; set; }
        public Level1ProcessingRecordEntity? Level1ProcessingRecord { get; set; }
        public Level1MinMaxRadianceEntity? Level1MinMaxRadiance { get; set; }
        public Level1MinMaxReflectanceEntity? Level1MinMaxReflectance { get; set; }
        public Level1MinMaxPixelValueEntity? Level1MinMaxPixelValue { get; set; }
        public Level1RadiometricRescalingEntity? Level1RadiometricRescaling { get; set; }
        public Level1ThermalConstantsEntity? Level1ThermalConstants { get; set; }
        public Level1ProjectionParametersEntity? Level1ProjectionParameters { get; set; }
        public DateTime SatelliteDate { get; set; }
    }

    public class LandsatStacParseResult
    {
        public StacFeatureEntity Feature { get; set; }
        public List<StacLinkEntity> Links { get; set; }
        public List<StacAssetEntity> Assets { get; set; }
        public DateTime SatelliteDate { get; set; }
    }

    public class LandsatArchiveParserHelper
    {

        public async Task<LandsatParseResult> ParseMtlFileAsync(Stream fileStream)
        {
            // 1. Стримим JSON и парсим в LandsatSourceMetadataRaw
            using var reader = new StreamReader(fileStream);
            var jsonContent = await reader.ReadToEndAsync();
            if (string.IsNullOrWhiteSpace(jsonContent))
                throw new ArgumentException("JSON-содержимое не может быть пустым");

            var mtlRaw = DeserializeMtlJson(jsonContent);
            if (mtlRaw == null)
                throw new InvalidOperationException("Не удалось десериализовать JSON в модель MTL");

            // 2. Извлекаем дату спутника
            var satelliteDate = ExtractSatelliteDate(mtlRaw);

            // 3. Парсим все секции MTL в соответствующие сущности
            var productContentsEntity = CreateProductContents(mtlRaw, satelliteDate);
            var imageAttributesEntity = CreateImageAttributes(mtlRaw, satelliteDate);
            var projectionAttributesEntity = CreateProjectionAttributes(mtlRaw, satelliteDate);
            var level2ProcessingRecordEntity = CreateLevel2ProcessingRecord(mtlRaw, satelliteDate);
            var level2SurfaceReflectanceParametersEntity = CreateLevel2SurfaceReflectanceParameters(mtlRaw, satelliteDate);
            var level2SurfaceTemperatureParametersEntity = CreateLevel2SurfaceTemperatureParameters(mtlRaw, satelliteDate);
            var level1ProcessingRecordEntity = CreateLevel1ProcessingRecord(mtlRaw, satelliteDate);
            var level1MinMaxRadianceEntity = CreateLevel1MinMaxRadiance(mtlRaw, satelliteDate);
            var level1MinMaxReflectanceEntity = CreateLevel1MinMaxReflectance(mtlRaw, satelliteDate);
            var level1MinMaxPixelValueEntity = CreateLevel1MinMaxPixelValue(mtlRaw, satelliteDate);
            var level1RadiometricRescalingEntity = CreateLevel1RadiometricRescaling(mtlRaw, satelliteDate);
            var level1ThermalConstantsEntity = CreateLevel1ThermalConstants(mtlRaw, satelliteDate);
            var level1ProjectionParametersEntity = CreateLevel1ProjectionParameters(mtlRaw, satelliteDate);

            // 4. Создаем и заполняем LandsatSourceMetadataEntity
            var sourceMetadataEntity = new LandsatSourceMetadataEntity
            {
                SatellateProductId = mtlRaw.SatellateProductId,
                ProductContentsId = null, // Будет установлено при сохранении
                ImageAttributesId = null, // Будет установлено при сохранении
                ProjectionAttributesId = null, // Будет установлено при сохранении
                Level2ProcessingRecordId = null, // Будет установлено при сохранении
                Level2SurfaceReflectanceParametersId = null, // Будет установлено при сохранении
                Level2SurfaceTemperatureParametersId = null, // Будет установлено при сохранении
                Level1ProcessingRecordId = null, // Будет установлено при сохранении
                Level1MinMaxRadianceId = null, // Будет установлено при сохранении
                Level1MinMaxReflectanceId = null, // Будет установлено при сохранении
                Level1MinMaxPixelValueId = null, // Будет установлено при сохранении
                Level1RadiometricRescalingId = null, // Будет установлено при сохранении
                Level1ThermalConstantsId = null, // Будет установлено при сохранении
                Level1ProjectionParametersId = null, // Будет установлено при сохранении
                CreateAt = satelliteDate
            };

            // 5. Возвращаем результат парсинга
            return new LandsatParseResult
            {
                SourceMetadata = sourceMetadataEntity,
                ProductContents = productContentsEntity,
                ImageAttributes = imageAttributesEntity,
                ProjectionAttributes = projectionAttributesEntity,
                Level2ProcessingRecord = level2ProcessingRecordEntity,
                Level2SurfaceReflectanceParameters = level2SurfaceReflectanceParametersEntity,
                Level2SurfaceTemperatureParameters = level2SurfaceTemperatureParametersEntity,
                Level1ProcessingRecord = level1ProcessingRecordEntity,
                Level1MinMaxRadiance = level1MinMaxRadianceEntity,
                Level1MinMaxReflectance = level1MinMaxReflectanceEntity,
                Level1MinMaxPixelValue = level1MinMaxPixelValueEntity,
                Level1RadiometricRescaling = level1RadiometricRescalingEntity,
                Level1ThermalConstants = level1ThermalConstantsEntity,
                Level1ProjectionParameters = level1ProjectionParametersEntity,
                SatelliteDate = satelliteDate
            };
        }

        public async Task<LandsatStacParseResult> ParseSrStacFileAsync(Stream fileStream)
        {
            using var reader = new StreamReader(fileStream);
            var jsonContent = await reader.ReadToEndAsync();
            if (string.IsNullOrWhiteSpace(jsonContent))
                throw new ArgumentException("JSON-содержимое не может быть пустым");
            var stacFeature = DeserializeSrJson(jsonContent);
            if (stacFeature == null)
                throw new InvalidOperationException("Не удалось десериализовать JSON в модель STAC");

            // Извлекаем дату спутника
            var satelliteDate = ExtractSatelliteDateFromStac(stacFeature);

            var (featureEntity, linkEntities, assetEntities) = StacEntityConverter.ConvertToEntities(stacFeature, ProductTypeEnum.SR);

            // Устанавливаем дату спутника для всех сущностей
            featureEntity.CreateAt = satelliteDate;
            foreach (var linkEntity in linkEntities)
            {
                linkEntity.CreateAt = satelliteDate;
            }
            foreach (var assetEntity in assetEntities)
            {
                assetEntity.CreateAt = satelliteDate;
            }

            return new LandsatStacParseResult
            {
                Feature = featureEntity,
                Links = linkEntities,
                Assets = assetEntities,
                SatelliteDate = satelliteDate
            };
        }

        public async Task<LandsatStacParseResult> ParseStStacFileAsync(Stream fileStream)
        {
            using var reader = new StreamReader(fileStream);
            var jsonContent = await reader.ReadToEndAsync();
            if (string.IsNullOrWhiteSpace(jsonContent))
                throw new ArgumentException("JSON-содержимое не может быть пустым");
            var stacFeature = DeserializeStJson(jsonContent);
            if (stacFeature == null)
                throw new InvalidOperationException("Не удалось десериализовать JSON в модель STAC");

            // Извлекаем дату спутника
            var satelliteDate = ExtractSatelliteDateFromStac(stacFeature);

            var (featureEntity, linkEntities, assetEntities) = StacEntityConverter.ConvertToEntities(stacFeature, ProductTypeEnum.ST);

            // Устанавливаем дату спутника для всех сущностей
            featureEntity.CreateAt = satelliteDate;
            foreach (var linkEntity in linkEntities)
            {
                linkEntity.CreateAt = satelliteDate;
            }
            foreach (var assetEntity in assetEntities)
            {
                assetEntity.CreateAt = satelliteDate;
            }

            return new LandsatStacParseResult
            {
                Feature = featureEntity,
                Links = linkEntities,
                Assets = assetEntities,
                SatelliteDate = satelliteDate
            };
        }

        private LandsatSourceMetadataRaw DeserializeMtlJson(string jsonContent)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<LandsatSourceMetadataRaw>(jsonContent);
        }

        private SrSurfaceReflectanceStacFeature DeserializeSrJson(string jsonContent)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<SrSurfaceReflectanceStacFeature>(jsonContent);
        }

        private StSurfaceTemperatureStacFeature DeserializeStJson(string jsonContent)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<StSurfaceTemperatureStacFeature>(jsonContent);
        }

        /// <summary>
        /// Извлекает дату спутника из MTL данных
        /// </summary>
        /// <param name="mtlData">MTL данные</param>
        /// <returns>Дата спутника или текущее время, если дата недоступна</returns>
        private DateTime ExtractSatelliteDate(LandsatSourceMetadataRaw mtlRaw)
        {
            // Проверяем, есть ли данные ImageAttributes в mtlData
            if (mtlRaw.LandsatMetadataFile?.ImageAttributes == null)
                return DateTime.UtcNow;

            if (!string.IsNullOrEmpty(mtlRaw.LandsatMetadataFile.ImageAttributes.DateAcquired))
            {
                if (DateTime.TryParse(mtlRaw.LandsatMetadataFile.ImageAttributes.DateAcquired, out var dateAcquired))
                {
                    // Убеждаемся, что DateTime имеет Kind.Utc для PostgreSQL
                    return DateTime.SpecifyKind(dateAcquired, DateTimeKind.Utc);
                }
            }

            // Пытаемся парсить SceneCenterTime
            if (!string.IsNullOrEmpty(mtlRaw.LandsatMetadataFile.ImageAttributes.SceneCenterTime))
            {
                if (DateTime.TryParse(mtlRaw.LandsatMetadataFile.ImageAttributes.SceneCenterTime, out var sceneCenterTime))
                {
                    // Убеждаемся, что DateTime имеет Kind.Utc для PostgreSQL
                    return DateTime.SpecifyKind(sceneCenterTime, DateTimeKind.Utc);
                }
            }

            // Fallback: возвращаем текущее время
            return DateTime.UtcNow;
        }

        /// <summary>
        /// Извлекает дату спутника из STAC данных
        /// </summary>
        /// <param name="stacFeature">STAC данные</param>
        /// <returns>Дата спутника или текущее время, если дата недоступна</returns>
        private DateTime ExtractSatelliteDateFromStac(StacFeature stacFeature)
        {
            // Пытаемся извлечь дату из свойств STAC
            if (stacFeature?.Properties?.Datetime != default)
            {
                // Убеждаемся, что DateTime имеет Kind.Utc для PostgreSQL
                return DateTime.SpecifyKind(stacFeature.Properties.Datetime, DateTimeKind.Utc);
            }

            // Fallback: возвращаем текущее время
            return DateTime.UtcNow;
        }



        private ProductContentsEntity? CreateProductContents(LandsatSourceMetadataRaw mtlRaw, DateTime satelliteDate)
        {
            if (mtlRaw.ProductContentsId.HasValue)
                return null;

            // Проверяем, есть ли данные ProductContents в mtlData
            if (mtlRaw.LandsatMetadataFile?.ProductContents == null)
                return null;

            var entity = new ProductContentsEntity
            {
                Origin = mtlRaw.LandsatMetadataFile.ProductContents.Origin,
                DigitalObjectIdentifier = mtlRaw.LandsatMetadataFile.ProductContents.DigitalObjectIdentifier,
                LandsatProductId = mtlRaw.LandsatMetadataFile.ProductContents.LandsatProductId,
                ProcessingLevel = mtlRaw.LandsatMetadataFile.ProductContents.ProcessingLevel,
                CollectionNumber = mtlRaw.LandsatMetadataFile.ProductContents.CollectionNumber,
                CollectionCategory = mtlRaw.LandsatMetadataFile.ProductContents.CollectionCategory,
                OutputFormat = mtlRaw.LandsatMetadataFile.ProductContents.OutputFormat,
                FileNameBand1 = mtlRaw.LandsatMetadataFile.ProductContents.FileNameBand1,
                FileNameBand2 = mtlRaw.LandsatMetadataFile.ProductContents.FileNameBand2,
                FileNameBand3 = mtlRaw.LandsatMetadataFile.ProductContents.FileNameBand3,
                FileNameBand4 = mtlRaw.LandsatMetadataFile.ProductContents.FileNameBand4,
                FileNameBand5 = mtlRaw.LandsatMetadataFile.ProductContents.FileNameBand5,
                FileNameBand6 = mtlRaw.LandsatMetadataFile.ProductContents.FileNameBand6,
                FileNameBand7 = mtlRaw.LandsatMetadataFile.ProductContents.FileNameBand7,
                FileNameBandStB10 = mtlRaw.LandsatMetadataFile.ProductContents.FileNameBandStB10,
                FileNameThermalRadiance = mtlRaw.LandsatMetadataFile.ProductContents.FileNameThermalRadiance,
                FileNameUpwellRadiance = mtlRaw.LandsatMetadataFile.ProductContents.FileNameUpwellRadiance,
                FileNameDownwellRadiance = mtlRaw.LandsatMetadataFile.ProductContents.FileNameDownwellRadiance,
                FileNameAtmosphericTransmittance = mtlRaw.LandsatMetadataFile.ProductContents.FileNameAtmosphericTransmittance,
                FileNameEmissivity = mtlRaw.LandsatMetadataFile.ProductContents.FileNameEmissivity,
                FileNameEmissivityStdev = mtlRaw.LandsatMetadataFile.ProductContents.FileNameEmissivityStdev,
                FileNameCloudDistance = mtlRaw.LandsatMetadataFile.ProductContents.FileNameCloudDistance,
                FileNameQualityL2Aerosol = mtlRaw.LandsatMetadataFile.ProductContents.FileNameQualityL2Aerosol,
                FileNameQualityL2SurfaceTemperature = mtlRaw.LandsatMetadataFile.ProductContents.FileNameQualityL2SurfaceTemperature,
                FileNameQualityL1Pixel = mtlRaw.LandsatMetadataFile.ProductContents.FileNameQualityL1Pixel,
                FileNameQualityL1RadiometricSaturation = mtlRaw.LandsatMetadataFile.ProductContents.FileNameQualityL1RadiometricSaturation,
                FileNameAngleCoefficient = mtlRaw.LandsatMetadataFile.ProductContents.FileNameAngleCoefficient,
                FileNameMetadataOdl = mtlRaw.LandsatMetadataFile.ProductContents.FileNameMetadataOdl,
                FileNameMetadataXml = mtlRaw.LandsatMetadataFile.ProductContents.FileNameMetadataXml,
                DataTypeBand1 = mtlRaw.LandsatMetadataFile.ProductContents.DataTypeBand1,
                DataTypeBand2 = mtlRaw.LandsatMetadataFile.ProductContents.DataTypeBand2,
                DataTypeBand3 = mtlRaw.LandsatMetadataFile.ProductContents.DataTypeBand3,
                DataTypeBand4 = mtlRaw.LandsatMetadataFile.ProductContents.DataTypeBand4,
                DataTypeBand5 = mtlRaw.LandsatMetadataFile.ProductContents.DataTypeBand5,
                DataTypeBand6 = mtlRaw.LandsatMetadataFile.ProductContents.DataTypeBand6,
                DataTypeBand7 = mtlRaw.LandsatMetadataFile.ProductContents.DataTypeBand7,
                DataTypeBandStB10 = mtlRaw.LandsatMetadataFile.ProductContents.DataTypeBandStB10,
                DataTypeThermalRadiance = mtlRaw.LandsatMetadataFile.ProductContents.DataTypeThermalRadiance,
                DataTypeUpwellRadiance = mtlRaw.LandsatMetadataFile.ProductContents.DataTypeUpwellRadiance,
                DataTypeDownwellRadiance = mtlRaw.LandsatMetadataFile.ProductContents.DataTypeDownwellRadiance,
                DataTypeAtmosphericTransmittance = mtlRaw.LandsatMetadataFile.ProductContents.DataTypeAtmosphericTransmittance,
                DataTypeEmissivity = mtlRaw.LandsatMetadataFile.ProductContents.DataTypeEmissivity,
                DataTypeEmissivityStdev = mtlRaw.LandsatMetadataFile.ProductContents.DataTypeEmissivityStdev,
                DataTypeCloudDistance = mtlRaw.LandsatMetadataFile.ProductContents.DataTypeCloudDistance,
                DataTypeQualityL2Aerosol = mtlRaw.LandsatMetadataFile.ProductContents.DataTypeQualityL2Aerosol,
                DataTypeQualityL2SurfaceTemperature = mtlRaw.LandsatMetadataFile.ProductContents.DataTypeQualityL2SurfaceTemperature,
                DataTypeQualityL1Pixel = mtlRaw.LandsatMetadataFile.ProductContents.DataTypeQualityL1Pixel,
                DataTypeQualityL1RadiometricSaturation = mtlRaw.LandsatMetadataFile.ProductContents.DataTypeQualityL1RadiometricSaturation,
                CreateAt = satelliteDate
            };
            return entity;
        }

        private ImageAttributesEntity? CreateImageAttributes(LandsatSourceMetadataRaw mtlRaw, DateTime satelliteDate)
        {
            if (mtlRaw.ImageAttributesId.HasValue)
                return null;

            // Проверяем, есть ли данные ImageAttributes в mtlData
            if (mtlRaw.LandsatMetadataFile?.ImageAttributes == null)
                return null;

            var entity = new ImageAttributesEntity
            {
                SpacecraftId = mtlRaw.LandsatMetadataFile.ImageAttributes.SpacecraftId,
                SensorId = mtlRaw.LandsatMetadataFile.ImageAttributes.SensorId,
                WrsType = mtlRaw.LandsatMetadataFile.ImageAttributes.WrsType,
                WrsPath = mtlRaw.LandsatMetadataFile.ImageAttributes.WrsPath,
                WrsRow = mtlRaw.LandsatMetadataFile.ImageAttributes.WrsRow,
                NadirOffnadir = mtlRaw.LandsatMetadataFile.ImageAttributes.NadirOffnadir,
                TargetWrsPath = mtlRaw.LandsatMetadataFile.ImageAttributes.TargetWrsPath,
                TargetWrsRow = mtlRaw.LandsatMetadataFile.ImageAttributes.TargetWrsRow,
                DateAcquired = mtlRaw.LandsatMetadataFile.ImageAttributes.DateAcquired,
                SceneCenterTime = mtlRaw.LandsatMetadataFile.ImageAttributes.SceneCenterTime,
                StationId = mtlRaw.LandsatMetadataFile.ImageAttributes.StationId,
                CloudCover = mtlRaw.LandsatMetadataFile.ImageAttributes.CloudCover,
                CloudCoverLand = mtlRaw.LandsatMetadataFile.ImageAttributes.CloudCoverLand,
                ImageQualityOli = mtlRaw.LandsatMetadataFile.ImageAttributes.ImageQualityOli,
                ImageQualityTirs = mtlRaw.LandsatMetadataFile.ImageAttributes.ImageQualityTirs,
                SaturationBand1 = mtlRaw.LandsatMetadataFile.ImageAttributes.SaturationBand1,
                SaturationBand2 = mtlRaw.LandsatMetadataFile.ImageAttributes.SaturationBand2,
                SaturationBand3 = mtlRaw.LandsatMetadataFile.ImageAttributes.SaturationBand3,
                SaturationBand4 = mtlRaw.LandsatMetadataFile.ImageAttributes.SaturationBand4,
                SaturationBand5 = mtlRaw.LandsatMetadataFile.ImageAttributes.SaturationBand5,
                SaturationBand6 = mtlRaw.LandsatMetadataFile.ImageAttributes.SaturationBand6,
                SaturationBand7 = mtlRaw.LandsatMetadataFile.ImageAttributes.SaturationBand7,
                SaturationBand8 = mtlRaw.LandsatMetadataFile.ImageAttributes.SaturationBand8,
                SaturationBand9 = mtlRaw.LandsatMetadataFile.ImageAttributes.SaturationBand9,
                RollAngle = mtlRaw.LandsatMetadataFile.ImageAttributes.RollAngle,
                SunAzimuth = mtlRaw.LandsatMetadataFile.ImageAttributes.SunAzimuth,
                SunElevation = mtlRaw.LandsatMetadataFile.ImageAttributes.SunElevation,
                EarthSunDistance = mtlRaw.LandsatMetadataFile.ImageAttributes.EarthSunDistance,
                CreateAt = satelliteDate
            };
            return entity;
        }

        private ProjectionAttributesEntity? CreateProjectionAttributes(LandsatSourceMetadataRaw mtlRaw, DateTime satelliteDate)
        {
            if (mtlRaw.ProjectionAttributesId.HasValue)
                return null;

            // Проверяем, есть ли данные ProjectionAttributes в mtlData
            if (mtlRaw.LandsatMetadataFile?.ProjectionAttributes == null)
                return null;

            var entity = new ProjectionAttributesEntity
            {
                MapProjection = mtlRaw.LandsatMetadataFile.ProjectionAttributes.MapProjection,
                Datum = mtlRaw.LandsatMetadataFile.ProjectionAttributes.Datum,
                Ellipsoid = mtlRaw.LandsatMetadataFile.ProjectionAttributes.Ellipsoid,
                UtmZone = mtlRaw.LandsatMetadataFile.ProjectionAttributes.UtmZone,
                GridCellSizeReflective = mtlRaw.LandsatMetadataFile.ProjectionAttributes.GridCellSizeReflective,
                GridCellSizeThermal = mtlRaw.LandsatMetadataFile.ProjectionAttributes.GridCellSizeThermal,
                ReflectiveLines = mtlRaw.LandsatMetadataFile.ProjectionAttributes.ReflectiveLines,
                ReflectiveSamples = mtlRaw.LandsatMetadataFile.ProjectionAttributes.ReflectiveSamples,
                ThermalLines = mtlRaw.LandsatMetadataFile.ProjectionAttributes.ThermalLines,
                ThermalSamples = mtlRaw.LandsatMetadataFile.ProjectionAttributes.ThermalSamples,
                Orientation = mtlRaw.LandsatMetadataFile.ProjectionAttributes.Orientation,
                CornerUlLatProduct = mtlRaw.LandsatMetadataFile.ProjectionAttributes.CornerUlLatProduct,
                CornerUlLonProduct = mtlRaw.LandsatMetadataFile.ProjectionAttributes.CornerUlLonProduct,
                CornerUrLatProduct = mtlRaw.LandsatMetadataFile.ProjectionAttributes.CornerUrLatProduct,
                CornerUrLonProduct = mtlRaw.LandsatMetadataFile.ProjectionAttributes.CornerUrLonProduct,
                CornerLlLatProduct = mtlRaw.LandsatMetadataFile.ProjectionAttributes.CornerLlLatProduct,
                CornerLlLonProduct = mtlRaw.LandsatMetadataFile.ProjectionAttributes.CornerLlLonProduct,
                CornerLrLatProduct = mtlRaw.LandsatMetadataFile.ProjectionAttributes.CornerLrLatProduct,
                CornerLrLonProduct = mtlRaw.LandsatMetadataFile.ProjectionAttributes.CornerLrLonProduct,
                CornerUlProjectionXProduct = mtlRaw.LandsatMetadataFile.ProjectionAttributes.CornerUlProjectionXProduct,
                CornerUlProjectionYProduct = mtlRaw.LandsatMetadataFile.ProjectionAttributes.CornerUlProjectionYProduct,
                CornerUrProjectionXProduct = mtlRaw.LandsatMetadataFile.ProjectionAttributes.CornerUrProjectionXProduct,
                CornerUrProjectionYProduct = mtlRaw.LandsatMetadataFile.ProjectionAttributes.CornerUrProjectionYProduct,
                CornerLlProjectionXProduct = mtlRaw.LandsatMetadataFile.ProjectionAttributes.CornerLlProjectionXProduct,
                CornerLlProjectionYProduct = mtlRaw.LandsatMetadataFile.ProjectionAttributes.CornerLlProjectionYProduct,
                CornerLrProjectionXProduct = mtlRaw.LandsatMetadataFile.ProjectionAttributes.CornerLrProjectionXProduct,
                CornerLrProjectionYProduct = mtlRaw.LandsatMetadataFile.ProjectionAttributes.CornerLrProjectionYProduct,
                CreateAt = satelliteDate
            };
            return entity;
        }

        private Level2ProcessingRecordEntity? CreateLevel2ProcessingRecord(LandsatSourceMetadataRaw mtlRaw, DateTime satelliteDate)
        {
            if (mtlRaw.Level2ProcessingRecordId.HasValue)
                return null;

            // Проверяем, есть ли данные Level2ProcessingRecord в mtlData
            if (mtlRaw.LandsatMetadataFile?.Level2ProcessingRecord == null)
                return null;

            var entity = new Level2ProcessingRecordEntity
            {
                Origin = mtlRaw.LandsatMetadataFile.Level2ProcessingRecord.Origin,
                DigitalObjectIdentifier = mtlRaw.LandsatMetadataFile.Level2ProcessingRecord.DigitalObjectIdentifier,
                RequestId = mtlRaw.LandsatMetadataFile.Level2ProcessingRecord.RequestId,
                LandsatProductId = mtlRaw.LandsatMetadataFile.Level2ProcessingRecord.LandsatProductId,
                ProcessingLevel = mtlRaw.LandsatMetadataFile.Level2ProcessingRecord.ProcessingLevel,
                OutputFormat = mtlRaw.LandsatMetadataFile.Level2ProcessingRecord.OutputFormat,
                DateProductGenerated = mtlRaw.LandsatMetadataFile.Level2ProcessingRecord.DateProductGenerated,
                ProcessingSoftwareVersion = mtlRaw.LandsatMetadataFile.Level2ProcessingRecord.ProcessingSoftwareVersion,
                AlgorithmSourceSurfaceReflectance = mtlRaw.LandsatMetadataFile.Level2ProcessingRecord.AlgorithmSourceSurfaceReflectance,
                DataSourceOzone = mtlRaw.LandsatMetadataFile.Level2ProcessingRecord.DataSourceOzone,
                DataSourcePressure = mtlRaw.LandsatMetadataFile.Level2ProcessingRecord.DataSourcePressure,
                DataSourceWaterVapor = mtlRaw.LandsatMetadataFile.Level2ProcessingRecord.DataSourceWaterVapor,
                DataSourceAirTemperature = mtlRaw.LandsatMetadataFile.Level2ProcessingRecord.DataSourceAirTemperature,
                AlgorithmSourceSurfaceTemperature = mtlRaw.LandsatMetadataFile.Level2ProcessingRecord.AlgorithmSourceSurfaceTemperature,
                DataSourceReanalysis = mtlRaw.LandsatMetadataFile.Level2ProcessingRecord.DataSourceReanalysis,
                CreateAt = satelliteDate
            };
            return entity;
        }

        private Level2SurfaceReflectanceParametersEntity? CreateLevel2SurfaceReflectanceParameters(LandsatSourceMetadataRaw mtlRaw, DateTime satelliteDate)
        {
            if (mtlRaw.Level2SurfaceReflectanceParametersId.HasValue)
                return null;

            // Проверяем, есть ли данные Level2SurfaceReflectanceParameters в mtlData
            if (mtlRaw.LandsatMetadataFile?.Level2SurfaceReflectanceParameters == null)
                return null;

            var entity = new Level2SurfaceReflectanceParametersEntity
            {
                ReflectanceMaximumBand1 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceMaximumBand1,
                ReflectanceMinimumBand1 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceMinimumBand1,
                ReflectanceMaximumBand2 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceMaximumBand2,
                ReflectanceMinimumBand2 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceMinimumBand2,
                ReflectanceMaximumBand3 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceMaximumBand3,
                ReflectanceMinimumBand3 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceMinimumBand3,
                ReflectanceMaximumBand4 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceMaximumBand4,
                ReflectanceMinimumBand4 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceMinimumBand4,
                ReflectanceMaximumBand5 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceMaximumBand5,
                ReflectanceMinimumBand5 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceMinimumBand5,
                ReflectanceMaximumBand6 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceMaximumBand6,
                ReflectanceMinimumBand6 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceMinimumBand6,
                ReflectanceMaximumBand7 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceMaximumBand7,
                ReflectanceMinimumBand7 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceMinimumBand7,
                QuantizeCalMaxBand1 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.QuantizeCalMaxBand1,
                QuantizeCalMinBand1 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.QuantizeCalMinBand1,
                QuantizeCalMaxBand2 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.QuantizeCalMaxBand2,
                QuantizeCalMinBand2 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.QuantizeCalMinBand2,
                QuantizeCalMaxBand3 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.QuantizeCalMaxBand3,
                QuantizeCalMinBand3 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.QuantizeCalMinBand3,
                QuantizeCalMaxBand4 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.QuantizeCalMaxBand4,
                QuantizeCalMinBand4 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.QuantizeCalMinBand4,
                QuantizeCalMaxBand5 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.QuantizeCalMaxBand5,
                QuantizeCalMinBand5 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.QuantizeCalMinBand5,
                QuantizeCalMaxBand6 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.QuantizeCalMaxBand6,
                QuantizeCalMinBand6 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.QuantizeCalMinBand6,
                QuantizeCalMaxBand7 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.QuantizeCalMaxBand7,
                QuantizeCalMinBand7 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.QuantizeCalMinBand7,
                ReflectanceMultBand1 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceMultBand1,
                ReflectanceMultBand2 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceMultBand2,
                ReflectanceMultBand3 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceMultBand3,
                ReflectanceMultBand4 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceMultBand4,
                ReflectanceMultBand5 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceMultBand5,
                ReflectanceMultBand6 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceMultBand6,
                ReflectanceMultBand7 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceMultBand7,
                ReflectanceAddBand1 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceAddBand1,
                ReflectanceAddBand2 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceAddBand2,
                ReflectanceAddBand3 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceAddBand3,
                ReflectanceAddBand4 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceAddBand4,
                ReflectanceAddBand5 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceAddBand5,
                ReflectanceAddBand6 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceAddBand6,
                ReflectanceAddBand7 = mtlRaw.LandsatMetadataFile.Level2SurfaceReflectanceParameters.ReflectanceAddBand7,
                CreateAt = satelliteDate
            };
            return entity;
        }

        private Level2SurfaceTemperatureParametersEntity? CreateLevel2SurfaceTemperatureParameters(LandsatSourceMetadataRaw mtlRaw, DateTime satelliteDate)
        {
            if (mtlRaw.Level2SurfaceTemperatureParametersId.HasValue)
                return null;

            // Проверяем, есть ли данные Level2SurfaceTemperatureParameters в mtlData
            if (mtlRaw.LandsatMetadataFile?.Level2SurfaceTemperatureParameters == null)
                return null;

            var entity = new Level2SurfaceTemperatureParametersEntity
            {
                TemperatureMaximumBandStB10 = mtlRaw.LandsatMetadataFile.Level2SurfaceTemperatureParameters.TemperatureMaximumBandStB10,
                TemperatureMinimumBandStB10 = mtlRaw.LandsatMetadataFile.Level2SurfaceTemperatureParameters.TemperatureMinimumBandStB10,
                QuantizeCalMaximumBandStB10 = mtlRaw.LandsatMetadataFile.Level2SurfaceTemperatureParameters.QuantizeCalMaximumBandStB10,
                QuantizeCalMinimumBandStB10 = mtlRaw.LandsatMetadataFile.Level2SurfaceTemperatureParameters.QuantizeCalMinimumBandStB10,
                TemperatureMultBandStB10 = mtlRaw.LandsatMetadataFile.Level2SurfaceTemperatureParameters.TemperatureMultBandStB10,
                TemperatureAddBandStB10 = mtlRaw.LandsatMetadataFile.Level2SurfaceTemperatureParameters.TemperatureAddBandStB10,
                CreateAt = satelliteDate
            };
            return entity;
        }

        private Level1ProcessingRecordEntity? CreateLevel1ProcessingRecord(LandsatSourceMetadataRaw mtlRaw, DateTime satelliteDate)
        {
            if (mtlRaw.Level1ProcessingRecordId.HasValue)
                return null;

            // Проверяем, есть ли данные Level1ProcessingRecord в mtlData
            if (mtlRaw.LandsatMetadataFile?.Level1ProcessingRecord == null)
                return null;

            var entity = new Level1ProcessingRecordEntity
            {
                Origin = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.Origin,
                DigitalObjectIdentifier = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.DigitalObjectIdentifier,
                RequestId = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.RequestId,
                LandsatSceneId = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.LandsatSceneId,
                LandsatProductId = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.LandsatProductId,
                ProcessingLevel = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.ProcessingLevel,
                CollectionCategory = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.CollectionCategory,
                OutputFormat = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.OutputFormat,
                DateProductGenerated = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.DateProductGenerated,
                ProcessingSoftwareVersion = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.ProcessingSoftwareVersion,
                FileNameBand1 = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.FileNameBand1,
                FileNameBand2 = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.FileNameBand2,
                FileNameBand3 = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.FileNameBand3,
                FileNameBand4 = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.FileNameBand4,
                FileNameBand5 = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.FileNameBand5,
                FileNameBand6 = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.FileNameBand6,
                FileNameBand7 = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.FileNameBand7,
                FileNameBand8 = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.FileNameBand8,
                FileNameBand9 = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.FileNameBand9,
                FileNameBand10 = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.FileNameBand10,
                FileNameBand11 = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.FileNameBand11,
                FileNameQualityL1Pixel = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.FileNameQualityL1Pixel,
                FileNameQualityL1RadiometricSaturation = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.FileNameQualityL1RadiometricSaturation,
                FileNameAngleCoefficient = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.FileNameAngleCoefficient,
                FileNameAngleSensorAzimuthBand4 = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.FileNameAngleSensorAzimuthBand4,
                FileNameAngleSensorZenithBand4 = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.FileNameAngleSensorZenithBand4,
                FileNameAngleSolarAzimuthBand4 = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.FileNameAngleSolarAzimuthBand4,
                FileNameAngleSolarZenithBand4 = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.FileNameAngleSolarZenithBand4,
                FileNameMetadataOdl = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.FileNameMetadataOdl,
                FileNameMetadataXml = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.FileNameMetadataXml,
                FileNameCpf = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.FileNameCpf,
                FileNameBpfOli = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.FileNameBpfOli,
                FileNameBpfTirs = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.FileNameBpfTirs,
                FileNameRlut = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.FileNameRlut,
                DataSourceElevation = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.DataSourceElevation,
                GroundControlPointsVersion = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.GroundControlPointsVersion,
                GroundControlPointsModel = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.GroundControlPointsModel,
                GeometricRmseModel = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.GeometricRmseModel,
                GeometricRmseModelY = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.GeometricRmseModelY,
                GeometricRmseModelX = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.GeometricRmseModelX,
                GroundControlPointsVerify = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.GroundControlPointsVerify,
                GeometricRmseVerify = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.GeometricRmseVerify,
                EphemerisType = mtlRaw.LandsatMetadataFile.Level1ProcessingRecord.EphemerisType,
                CreateAt = satelliteDate
            };
            return entity;
        }

        private Level1MinMaxRadianceEntity? CreateLevel1MinMaxRadiance(LandsatSourceMetadataRaw mtlRaw, DateTime satelliteDate)
        {
            if (mtlRaw.Level1MinMaxRadianceId.HasValue)
                return null;

            // Проверяем, есть ли данные Level1MinMaxRadiance в mtlData
            if (mtlRaw.LandsatMetadataFile?.Level1MinMaxRadiance == null)
                return null;

            var entity = new Level1MinMaxRadianceEntity
            {
                RadianceMaximumBand1 = mtlRaw.LandsatMetadataFile.Level1MinMaxRadiance.RadianceMaximumBand1,
                RadianceMinimumBand1 = mtlRaw.LandsatMetadataFile.Level1MinMaxRadiance.RadianceMinimumBand1,
                RadianceMaximumBand2 = mtlRaw.LandsatMetadataFile.Level1MinMaxRadiance.RadianceMaximumBand2,
                RadianceMinimumBand2 = mtlRaw.LandsatMetadataFile.Level1MinMaxRadiance.RadianceMinimumBand2,
                RadianceMaximumBand3 = mtlRaw.LandsatMetadataFile.Level1MinMaxRadiance.RadianceMaximumBand3,
                RadianceMinimumBand3 = mtlRaw.LandsatMetadataFile.Level1MinMaxRadiance.RadianceMinimumBand3,
                RadianceMaximumBand4 = mtlRaw.LandsatMetadataFile.Level1MinMaxRadiance.RadianceMaximumBand4,
                RadianceMinimumBand4 = mtlRaw.LandsatMetadataFile.Level1MinMaxRadiance.RadianceMinimumBand4,
                RadianceMaximumBand5 = mtlRaw.LandsatMetadataFile.Level1MinMaxRadiance.RadianceMaximumBand5,
                RadianceMinimumBand5 = mtlRaw.LandsatMetadataFile.Level1MinMaxRadiance.RadianceMinimumBand5,
                RadianceMaximumBand6 = mtlRaw.LandsatMetadataFile.Level1MinMaxRadiance.RadianceMaximumBand6,
                RadianceMinimumBand6 = mtlRaw.LandsatMetadataFile.Level1MinMaxRadiance.RadianceMinimumBand6,
                RadianceMaximumBand7 = mtlRaw.LandsatMetadataFile.Level1MinMaxRadiance.RadianceMaximumBand7,
                RadianceMinimumBand7 = mtlRaw.LandsatMetadataFile.Level1MinMaxRadiance.RadianceMinimumBand7,
                RadianceMaximumBand8 = mtlRaw.LandsatMetadataFile.Level1MinMaxRadiance.RadianceMaximumBand8,
                RadianceMinimumBand8 = mtlRaw.LandsatMetadataFile.Level1MinMaxRadiance.RadianceMinimumBand8,
                RadianceMaximumBand9 = mtlRaw.LandsatMetadataFile.Level1MinMaxRadiance.RadianceMaximumBand9,
                RadianceMinimumBand9 = mtlRaw.LandsatMetadataFile.Level1MinMaxRadiance.RadianceMinimumBand9,
                RadianceMaximumBand10 = mtlRaw.LandsatMetadataFile.Level1MinMaxRadiance.RadianceMaximumBand10,
                RadianceMinimumBand10 = mtlRaw.LandsatMetadataFile.Level1MinMaxRadiance.RadianceMinimumBand10,
                RadianceMaximumBand11 = mtlRaw.LandsatMetadataFile.Level1MinMaxRadiance.RadianceMaximumBand11,
                RadianceMinimumBand11 = mtlRaw.LandsatMetadataFile.Level1MinMaxRadiance.RadianceMinimumBand11,
                CreateAt = satelliteDate
            };
            return entity;
        }

        private Level1MinMaxReflectanceEntity? CreateLevel1MinMaxReflectance(LandsatSourceMetadataRaw mtlRaw, DateTime satelliteDate)
        {
            if (mtlRaw.Level1MinMaxReflectanceId.HasValue)
                return null;

            // Проверяем, есть ли данные Level1MinMaxReflectance в mtlData
            if (mtlRaw.LandsatMetadataFile?.Level1MinMaxReflectance == null)
                return null;

            var entity = new Level1MinMaxReflectanceEntity
            {
                ReflectanceMaximumBand1 = mtlRaw.LandsatMetadataFile.Level1MinMaxReflectance.ReflectanceMaximumBand1,
                ReflectanceMinimumBand1 = mtlRaw.LandsatMetadataFile.Level1MinMaxReflectance.ReflectanceMinimumBand1,
                ReflectanceMaximumBand2 = mtlRaw.LandsatMetadataFile.Level1MinMaxReflectance.ReflectanceMaximumBand2,
                ReflectanceMinimumBand2 = mtlRaw.LandsatMetadataFile.Level1MinMaxReflectance.ReflectanceMinimumBand2,
                ReflectanceMaximumBand3 = mtlRaw.LandsatMetadataFile.Level1MinMaxReflectance.ReflectanceMaximumBand3,
                ReflectanceMinimumBand3 = mtlRaw.LandsatMetadataFile.Level1MinMaxReflectance.ReflectanceMinimumBand3,
                ReflectanceMaximumBand4 = mtlRaw.LandsatMetadataFile.Level1MinMaxReflectance.ReflectanceMaximumBand4,
                ReflectanceMinimumBand4 = mtlRaw.LandsatMetadataFile.Level1MinMaxReflectance.ReflectanceMinimumBand4,
                ReflectanceMaximumBand5 = mtlRaw.LandsatMetadataFile.Level1MinMaxReflectance.ReflectanceMaximumBand5,
                ReflectanceMinimumBand5 = mtlRaw.LandsatMetadataFile.Level1MinMaxReflectance.ReflectanceMinimumBand5,
                ReflectanceMaximumBand6 = mtlRaw.LandsatMetadataFile.Level1MinMaxReflectance.ReflectanceMaximumBand6,
                ReflectanceMinimumBand6 = mtlRaw.LandsatMetadataFile.Level1MinMaxReflectance.ReflectanceMinimumBand6,
                ReflectanceMaximumBand7 = mtlRaw.LandsatMetadataFile.Level1MinMaxReflectance.ReflectanceMaximumBand7,
                ReflectanceMinimumBand7 = mtlRaw.LandsatMetadataFile.Level1MinMaxReflectance.ReflectanceMinimumBand7,
                ReflectanceMaximumBand8 = mtlRaw.LandsatMetadataFile.Level1MinMaxReflectance.ReflectanceMaximumBand8,
                ReflectanceMinimumBand8 = mtlRaw.LandsatMetadataFile.Level1MinMaxReflectance.ReflectanceMinimumBand8,
                ReflectanceMaximumBand9 = mtlRaw.LandsatMetadataFile.Level1MinMaxReflectance.ReflectanceMaximumBand9,
                ReflectanceMinimumBand9 = mtlRaw.LandsatMetadataFile.Level1MinMaxReflectance.ReflectanceMinimumBand9,
                CreateAt = satelliteDate
            };
            return entity;
        }

        private Level1MinMaxPixelValueEntity? CreateLevel1MinMaxPixelValue(LandsatSourceMetadataRaw mtlRaw, DateTime satelliteDate)
        {
            if (mtlRaw.Level1MinMaxPixelValueId.HasValue)
                return null;

            // Проверяем, есть ли данные Level1MinMaxPixelValue в mtlData
            if (mtlRaw.LandsatMetadataFile?.Level1MinMaxPixelValue == null)
                return null;

            var entity = new Level1MinMaxPixelValueEntity
            {
                QuantizeCalMaxBand1 = mtlRaw.LandsatMetadataFile.Level1MinMaxPixelValue.QuantizeCalMaxBand1,
                QuantizeCalMinBand1 = mtlRaw.LandsatMetadataFile.Level1MinMaxPixelValue.QuantizeCalMinBand1,
                QuantizeCalMaxBand2 = mtlRaw.LandsatMetadataFile.Level1MinMaxPixelValue.QuantizeCalMaxBand2,
                QuantizeCalMinBand2 = mtlRaw.LandsatMetadataFile.Level1MinMaxPixelValue.QuantizeCalMinBand2,
                QuantizeCalMaxBand3 = mtlRaw.LandsatMetadataFile.Level1MinMaxPixelValue.QuantizeCalMaxBand3,
                QuantizeCalMinBand3 = mtlRaw.LandsatMetadataFile.Level1MinMaxPixelValue.QuantizeCalMinBand3,
                QuantizeCalMaxBand4 = mtlRaw.LandsatMetadataFile.Level1MinMaxPixelValue.QuantizeCalMaxBand4,
                QuantizeCalMinBand4 = mtlRaw.LandsatMetadataFile.Level1MinMaxPixelValue.QuantizeCalMinBand4,
                QuantizeCalMaxBand5 = mtlRaw.LandsatMetadataFile.Level1MinMaxPixelValue.QuantizeCalMaxBand5,
                QuantizeCalMinBand5 = mtlRaw.LandsatMetadataFile.Level1MinMaxPixelValue.QuantizeCalMinBand5,
                QuantizeCalMaxBand6 = mtlRaw.LandsatMetadataFile.Level1MinMaxPixelValue.QuantizeCalMaxBand6,
                QuantizeCalMinBand6 = mtlRaw.LandsatMetadataFile.Level1MinMaxPixelValue.QuantizeCalMinBand6,
                QuantizeCalMaxBand7 = mtlRaw.LandsatMetadataFile.Level1MinMaxPixelValue.QuantizeCalMaxBand7,
                QuantizeCalMinBand7 = mtlRaw.LandsatMetadataFile.Level1MinMaxPixelValue.QuantizeCalMinBand7,
                QuantizeCalMaxBand8 = mtlRaw.LandsatMetadataFile.Level1MinMaxPixelValue.QuantizeCalMaxBand8,
                QuantizeCalMinBand8 = mtlRaw.LandsatMetadataFile.Level1MinMaxPixelValue.QuantizeCalMinBand8,
                QuantizeCalMaxBand9 = mtlRaw.LandsatMetadataFile.Level1MinMaxPixelValue.QuantizeCalMaxBand9,
                QuantizeCalMinBand9 = mtlRaw.LandsatMetadataFile.Level1MinMaxPixelValue.QuantizeCalMinBand9,
                QuantizeCalMaxBand10 = mtlRaw.LandsatMetadataFile.Level1MinMaxPixelValue.QuantizeCalMaxBand10,
                QuantizeCalMinBand10 = mtlRaw.LandsatMetadataFile.Level1MinMaxPixelValue.QuantizeCalMinBand10,
                QuantizeCalMaxBand11 = mtlRaw.LandsatMetadataFile.Level1MinMaxPixelValue.QuantizeCalMaxBand11,
                QuantizeCalMinBand11 = mtlRaw.LandsatMetadataFile.Level1MinMaxPixelValue.QuantizeCalMinBand11,
                CreateAt = satelliteDate
            };
            return entity;
        }

        private Level1RadiometricRescalingEntity? CreateLevel1RadiometricRescaling(LandsatSourceMetadataRaw mtlRaw, DateTime satelliteDate)
        {
            if (mtlRaw.Level1RadiometricRescalingId.HasValue)
                return null;

            // Проверяем, есть ли данные Level1RadiometricRescaling в mtlData
            if (mtlRaw.LandsatMetadataFile?.Level1RadiometricRescaling == null)
                return null;

            var entity = new Level1RadiometricRescalingEntity
            {
                RadianceMultBand1 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.RadianceMultBand1,
                RadianceMultBand2 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.RadianceMultBand2,
                RadianceMultBand3 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.RadianceMultBand3,
                RadianceMultBand4 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.RadianceMultBand4,
                RadianceMultBand5 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.RadianceMultBand5,
                RadianceMultBand6 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.RadianceMultBand6,
                RadianceMultBand7 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.RadianceMultBand7,
                RadianceMultBand8 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.RadianceMultBand8,
                RadianceMultBand9 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.RadianceMultBand9,
                RadianceMultBand10 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.RadianceMultBand10,
                RadianceMultBand11 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.RadianceMultBand11,
                RadianceAddBand1 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.RadianceAddBand1,
                RadianceAddBand2 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.RadianceAddBand2,
                RadianceAddBand3 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.RadianceAddBand3,
                RadianceAddBand4 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.RadianceAddBand4,
                RadianceAddBand5 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.RadianceAddBand5,
                RadianceAddBand6 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.RadianceAddBand6,
                RadianceAddBand7 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.RadianceAddBand7,
                RadianceAddBand8 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.RadianceAddBand8,
                RadianceAddBand9 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.RadianceAddBand9,
                RadianceAddBand10 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.RadianceAddBand10,
                RadianceAddBand11 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.RadianceAddBand11,
                ReflectanceMultBand1 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.ReflectanceMultBand1,
                ReflectanceMultBand2 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.ReflectanceMultBand2,
                ReflectanceMultBand3 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.ReflectanceMultBand3,
                ReflectanceMultBand4 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.ReflectanceMultBand4,
                ReflectanceMultBand5 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.ReflectanceMultBand5,
                ReflectanceMultBand6 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.ReflectanceMultBand6,
                ReflectanceMultBand7 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.ReflectanceMultBand7,
                ReflectanceMultBand8 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.ReflectanceMultBand8,
                ReflectanceMultBand9 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.ReflectanceMultBand9,
                ReflectanceAddBand1 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.ReflectanceAddBand1,
                ReflectanceAddBand2 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.ReflectanceAddBand2,
                ReflectanceAddBand3 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.ReflectanceAddBand3,
                ReflectanceAddBand4 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.ReflectanceAddBand4,
                ReflectanceAddBand5 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.ReflectanceAddBand5,
                ReflectanceAddBand6 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.ReflectanceAddBand6,
                ReflectanceAddBand7 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.ReflectanceAddBand7,
                ReflectanceAddBand8 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.ReflectanceAddBand8,
                ReflectanceAddBand9 = mtlRaw.LandsatMetadataFile.Level1RadiometricRescaling.ReflectanceAddBand9,
                CreateAt = satelliteDate
            };
            return entity;
        }

        private Level1ThermalConstantsEntity? CreateLevel1ThermalConstants(LandsatSourceMetadataRaw mtlRaw, DateTime satelliteDate)
        {
            if (mtlRaw.Level1ThermalConstantsId.HasValue)
                return null;

            // Проверяем, есть ли данные Level1ThermalConstants в mtlData
            if (mtlRaw.LandsatMetadataFile?.Level1ThermalConstants == null)
                return null;

            var entity = new Level1ThermalConstantsEntity
            {
                K1ConstantBand10 = mtlRaw.LandsatMetadataFile.Level1ThermalConstants.K1ConstantBand10,
                K2ConstantBand10 = mtlRaw.LandsatMetadataFile.Level1ThermalConstants.K2ConstantBand10,
                K1ConstantBand11 = mtlRaw.LandsatMetadataFile.Level1ThermalConstants.K1ConstantBand11,
                K2ConstantBand11 = mtlRaw.LandsatMetadataFile.Level1ThermalConstants.K2ConstantBand11,
                CreateAt = satelliteDate
            };
            return entity;
        }

        private Level1ProjectionParametersEntity? CreateLevel1ProjectionParameters(LandsatSourceMetadataRaw mtlRaw, DateTime satelliteDate)
        {
            if (mtlRaw.Level1ProjectionParametersId.HasValue)
                return null;

            // Проверяем, есть ли данные Level1ProjectionParameters в mtlData
            if (mtlRaw.LandsatMetadataFile?.Level1ProjectionParameters == null)
                return null;

            var entity = new Level1ProjectionParametersEntity
            {
                MapProjection = mtlRaw.LandsatMetadataFile.Level1ProjectionParameters.MapProjection,
                Datum = mtlRaw.LandsatMetadataFile.Level1ProjectionParameters.Datum,
                Ellipsoid = mtlRaw.LandsatMetadataFile.Level1ProjectionParameters.Ellipsoid,
                UtmZone = mtlRaw.LandsatMetadataFile.Level1ProjectionParameters.UtmZone,
                GridCellSizePanchromatic = mtlRaw.LandsatMetadataFile.Level1ProjectionParameters.GridCellSizePanchromatic,
                GridCellSizeReflective = mtlRaw.LandsatMetadataFile.Level1ProjectionParameters.GridCellSizeReflective,
                GridCellSizeThermal = mtlRaw.LandsatMetadataFile.Level1ProjectionParameters.GridCellSizeThermal,
                Orientation = mtlRaw.LandsatMetadataFile.Level1ProjectionParameters.Orientation,
                ResamplingOption = mtlRaw.LandsatMetadataFile.Level1ProjectionParameters.ResamplingOption,
                CreateAt = satelliteDate
            };
            return entity;
        }










    }
}