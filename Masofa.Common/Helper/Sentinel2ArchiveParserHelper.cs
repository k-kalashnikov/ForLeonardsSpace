using Masofa.Common.Models.Satellite.Parse.Sentinel;
using Masofa.Common.Models.Satellite.Parse.Sentinel.Inspire;
using Masofa.Common.Models.Satellite.Parse.Sentinel.L1C;
using Masofa.Common.Models.Satellite.Parse.Sentinel.Quality;
using Masofa.Common.Models.Satellite.Parse.Sentinel.Tile;
using Masofa.Common.Models.Satellite.Sentinel;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;


namespace Masofa.Common.Helper
{
    // DTO модели для возврата результатов парсинга
    public class SentinelL1CParseResult
    {
        public SentinelL1CProductMetadata Metadata { get; set; }
        public DateTime SatelliteDate { get; set; }
    }

    public class SentinelInspireParseResult
    {
        public SentinelInspireMetadata Metadata { get; set; }
        public DateTime SatelliteDate { get; set; }
    }

    public class SentinelTileParseResult
    {
        public SentinelL1CTileMetadata Metadata { get; set; }
        public DateTime SatelliteDate { get; set; }
    }

    public class SentinelQualityParseResult
    {
        public SentinelProductQualityMetadata Metadata { get; set; }
        public DateTime SatelliteDate { get; set; }
    }

    public static class Sentinel2ArchiveParserHelper
    {
        /// <summary>
        /// Парсит L1C Product Metadata XML файл
        /// </summary>
        public static async Task<SentinelL1CParseResult> ParseL1CProductMetadataAsync(Stream fileStream)
        {
            // 1. Конвертируем XML в JSON
            var jsonContent = await XmlToJsonConverter.ConvertXmlStreamToJsonAsync(fileStream);
            if (string.IsNullOrWhiteSpace(jsonContent))
                throw new ArgumentException("JSON-содержимое не может быть пустым");

            // 2. Десериализуем в JSON модель
            var jsonMetadata = DeserializeL1CProductMetadataJson(jsonContent);
            if (jsonMetadata == null)
                throw new InvalidOperationException("Не удалось десериализовать JSON в модель L1C Product Metadata");

            return ParseFlatL1CProductMetadata(jsonMetadata);
        }

        /// <summary>
        /// Парсит INSPIRE Metadata XML файл
        /// </summary>
        public static async Task<SentinelInspireParseResult> ParseInspireMetadataAsync(Stream fileStream)
        {
            // 1. Конвертируем XML в JSON
            var jsonContent = await XmlToJsonConverter.ConvertXmlStreamToJsonAsync(fileStream);
            if (string.IsNullOrWhiteSpace(jsonContent))
                throw new ArgumentException("JSON-содержимое не может быть пустым");

            // 2. Десериализуем в JSON модель
            var jsonMetadata = DeserializeInspireMetadataJson(jsonContent);
            if (jsonMetadata == null)
                throw new InvalidOperationException("Не удалось десериализовать JSON в модель INSPIRE Metadata");

            // 4. Преобразование в модель базы данных
            var dbModel = new SentinelInspireMetadata
            {
                FileIdentifier = jsonMetadata.FileIdentifier?.CharacterString,
                LanguageCode = jsonMetadata.Language?.LanguageCode,
                CharacterSetCode = jsonMetadata.CharacterSet?.MDCharacterSetCode,
                HierarchyLevelCode = jsonMetadata.HierarchyLevel?.MDScopeCode,
                DateStamp = DateTimeHelper.ParseXmlDateTime(jsonMetadata.DateStamp?.Date),
                MetadataStandardName = jsonMetadata.MetadataStandardName?.CharacterString,
                MetadataStandardVersion = jsonMetadata.MetadataStandardVersion?.CharacterString,
                OrganisationName = jsonMetadata.Contact?.CIResponsibleParty?.OrganisationName?.CharacterString,
                Email = jsonMetadata.Contact?.CIResponsibleParty?.ContactInfo?.CIContact?.Address?.CIAddress?.ElectronicMailAddress?.CharacterString,
                RoleCode = jsonMetadata.Contact?.CIResponsibleParty?.Role?.CIRoleCode,
                // Geographic coordinates
                WestBoundLongitude = decimal.TryParse(jsonMetadata.IdentificationInfo?.MDDataIdentification?.Extent?.EXExtent?.GeographicElement?.EXGeographicBoundingBox?.WestBoundLongitude?.Decimal, NumberStyles.Any, CultureInfo.InvariantCulture, out var west) ? west : 0,
                EastBoundLongitude = decimal.TryParse(jsonMetadata.IdentificationInfo?.MDDataIdentification?.Extent?.EXExtent?.GeographicElement?.EXGeographicBoundingBox?.EastBoundLongitude?.Decimal, NumberStyles.Any, CultureInfo.InvariantCulture, out var east) ? east : 0,
                SouthBoundLatitude = decimal.TryParse(jsonMetadata.IdentificationInfo?.MDDataIdentification?.Extent?.EXExtent?.GeographicElement?.EXGeographicBoundingBox?.SouthBoundLatitude?.Decimal, NumberStyles.Any, CultureInfo.InvariantCulture, out var south) ? south : 0,
                NorthBoundLatitude = decimal.TryParse(jsonMetadata.IdentificationInfo?.MDDataIdentification?.Extent?.EXExtent?.GeographicElement?.EXGeographicBoundingBox?.NorthBoundLatitude?.Decimal, NumberStyles.Any, CultureInfo.InvariantCulture, out var north) ? north : 0,
                // Reference System
                ReferenceSystemCode = jsonMetadata.ReferenceSystemInfo?.MDReferenceSystem?.ReferenceSystemIdentifier?.RSIdentifier?.Code?.CharacterString,
                ReferenceSystemCodeSpace = jsonMetadata.ReferenceSystemInfo?.MDReferenceSystem?.ReferenceSystemIdentifier?.RSIdentifier?.CodeSpace?.CharacterString
            };

            return new SentinelInspireParseResult
            {
                Metadata = dbModel
            };
        }

