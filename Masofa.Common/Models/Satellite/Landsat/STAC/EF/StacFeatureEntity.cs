using Masofa.Common.Attributes;
using Masofa.Common.Models.Satellite.Parse.Landsat.STAC;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Landsat
{
    /// <summary>
    /// Сущность для хранения основных метаданных STAC в базе данных
    /// </summary>
    [PartitionedTable]
    [Table("StacFeatures")]
    public class StacFeatureEntity : BaseEntity
    {
        /// <summary>
        /// Тип объекта, обычно "Feature"
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = default!;

        /// <summary>
        /// Версия STAC
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string StacVersion { get; set; } = default!;

        /// <summary>
        /// Расширения STAC, сохраненные в виде JSON
        /// </summary>
        [Column(TypeName = "text")]
        public string StacExtensionsJson { get; set; } = default!;

        /// <summary>
        /// Идентификатор объекта
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string FeatureId { get; set; } = default!;

        /// <summary>
        /// Описание объекта
        /// </summary>
        [MaxLength(1000)]
        public string Description { get; set; } = default!;

        /// <summary>
        /// Ограничивающий прямоугольник [запад, юг, восток, север], сохраненный в виде JSON
        /// </summary>
        [Column(TypeName = "text")]
        public string BoundingBoxJson { get; set; } = default!;

        /// <summary>
        /// Геометрия объекта, сохраненная в виде JSON
        /// </summary>
        [Column(TypeName = "text")]
        public string GeometryJson { get; set; } = default!;

        /// <summary>
        /// Дата и время съемки
        /// </summary>
        [Required]
        public DateTime Datetime { get; set; }

        /// <summary>
        /// Процент облачности (0-100)
        /// </summary>
        public double CloudCover { get; set; }

        /// <summary>
        /// Азимут солнца в градусах
        /// </summary>
        public double SunAzimuth { get; set; }

        /// <summary>
        /// Высота солнца в градусах
        /// </summary>
        public double SunElevation { get; set; }

        /// <summary>
        /// Название платформы (например, "LANDSAT_9")
        /// </summary>
        [MaxLength(50)]
        public string Platform { get; set; } = default!;

        /// <summary>
        /// Инструменты, использованные для сбора данных, сохраненные в виде JSON
        /// </summary>
        [Column(TypeName = "text")]
        public string InstrumentsJson { get; set; } = default!;

        /// <summary>
        /// Угол отклонения от надира в градусах
        /// </summary>
        public int OffNadir { get; set; }

        /// <summary>
        /// Процент облачности над сушей (0-100)
        /// </summary>
        public double CloudCoverLand { get; set; }

        /// <summary>
        /// Тип WRS (например, "2")
        /// </summary>
        [MaxLength(10)]
        public string WrsType { get; set; } = default!;

        /// <summary>
        /// Путь WRS
        /// </summary>
        [MaxLength(10)]
        public string WrsPath { get; set; } = default!;

        /// <summary>
        /// Ряд WRS
        /// </summary>
        [MaxLength(10)]
        public string WrsRow { get; set; } = default!;

        /// <summary>
        /// Идентификатор сцены
        /// </summary>
        [MaxLength(255)]
        public string SceneId { get; set; } = default!;

        /// <summary>
        /// Категория коллекции (например, "T1")
        /// </summary>
        [MaxLength(50)]
        public string CollectionCategory { get; set; } = default!;

        /// <summary>
        /// Номер коллекции (например, "02")
        /// </summary>
        [MaxLength(10)]
        public string CollectionNumber { get; set; } = default!;

        /// <summary>
        /// Уровень коррекции (например, "L2SP")
        /// </summary>
        [MaxLength(50)]
        public string Correction { get; set; } = default!;

        /// <summary>
        /// Смещение по X
        /// </summary>
        public int GeometricXBias { get; set; }

        /// <summary>
        /// Смещение по Y
        /// </summary>
        public int GeometricYBias { get; set; }

        /// <summary>
        /// Стандартное отклонение по X
        /// </summary>
        public double GeometricXStddev { get; set; }

        /// <summary>
        /// Стандартное отклонение по Y
        /// </summary>
        public double GeometricYStddev { get; set; }

        /// <summary>
        /// Среднеквадратическая ошибка геометрии
        /// </summary>
        public double GeometricRmse { get; set; }

        /// <summary>
        /// Код EPSG для проекции
        /// </summary>
        public int ProjEpsg { get; set; }

        /// <summary>
        /// Форма изображения [высота, ширина], сохраненная в виде JSON
        /// </summary>
        [Column(TypeName = "text")]
        public string ProjShapeJson { get; set; } = default!;

        /// <summary>
        /// Матрица трансформации для проекции, сохраненная в виде JSON
        /// </summary>
        [Column(TypeName = "text")]
        public string ProjTransformJson { get; set; } = default!;

        /// <summary>
        /// Спецификация CARD4L (например, "ST" для температуры поверхности, "SR" для отражательной способности поверхности)
        /// </summary>
        [MaxLength(50)]
        public string Card4lSpecification { get; set; } = default!;

        /// <summary>
        /// Версия спецификации CARD4L
        /// </summary>
        [MaxLength(50)]
        public string Card4lSpecificationVersion { get; set; } = default!;

        /// <summary>
        /// Идентификатор коллекции
        /// </summary>
        [MaxLength(255)]
        public string Collection { get; set; } = default!;

        /// <summary>
        /// Тип продукта (SR - Surface Reflectance, ST - Surface Temperature)
        /// </summary>
        [MaxLength(10)]
        public ProductTypeEnum ProductType { get; set; } = default!;

        #region Asset References

        #region Metadata Assets
        /// <summary>
        /// Идентификатор ассета метаданных MTL.json
        /// </summary>
        public Guid? MtlJsonAssetId { get; set; }

        /// <summary>
        /// Идентификатор ассета метаданных MTL.txt
        /// </summary>
        public Guid? MtlTxtAssetId { get; set; }

        /// <summary>
        /// Идентификатор ассета метаданных MTL.xml
        /// </summary>
        public Guid? MtlXmlAssetId { get; set; }

        /// <summary>
        /// Идентификатор ассета метаданных ANG.txt
        /// </summary>
        public Guid? AngTxtAssetId { get; set; }

        /// <summary>
        /// Идентификатор ассета миниатюры
        /// </summary>
        public Guid? ThumbnailAssetId { get; set; }

        /// <summary>
        /// Идентификатор ассета уменьшенного изображения
        /// </summary>
        public Guid? ReducedResolutionBrowseAssetId { get; set; }
        #endregion

        #region Surface Reflectance (SR) Assets
        /// <summary>
        /// Идентификатор ассета прибрежного канала (B1)
        /// </summary>
        public Guid? CoastalAssetId { get; set; }

        /// <summary>
        /// Идентификатор ассета синего канала (B2)
        /// </summary>
        public Guid? BlueAssetId { get; set; }

        /// <summary>
        /// Идентификатор ассета зеленого канала (B3)
        /// </summary>
        public Guid? GreenAssetId { get; set; }

        /// <summary>
        /// Идентификатор ассета красного канала (B4)
        /// </summary>
        public Guid? RedAssetId { get; set; }

        /// <summary>
        /// Идентификатор ассета ближнего инфракрасного канала (B5)
        /// </summary>
        public Guid? Nir08AssetId { get; set; }

        /// <summary>
        /// Идентификатор ассета коротковолнового инфракрасного канала 1.6 (B6)
        /// </summary>
        public Guid? Swir16AssetId { get; set; }

        /// <summary>
        /// Идентификатор ассета коротковолнового инфракрасного канала 2.2 (B7)
        /// </summary>
        public Guid? Swir22AssetId { get; set; }

        /// <summary>
        /// Идентификатор ассета качества аэрозолей
        /// </summary>
        public Guid? QaAerosolAssetId { get; set; }
        #endregion

        #region Surface Temperature (ST) Assets
        /// <summary>
        /// Идентификатор ассета температуры поверхности (B10)
        /// </summary>
        public Guid? Lwir11AssetId { get; set; }

        /// <summary>
        /// Идентификатор ассета теплового излучения
        /// </summary>
        public Guid? TradAssetId { get; set; }

        /// <summary>
        /// Идентификатор ассета восходящего излучения
        /// </summary>
        public Guid? UradAssetId { get; set; }

        /// <summary>
        /// Идентификатор ассета нисходящего излучения
        /// </summary>
        public Guid? DradAssetId { get; set; }

        /// <summary>
        /// Идентификатор ассета атмосферного пропускания
        /// </summary>
        public Guid? AtranAssetId { get; set; }

        /// <summary>
        /// Идентификатор ассета излучательной способности
        /// </summary>
        public Guid? EmisAssetId { get; set; }

        /// <summary>
        /// Идентификатор ассета стандартного отклонения излучательной способности
        /// </summary>
        public Guid? EmsdAssetId { get; set; }

        /// <summary>
        /// Идентификатор ассета расстояния до облаков
        /// </summary>
        public Guid? CdistAssetId { get; set; }

        /// <summary>
        /// Идентификатор ассета качества температуры поверхности
        /// </summary>
        public Guid? QaAssetId { get; set; }
        #endregion

        #region Common Quality Assets
        /// <summary>
        /// Идентификатор ассета качества пикселей
        /// </summary>
        public Guid? QaPixelAssetId { get; set; }

        /// <summary>
        /// Идентификатор ассета качества радиометрического насыщения
        /// </summary>
        public Guid? QaRadsatAssetId { get; set; }
        #endregion

        #endregion

        /// <summary>
        /// Преобразует StacFeature в StacFeatureEntity
        /// </summary>
        /// <param name="stacFeature">Объект StacFeature</param>
        /// <param name="productType">Тип продукта (SR или ST)</param>
        /// <returns>Объект StacFeatureEntity</returns>
        public static StacFeatureEntity FromStacFeature(StacFeature stacFeature, ProductTypeEnum productType)
        {
            return new StacFeatureEntity
            {
                Type = stacFeature.Type,
                StacVersion = stacFeature.StacVersion,
                StacExtensionsJson = Newtonsoft.Json.JsonConvert.SerializeObject(stacFeature.StacExtensions),
                FeatureId = stacFeature.Id,
                Description = stacFeature.Description,
                BoundingBoxJson = Newtonsoft.Json.JsonConvert.SerializeObject(stacFeature.BoundingBox),
                GeometryJson = Newtonsoft.Json.JsonConvert.SerializeObject(stacFeature.Geometry),
                Datetime = stacFeature.Properties.Datetime,
                CloudCover = stacFeature.Properties.CloudCover,
                SunAzimuth = stacFeature.Properties.SunAzimuth,
                SunElevation = stacFeature.Properties.SunElevation,
                Platform = stacFeature.Properties.Platform,
                InstrumentsJson = Newtonsoft.Json.JsonConvert.SerializeObject(stacFeature.Properties.Instruments),
                OffNadir = stacFeature.Properties.OffNadir,
                CloudCoverLand = stacFeature.Properties.CloudCoverLand,
                WrsType = stacFeature.Properties.WrsType,
                WrsPath = stacFeature.Properties.WrsPath,
                WrsRow = stacFeature.Properties.WrsRow,
                SceneId = stacFeature.Properties.SceneId,
                CollectionCategory = stacFeature.Properties.CollectionCategory,
                CollectionNumber = stacFeature.Properties.CollectionNumber,
                Correction = stacFeature.Properties.Correction,
                GeometricXBias = stacFeature.Properties.GeometricXBias,
                GeometricYBias = stacFeature.Properties.GeometricYBias,
                GeometricXStddev = stacFeature.Properties.GeometricXStddev,
                GeometricYStddev = stacFeature.Properties.GeometricYStddev,
                GeometricRmse = stacFeature.Properties.GeometricRmse,
                ProjEpsg = stacFeature.Properties.ProjEpsg,
                ProjShapeJson = Newtonsoft.Json.JsonConvert.SerializeObject(stacFeature.Properties.ProjShape),
                ProjTransformJson = Newtonsoft.Json.JsonConvert.SerializeObject(stacFeature.Properties.ProjTransform),
                Card4lSpecification = stacFeature.Properties.Card4lSpecification,
                Card4lSpecificationVersion = stacFeature.Properties.Card4lSpecificationVersion,
                Collection = stacFeature.Collection,
                ProductType = productType
            };
        }
    }

    public enum ProductTypeEnum
    {
        /// <summary>
        /// Surface Reflectance
        /// </summary>
        SR,

        /// <summary>
        /// Surface Temperature
        /// </summary>
        ST
    }

    public class StacFeatureEntityHistory : BaseHistoryEntity<StacFeatureEntity> { }
}