        public static SentinelInspireMetadata ParseInspireMetadataAsyncAsString(string xmlContent)
        {
            var doc = XDocument.Parse(xmlContent);
            var ns = doc.Root?.GetDefaultNamespace() ?? XNamespace.None;

            var metadata = new SentinelInspireMetadata();

            // Простые текстовые поля
            metadata.FileIdentifier = doc.Root?.Element(ns + "fileIdentifier")?.Value.Trim();
            metadata.LanguageCode = doc.Root?.Element(ns + "language")?.Element(ns + "LanguageCode")?.Value.Trim();
            metadata.CharacterSetCode = doc.Root?.Element(ns + "characterSet")?.Element(ns + "MD_CharacterSetCode")?.Value.Trim();
            metadata.HierarchyLevelCode = doc.Root?.Element(ns + "hierarchyLevel")?.Element(ns + "MD_ScopeCode")?.Value.Trim();
            metadata.MetadataStandardName = doc.Root?.Element(ns + "metadataStandardName")?.Value.Trim();
            metadata.MetadataStandardVersion = doc.Root?.Element(ns + "metadataStandardVersion")?.Value.Trim();

            var originDateTime = doc.Root?.Element(ns + "identificationInfo")
                     ?.Element(ns + "MD_DataIdentification")
                     ?.Element(ns + "extent")
                     ?.Element(ns + "EX_Extent")
                     ?.Element(ns + "temporalElement")
                     ?.Element(ns + "EX_TemporalExtent")
                     ?.Element(ns + "extent")
                     ?.Element(ns + "TimePeriod")
                     ?.Element(ns + "beginPosition");

            // Дата
            if (DateTime.TryParse(originDateTime?.Value.Trim(), out var dateStamp))
            {
                metadata.DateStamp = new DateTime(dateStamp.Year, dateStamp.Month, dateStamp.Day, 0, 0, 0, DateTimeKind.Utc);
            }

            // Contact (берём первый, если несколько)
            var contact = doc.Root?.Element(ns + "contact")?.Element(ns + "CI_ResponsibleParty");
            metadata.OrganisationName = contact?.Element(ns + "organisationName")?.Value.Trim();
            metadata.Email = contact?.Element(ns + "contactInfo")
                                     ?.Element(ns + "CI_Contact")
                                     ?.Element(ns + "address")
                                     ?.Element(ns + "CI_Address")
                                     ?.Element(ns + "electronicMailAddress")?.Value.Trim();
            metadata.RoleCode = contact?.Element(ns + "role")
                                       ?.Element(ns + "CI_RoleCode")?.Value.Trim();

            // Reference System
            var rsIdentifier = doc.Root?.Element(ns + "referenceSystemInfo")
                                         ?.Element(ns + "MD_ReferenceSystem")
                                         ?.Element(ns + "referenceSystemIdentifier")
                                         ?.Element(ns + "RS_Identifier");
            metadata.ReferenceSystemCode = rsIdentifier?.Element(ns + "code")?.Value.Trim();
            metadata.ReferenceSystemCodeSpace = rsIdentifier?.Element(ns + "codeSpace")?.Value.Trim();

            // Geographic Bounding Box
            var bbox = doc.Root?.Element(ns + "identificationInfo")
                                 ?.Element(ns + "MD_DataIdentification")
                                 ?.Element(ns + "extent")
                                 ?.Element(ns + "EX_Extent")
                                 ?.Element(ns + "geographicElement")
                                 ?.Element(ns + "EX_GeographicBoundingBox");

            if (bbox != null)
            {
                decimal.TryParse(bbox.Element(ns + "westBoundLongitude")?.Value.Trim(), out var west);
                decimal.TryParse(bbox.Element(ns + "eastBoundLongitude")?.Value.Trim(), out var east);
                decimal.TryParse(bbox.Element(ns + "southBoundLatitude")?.Value.Trim(), out var south);
                decimal.TryParse(bbox.Element(ns + "northBoundLatitude")?.Value.Trim(), out var north);

                metadata.WestBoundLongitude = west;
                metadata.EastBoundLongitude = east;
                metadata.SouthBoundLatitude = south;
                metadata.NorthBoundLatitude = north;
            }

            return metadata;
        }

        /// <summary>
        /// Парсит L1C Tile Metadata XML файл
        /// </summary>
        public static async Task<SentinelTileParseResult> ParseL1CTileMetadataAsync(Stream fileStream)
        {
            // 1. Конвертируем XML в JSON
            var jsonContent = await XmlToJsonConverter.ConvertXmlStreamToJsonAsync(fileStream);
            if (string.IsNullOrWhiteSpace(jsonContent))
                throw new ArgumentException("JSON-содержимое не может быть пустым");

            // 2. Десериализуем в JSON модель
            var jsonMetadata = DeserializeL1CTileMetadataJson(jsonContent);
            if (jsonMetadata == null)
                throw new InvalidOperationException("Не удалось десериализовать JSON в модель L1C Tile Metadata");

            // 3. Извлекаем дату спутника
            var satelliteDate = ExtractSatelliteDateFromTile(jsonMetadata);

            // 4. Преобразование в модель базы данных
            var dbModel = new SentinelL1CTileMetadata
            {
                TileId = jsonMetadata.GeneralInfo?.TileId,
                DatastripId = jsonMetadata.GeneralInfo?.DatastripId,
                DownlinkPriority = jsonMetadata.GeneralInfo?.DownlinkPriority,
                SensingTime = DateTimeHelper.SafeParseDateTime(jsonMetadata.GeneralInfo?.SensingTime),
                ArchivingCentre = jsonMetadata.GeneralInfo?.ArchivingInfo?.ArchivingCentre,
                ArchivingTime = DateTimeHelper.SafeParseDateTime(jsonMetadata.GeneralInfo?.ArchivingInfo?.ArchivingTime),
                HorizontalCSName = jsonMetadata.GeometricInfo?.TileGeocoding?.HorizontalCsName,
                HorizontalCSCode = jsonMetadata.GeometricInfo?.TileGeocoding?.HorizontalCsCode,
                SizesRaw = jsonMetadata.GeometricInfo?.TileGeocoding?.Sizes != null
                    ? string.Join("~", jsonMetadata.GeometricInfo.TileGeocoding.Sizes.Select(s => s.ToString()))
                    : null,
                GeopositionsRaw = jsonMetadata.GeometricInfo?.TileGeocoding?.Geopositions != null
                    ? string.Join("~", jsonMetadata.GeometricInfo.TileGeocoding.Geopositions.Select(g => g.ToString()))
                    : null,
                // Tile Angles - Sun Angles Grid
                SunAnglesZenithColStepValue = jsonMetadata.GeometricInfo?.TileAngles?.SunAnglesGrid?.Zenith?.ColStep ?? 0,
                SunAnglesZenithRowStepValue = jsonMetadata.GeometricInfo?.TileAngles?.SunAnglesGrid?.Zenith?.RowStep ?? 0,
                SunAnglesZenithValuesList = jsonMetadata.GeometricInfo?.TileAngles?.SunAnglesGrid?.Zenith?.ValuesList?.Values?.ToList() ?? new List<string>(),
                SunAnglesAzimuthColStepValue = jsonMetadata.GeometricInfo?.TileAngles?.SunAnglesGrid?.Azimuth?.ColStep ?? 0,
                SunAnglesAzimuthRowStepValue = jsonMetadata.GeometricInfo?.TileAngles?.SunAnglesGrid?.Azimuth?.RowStep ?? 0,
                SunAnglesAzimuthValuesList = jsonMetadata.GeometricInfo?.TileAngles?.SunAnglesGrid?.Azimuth?.ValuesList?.Values?.ToList() ?? new List<string>(),
                // Mean Sun Angle
                MeanSunAngleZenithAngle = jsonMetadata.GeometricInfo?.TileAngles?.MeanSunAngle?.ZenithAngle,
                MeanSunAngleAzimuthAngle = jsonMetadata.GeometricInfo?.TileAngles?.MeanSunAngle?.AzimuthAngle,
                // Viewing Incidence Angles Grids
                ViewingIncidenceAnglesGridsRaw = jsonMetadata.GeometricInfo?.TileAngles?.ViewingIncidenceAnglesGrids != null
                    ? string.Join("~", jsonMetadata.GeometricInfo.TileAngles.ViewingIncidenceAnglesGrids.Select(v => v.ToString()))
                    : null,
                // Mean Viewing Incidence Angle List
                MeanViewingIncidenceAngleListRaw = jsonMetadata.GeometricInfo?.TileAngles?.MeanViewingIncidenceAngleList?.MeanViewingIncidenceAngles != null
                    ? string.Join("~", jsonMetadata.GeometricInfo.TileAngles.MeanViewingIncidenceAngleList.MeanViewingIncidenceAngles.Select(m => m.ToString()))
                    : null,
                // Quality Indicators Info
                CloudyPixelPercentage = double.TryParse(jsonMetadata.QualityIndicatorsInfo?.ImageContentQI?.CloudyPixelPercentage, out var degradedCloudyPixelPercentage) ? degradedCloudyPixelPercentage : 0,
                DegradedDataPercentage = double.TryParse(jsonMetadata.QualityIndicatorsInfo?.ImageContentQI?.DegradedMsiDataPercentage, out var degradedDegradedDataPercentage) ? degradedDegradedDataPercentage : 0,
                SnowPixelPercentage = double.TryParse(jsonMetadata.QualityIndicatorsInfo?.ImageContentQI?.SnowPixelPercentage, out var degradedSnowPixelPercentage) ? degradedSnowPixelPercentage : 0,
                PixelLevelQIRaw = jsonMetadata.QualityIndicatorsInfo?.PixelLevelQI?.ToString(),
                CreateAt = satelliteDate
            };

            return new SentinelTileParseResult
            {
                Metadata = dbModel,
                SatelliteDate = satelliteDate
            };
        }

        /// <summary>
        /// Парсит Product Quality Metadata XML файл
        /// </summary>
        public static async Task<SentinelQualityParseResult> ParseProductQualityMetadataAsync(Stream fileStream)
        {
            // 1. Конвертируем XML в JSON
            var jsonContent = await XmlToJsonConverter.ConvertXmlStreamToJsonAsync(fileStream);
            if (string.IsNullOrWhiteSpace(jsonContent))
                throw new ArgumentException("JSON-содержимое не может быть пустым");

            // 2. Десериализуем в JSON модель
            var jsonMetadata = DeserializeProductQualityMetadataJson(jsonContent);
            if (jsonMetadata == null)
                throw new InvalidOperationException("Не удалось десериализовать JSON в модель Product Quality Metadata");

            // 4. Преобразование в модель базы данных
            var dbModel = new SentinelProductQualityMetadata
            {
                FileName = jsonMetadata.EarthExplorerHeader?.FixedHeader?.FileName,
                FileDescription = jsonMetadata.EarthExplorerHeader?.FixedHeader?.FileDescription,
                Notes = jsonMetadata.EarthExplorerHeader?.FixedHeader?.Notes,
                Mission = jsonMetadata.EarthExplorerHeader?.FixedHeader?.Mission,
                FileClass = jsonMetadata.EarthExplorerHeader?.FixedHeader?.FileClass,
                FileType = jsonMetadata.EarthExplorerHeader?.FixedHeader?.FileType,
                ValidityStart = CleanSentinelDateTimeString(jsonMetadata.EarthExplorerHeader?.FixedHeader?.ValidityPeriod?.ValidityStart),
                ValidityStop = CleanSentinelDateTimeString(jsonMetadata.EarthExplorerHeader?.FixedHeader?.ValidityPeriod?.ValidityStop),
                FileVersion = jsonMetadata.EarthExplorerHeader?.FixedHeader?.FileVersion?.ToString(),
                System = jsonMetadata.EarthExplorerHeader?.FixedHeader?.Source?.System,
                Creator = jsonMetadata.EarthExplorerHeader?.FixedHeader?.Source?.Creator,
                CreatorVersion = jsonMetadata.EarthExplorerHeader?.FixedHeader?.Source?.CreatorVersion,
                CreationDate = jsonMetadata.EarthExplorerHeader?.FixedHeader?.Source?.CreationDate,
                Type = jsonMetadata.DataBlock?.Type,
                GippVersion = jsonMetadata.DataBlock?.Report?.GippVersion,
                GlobalStatus = jsonMetadata.DataBlock?.Report?.GlobalStatus,
                Date = DateTimeHelper.ParseXmlDateTime(jsonMetadata.DataBlock?.Report?.Date),
                ParentId = jsonMetadata.DataBlock?.Report?.CheckList?.ParentId,
                Name = jsonMetadata.DataBlock?.Report?.CheckList?.Name,
                Version = jsonMetadata.DataBlock?.Report?.CheckList?.Version,
                ChecksRaw = jsonMetadata.DataBlock?.Report?.CheckList?.Checks != null
                    ? string.Join("~", jsonMetadata.DataBlock.Report.CheckList.Checks.Select(c => c.ToString()))
                    : null
            };

            return new SentinelQualityParseResult
            {
                Metadata = dbModel
            };
        }

        // Методы десериализации JSON
        private static SentinelL1CProductMetadataRaw DeserializeL1CProductMetadataJson(string jsonContent)
        {
            return System.Text.Json.JsonSerializer.Deserialize<SentinelL1CProductMetadataRaw>(jsonContent);
        }

        private static SentinelInspireMetadataRaw DeserializeInspireMetadataJson(string jsonContent)
        {
            return System.Text.Json.JsonSerializer.Deserialize<SentinelInspireMetadataRaw>(jsonContent);
        }

        private static SentinelL1CTileMetadataRaw DeserializeL1CTileMetadataJson(string jsonContent)
        {
            return System.Text.Json.JsonSerializer.Deserialize<SentinelL1CTileMetadataRaw>(jsonContent);
        }

        private static SentinelProductQualityMetadataRaw DeserializeProductQualityMetadataJson(string jsonContent)
        {
            return System.Text.Json.JsonSerializer.Deserialize<SentinelProductQualityMetadataRaw>(jsonContent);
        }

        private static DateTime ExtractSatelliteDateFromTile(SentinelL1CTileMetadataRaw jsonMetadata)
        {
            return DateTimeHelper.ExtractDateWithFallback(
                jsonMetadata.GeneralInfo?.SensingTime,
                jsonMetadata.GeneralInfo?.ArchivingInfo?.ArchivingTime
            );
        }

        /// <summary>
        /// Очищает строку даты от префиксов Sentinel (UTC=, GMT= и т.д.)
        /// </summary>
        /// <param name="dateTimeString">Исходная строка</param>
        /// <returns>Очищенная строка</returns>
        private static string CleanSentinelDateTimeString(string? dateTimeString)
        {
            if (string.IsNullOrWhiteSpace(dateTimeString))
                return dateTimeString ?? string.Empty;

            var cleaned = dateTimeString.Trim();

            // Префиксы Sentinel для удаления
            var prefixes = new[] { "UTC=", "UTC:", "UTC ", "GMT=", "GMT:", "GMT " };

            foreach (var prefix in prefixes)
            {
                if (cleaned.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    cleaned = cleaned.Substring(prefix.Length).Trim();
                    break;
                }
            }

            return cleaned;
        }

        private static SentinelL1CParseResult ParseFlatL1CProductMetadata(SentinelL1CProductMetadataRaw jsonMetadata)
        {
            // Для плоской структуры просто заполняем основные поля из структурированных данных
            var dbModel = new SentinelL1CProductMetadata
            {
                // Основные поля из General_Info
                ProductStartTime = DateTimeHelper.SafeParseDateTime(jsonMetadata.General_Info?.Product_Info?.PRODUCT_START_TIME),
                ProductStopTime = DateTimeHelper.SafeParseDateTime(jsonMetadata.General_Info?.Product_Info?.PRODUCT_STOP_TIME),
                ProductUri = jsonMetadata.General_Info?.Product_Info?.PRODUCT_URI,
                ProcessingLevel = jsonMetadata.General_Info?.Product_Info?.PROCESSING_LEVEL,
                ProductType = jsonMetadata.General_Info?.Product_Info?.PRODUCT_TYPE,
                ProcessingBaseline = jsonMetadata.General_Info?.Product_Info?.PROCESSING_BASELINE.ToString(),
                ProductDoi = jsonMetadata.General_Info?.Product_Info?.PRODUCT_DOI,
                GenerationTime = DateTimeHelper.SafeParseDateTime(jsonMetadata.General_Info?.Product_Info?.GENERATION_TIME),
                
                // Datatake - используем весь объект Datatake
                DatatakeRaw = jsonMetadata.General_Info?.Product_Info?.Datatake?.ToString() ?? string.Empty,
                
                // Granules - извлекаем пути к файлам изображений
                GranulesRaw = ExtractGranulesFromStructuredData(jsonMetadata) ?? string.Empty,
                
                // Special Values - извлекаем NODATA и SATURATED
                SpecialValuesRaw = ExtractSpecialValuesFromStructuredData(jsonMetadata) ?? string.Empty,
                
                // Image Display Order
                RedChannel = jsonMetadata.General_Info?.Product_Image_Characteristics?.Image_Display_Order?.RED_CHANNEL ?? 0,
                GreenChannel = jsonMetadata.General_Info?.Product_Image_Characteristics?.Image_Display_Order?.GREEN_CHANNEL ?? 0,
                BlueChannel = jsonMetadata.General_Info?.Product_Image_Characteristics?.Image_Display_Order?.BLUE_CHANNEL ?? 0,
                
                // Quantification
                QuantificationValue = jsonMetadata.General_Info?.Product_Image_Characteristics?.QUANTIFICATION_VALUE ?? 0,
                
                // Offsets
                OffsetsRaw = ExtractOffsetsFromStructuredData(jsonMetadata) ?? string.Empty,
                
                // Reflectance
                ReflectanceU = jsonMetadata.General_Info?.Product_Image_Characteristics?.Reflectance_Conversion?.U != null 
                    ? double.TryParse(jsonMetadata.General_Info.Product_Image_Characteristics.Reflectance_Conversion.U.ToString(), out var u) ? u : 0 
                    : 0,
                
                // Solar Irradiance
                SolarIrradianceListRaw = jsonMetadata.General_Info?.Product_Image_Characteristics?.Reflectance_Conversion?.Solar_Irradiance_List?.SOLAR_IRRADIANCE != null
                    ? string.Join("~", jsonMetadata.General_Info.Product_Image_Characteristics.Reflectance_Conversion.Solar_Irradiance_List.SOLAR_IRRADIANCE)
                    : string.Empty,
                
                
                // Reference Band
                ReferenceBand = jsonMetadata.General_Info?.Product_Image_Characteristics?.REFERENCE_BAND ?? 0,
                
                // Geometric Info
                RasterCsType = jsonMetadata.Geometric_Info?.Product_Footprint?.RASTER_CS_TYPE,
                PixelOrigin = jsonMetadata.Geometric_Info?.Product_Footprint?.PIXEL_ORIGIN ?? 0,
                GeoTables = jsonMetadata.Geometric_Info?.Coordinate_Reference_System?.GEO_TABLES,
                HorizontalCsType = jsonMetadata.Geometric_Info?.Coordinate_Reference_System?.HORIZONTAL_CS_TYPE
                
            };

            // Извлекаем дату спутника из DATATAKE_SENSING_START
            var satelliteDate = DateTimeHelper.SafeParseDateTime(jsonMetadata.General_Info?.Product_Info?.Datatake?.DATATAKE_SENSING_START);

            return new SentinelL1CParseResult
            {
                Metadata = dbModel,
                SatelliteDate = satelliteDate
            };
        }

        private static string? ExtractGranulesFromStructuredData(SentinelL1CProductMetadataRaw jsonMetadata)
        {
            if (jsonMetadata.General_Info?.Product_Info?.Product_Organisation?.Granule_List?.Granule?.IMAGE_FILE != null)
            {
                return string.Join("~", jsonMetadata.General_Info.Product_Info.Product_Organisation.Granule_List.Granule.IMAGE_FILE);
            }
            return null;
        }

        private static string? ExtractSpecialValuesFromStructuredData(SentinelL1CProductMetadataRaw jsonMetadata)
        {
            if (jsonMetadata.General_Info?.Product_Image_Characteristics?.Special_Values != null)
            {
                var specialValues = jsonMetadata.General_Info.Product_Image_Characteristics.Special_Values
                    .Select(sv => $"{sv.SPECIAL_VALUE_TEXT}:{sv.SPECIAL_VALUE_INDEX}")
                    .ToArray();
                return string.Join("~", specialValues);
            }
            return null;
        }

        private static string? ExtractOffsetsFromStructuredData(SentinelL1CProductMetadataRaw jsonMetadata)
        {
            if (jsonMetadata.General_Info?.Product_Image_Characteristics?.Radiometric_Offset_List?.RADIO_ADD_OFFSET != null)
            {
                return string.Join("~", jsonMetadata.General_Info.Product_Image_Characteristics.Radiometric_Offset_List.RADIO_ADD_OFFSET);
            }
            return null;
        }

        public static string SimplifyInspireMetadataXml(string xmlContent)
        {
            // 1. Удаляем ВСЕ xmlns и xsi атрибуты (включая их значения), но сохраняем переносы
            string result = System.Text.RegularExpressions.Regex.Replace(
                xmlContent,
                @"\s+(xmlns|xsi)(:\w+)?\s*=\s*""[^""]*""",
                "",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline
            );

            // 2. Удаляем остатки xsi:schemaLocation и подобные (на случай, если остались)
            result = System.Text.RegularExpressions.Regex.Replace(
                result,
                @"\s+xsi:\w+\s*=\s*""[^""]*""",
                "",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline
            );

            // 3. Убираем префиксы у тегов: <gmd:fileIdentifier> → <fileIdentifier>
            result = System.Text.RegularExpressions.Regex.Replace(
                result,
                @"<(\w+):(\w+)",
                "<$2",
                System.Text.RegularExpressions.RegexOptions.Multiline
            );

            result = System.Text.RegularExpressions.Regex.Replace(
                result,
                @"</(\w+):(\w+)",
                "</$2",
                System.Text.RegularExpressions.RegexOptions.Multiline
            );

            // 4. Заменяем gco-обёртки на чистый текст (осторожно: только если внутри нет тегов)
            result = System.Text.RegularExpressions.Regex.Replace(
                result,
                @"<CharacterString>([^<]*)</CharacterString>",
                "$1",
                System.Text.RegularExpressions.RegexOptions.Multiline
            );
            result = System.Text.RegularExpressions.Regex.Replace(result, @"<Date>([^<]*)</Date>", "$1");
            result = System.Text.RegularExpressions.Regex.Replace(result, @"<Decimal>([^<]*)</Decimal>", "$1");
            result = System.Text.RegularExpressions.Regex.Replace(result, @"<Integer>([^<]*)</Integer>", "$1");
            result = System.Text.RegularExpressions.Regex.Replace(result, @"<Boolean>([^<]*)</Boolean>", "$1");

            // 5. Убираем ДВОЙНЫЕ пробелы/переносы ТОЛЬКО внутри тегов, но не между ними
            // Например: <MD_Metadata   > → <MD_Metadata>
            result = System.Text.RegularExpressions.Regex.Replace(
                result,
                @"<(\w+)\s+>",
                "<$1>",
                System.Text.RegularExpressions.RegexOptions.Multiline
            );

            // 6. НЕ удаляем переносы между тегами! (раньше было: @">\s+<" → "><")
            // Теперь оставляем как есть — форматирование сохранится

            string pattern = @"(<TimePeriod)\s+gml:id=""[^""]*""(\s*>)";
            string replacement = "$1$2";
            result = Regex.Replace(result, pattern, replacement);

            return result;
        }
    }

    [XmlRoot("MD_Metadata")]
    public class MD_Metadata
    {
        [XmlElement("fileIdentifier")]
        public string FileIdentifier { get; set; }

        [XmlElement("language")]
        public LanguageCode Language { get; set; }

        [XmlElement("characterSet")]
        public MD_CharacterSetCode CharacterSet { get; set; }

        [XmlElement("hierarchyLevel")]
        public MD_ScopeCode HierarchyLevel { get; set; }

        [XmlElement("contact")]
        public CI_ResponsibleParty Contact { get; set; }

        [XmlElement("dateStamp")]
        public string DateStamp { get; set; }

        [XmlElement("metadataStandardName")]
        public string MetadataStandardName { get; set; }

        [XmlElement("metadataStandardVersion")]
        public string MetadataStandardVersion { get; set; }

        [XmlElement("referenceSystemInfo")]
        public MD_ReferenceSystem ReferenceSystemInfo { get; set; }

        [XmlElement("identificationInfo")]
        public MD_DataIdentification IdentificationInfo { get; set; }

        [XmlElement("distributionInfo")]
        public MD_Distribution DistributionInfo { get; set; }

        [XmlElement("dataQualityInfo")]
        public DQ_DataQuality DataQualityInfo { get; set; }
    }

    public class LanguageCode
    {
        [XmlAttribute("codeList")]
        public string CodeList { get; set; }

        [XmlAttribute("codeListValue")]
        public string CodeListValue { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public class MD_CharacterSetCode
    {
        [XmlAttribute("codeList")]
        public string CodeList { get; set; }

        [XmlAttribute("codeListValue")]
        public string CodeListValue { get; set; }

        [XmlAttribute("codeSpace")]
        public string CodeSpace { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public class MD_ScopeCode
    {
        [XmlAttribute("codeList")]
        public string CodeList { get; set; }

        [XmlAttribute("codeListValue")]
        public string CodeListValue { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public class CI_ResponsibleParty
    {
        [XmlElement("organisationName")]
        public string OrganisationName { get; set; }

        [XmlElement("contactInfo")]
        public CI_Contact ContactInfo { get; set; }

        [XmlElement("role")]
        public CI_RoleCode Role { get; set; }
    }

    public class CI_Contact
    {
        [XmlElement("address")]
        public CI_Address Address { get; set; }
    }

    public class CI_Address
    {
        [XmlElement("electronicMailAddress")]
        public string ElectronicMailAddress { get; set; }
    }

    public class CI_RoleCode
    {
        [XmlAttribute("codeList")]
        public string CodeList { get; set; }

        [XmlAttribute("codeListValue")]
        public string CodeListValue { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public class MD_ReferenceSystem
    {
        [XmlElement("referenceSystemIdentifier")]
        public RS_Identifier ReferenceSystemIdentifier { get; set; }
    }

    public class RS_Identifier
    {
        [XmlElement("code")]
        public string Code { get; set; }

        [XmlElement("codeSpace")]
        public string CodeSpace { get; set; }
    }

    public class MD_DataIdentification
    {
        [XmlElement("citation")]
        public CI_Citation Citation { get; set; }

        [XmlElement("abstract")]
        public string Abstract { get; set; }

        [XmlElement("pointOfContact")]
        public CI_ResponsibleParty PointOfContact { get; set; }

        [XmlElement("descriptiveKeywords")]
        public List<MD_Keywords> DescriptiveKeywords { get; set; }

        //[XmlElement("resourceConstraints", Type = typeof(MD_Constraints))]
        //[XmlElement("resourceConstraints", Type = typeof(MD_LegalConstraints))]
        //public List<object> ResourceConstraints { get; set; }

        [XmlElement("spatialResolution")]
        public MD_Resolution SpatialResolution { get; set; }

        [XmlElement("language")]
        public LanguageCode Language { get; set; }

        [XmlElement("topicCategory")]
        public string TopicCategory { get; set; }

        [XmlElement("extent")]
        public EX_Extent Extent { get; set; }
    }

    public class CI_Citation
    {
        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("date")]
        public CI_DateWrapper Date { get; set; }

        [XmlElement("identifier")]
        public List<RS_Identifier> Identifier { get; set; }
    }

    public class CI_DateWrapper
    {
        [XmlElement("date")]
        public string Date { get; set; }

        [XmlElement("dateType")]
        public CI_DateTypeCode DateType { get; set; }
    }

    public class CI_DateTypeCode
    {
        [XmlAttribute("codeList")]
        public string CodeList { get; set; }

        [XmlAttribute("codeListValue")]
        public string CodeListValue { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public class MD_Keywords
    {
        [XmlElement("keyword")]
        public string[] Keyword { get; set; }

        [XmlElement("thesaurusName")]
        public CI_Citation ThesaurusName { get; set; }
    }

    public class MD_Constraints
    {
        [XmlElement("useLimitation")]
        public string UseLimitation { get; set; }
    }

    public class MD_LegalConstraints
    {
        [XmlElement("accessConstraints")]
        public MD_RestrictionCode AccessConstraints { get; set; }

        [XmlElement("otherConstraints")]
        public string OtherConstraints { get; set; }
    }

    public class MD_RestrictionCode
    {
        [XmlAttribute("codeList")]
        public string CodeList { get; set; }

        [XmlAttribute("codeListValue")]
        public string CodeListValue { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public class MD_Resolution
    {
        [XmlElement("equivalentScale")]
        public MD_RepresentativeFraction EquivalentScale { get; set; }
    }

    public class MD_RepresentativeFraction
    {
        [XmlElement("denominator")]
        public int Denominator { get; set; }
    }

    public class EX_Extent
    {
        [XmlElement("geographicElement")]
        public EX_GeographicBoundingBox GeographicElement { get; set; }

        [XmlElement("temporalElement")]
        public EX_TemporalExtent TemporalElement { get; set; }
    }

    public class EX_GeographicBoundingBox
    {
        [XmlElement("westBoundLongitude")]
        public decimal WestBoundLongitude { get; set; }

        [XmlElement("eastBoundLongitude")]
        public decimal EastBoundLongitude { get; set; }

        [XmlElement("southBoundLatitude")]
        public decimal SouthBoundLatitude { get; set; }

        [XmlElement("northBoundLatitude")]
        public decimal NorthBoundLatitude { get; set; }
    }

    public class EX_TemporalExtent
    {
        [XmlElement("extent")]
        public TimePeriodWrapper Extent { get; set; }
    }

    public class TimePeriodWrapper
    {
        [XmlElement("TimePeriod")]
        public TimePeriod TimePeriod { get; set; }
    }

    public class TimePeriod
    {
        // gml:id игнорируется — не объявляем атрибут
        [XmlElement("beginPosition")]
        public DateTime BeginPosition { get; set; }

        [XmlElement("endPosition")]
        public DateTime EndPosition { get; set; }
    }

    public class MD_Distribution
    {
        [XmlElement("distributionFormat")]
        public MD_Format DistributionFormat { get; set; }

        [XmlElement("transferOptions")]
        public MD_DigitalTransferOptions TransferOptions { get; set; }
    }

    public class MD_Format
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("version")]
        public string Version { get; set; }
    }

    public class MD_DigitalTransferOptions
    {
        [XmlElement("onLine")]
        public CI_OnlineResource OnLine { get; set; }
    }

    public class CI_OnlineResource
    {
        [XmlElement("linkage")]
        public string Linkage { get; set; }
    }

    public class DQ_DataQuality
    {
        [XmlElement("scope")]
        public DQ_Scope Scope { get; set; }

        [XmlElement("report")]
        public DQ_DomainConsistency Report { get; set; }

        [XmlElement("lineage")]
        public LI_Lineage Lineage { get; set; }
    }

    public class DQ_Scope
    {
        [XmlElement("level")]
        public MD_ScopeCode Level { get; set; }
    }

    public class DQ_DomainConsistency
    {
        [XmlElement("result")]
        public DQ_ConformanceResult Result { get; set; }
    }

    public class DQ_ConformanceResult
    {
        [XmlElement("specification")]
        public CI_Citation Specification { get; set; }

        [XmlElement("explanation")]
        public string Explanation { get; set; }

        [XmlElement("pass")]
        public bool Pass { get; set; }
    }

    public class LI_Lineage
    {
        [XmlElement("statement")]
        public string Statement { get; set; }
    }
}