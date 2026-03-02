using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Parse.Landsat.STAC
{
    /// <summary>
    /// Представляет секцию свойств объекта STAC
    /// </summary>
    public class StacProperties
    {
        /// <summary>
        /// Дата и время получения сцены
        /// </summary>
        [JsonPropertyName("datetime")]
        public DateTime Datetime { get; set; }

        /// <summary>
        /// Процент облачного покрытия (0-100)
        /// </summary>
        [JsonPropertyName("eo:cloud_cover")]
        public double CloudCover { get; set; }

        /// <summary>
        /// Азимут солнца в градусах
        /// </summary>
        [JsonPropertyName("view:sun_azimuth")]
        public double SunAzimuth { get; set; }

        /// <summary>
        /// Угол возвышения солнца в градусах
        /// </summary>
        [JsonPropertyName("view:sun_elevation")]
        public double SunElevation { get; set; }

        /// <summary>
        /// Название платформы (например, "LANDSAT_9")
        /// </summary>
        [JsonPropertyName("platform")]
        public string Platform { get; set; } = default!;

        /// <summary>
        /// Инструменты, используемые для сбора данных
        /// </summary>
        [JsonPropertyName("instruments")]
        public List<string> Instruments { get; set; } = default!;

        /// <summary>
        /// Угол отклонения от надира в градусах
        /// </summary>
        [JsonPropertyName("view:off_nadir")]
        public int OffNadir { get; set; }

        /// <summary>
        /// Процент облачного покрытия над сушей (0-100)
        /// </summary>
        [JsonPropertyName("landsat:cloud_cover_land")]
        public double CloudCoverLand { get; set; }

        /// <summary>
        /// Тип WRS (например, "2")
        /// </summary>
        [JsonPropertyName("landsat:wrs_type")]
        public string WrsType { get; set; } = default!;

        /// <summary>
        /// Путь WRS
        /// </summary>
        [JsonPropertyName("landsat:wrs_path")]
        public string WrsPath { get; set; } = default!;

        /// <summary>
        /// Ряд WRS
        /// </summary>
        [JsonPropertyName("landsat:wrs_row")]
        public string WrsRow { get; set; } = default!;

        /// <summary>
        /// Идентификатор сцены
        /// </summary>
        [JsonPropertyName("landsat:scene_id")]
        public string SceneId { get; set; } = default!;

        /// <summary>
        /// Категория коллекции (например, "T1")
        /// </summary>
        [JsonPropertyName("landsat:collection_category")]
        public string CollectionCategory { get; set; } = default!;

        /// <summary>
        /// Номер коллекции (например, "02")
        /// </summary>
        [JsonPropertyName("landsat:collection_number")]
        public string CollectionNumber { get; set; } = default!;

        /// <summary>
        /// Уровень коррекции (например, "L2SP")
        /// </summary>
        [JsonPropertyName("landsat:correction")]
        public string Correction { get; set; } = default!;

        /// <summary>
        /// Геометрическое смещение по X
        /// </summary>
        [JsonPropertyName("accuracy:geometric_x_bias")]
        public int GeometricXBias { get; set; }

        /// <summary>
        /// Геометрическое смещение по Y
        /// </summary>
        [JsonPropertyName("accuracy:geometric_y_bias")]
        public int GeometricYBias { get; set; }

        /// <summary>
        /// Стандартное отклонение по X
        /// </summary>
        [JsonPropertyName("accuracy:geometric_x_stddev")]
        public double GeometricXStddev { get; set; }

        /// <summary>
        /// Стандартное отклонение по Y
        /// </summary>
        [JsonPropertyName("accuracy:geometric_y_stddev")]
        public double GeometricYStddev { get; set; }

        /// <summary>
        /// Среднеквадратическая ошибка геометрии
        /// </summary>
        [JsonPropertyName("accuracy:geometric_rmse")]
        public double GeometricRmse { get; set; }

        /// <summary>
        /// Код EPSG для проекции
        /// </summary>
        [JsonPropertyName("proj:epsg")]
        public int ProjEpsg { get; set; }

        /// <summary>
        /// Размеры изображения [высота, ширина]
        /// </summary>
        [JsonPropertyName("proj:shape")]
        public List<int> ProjShape { get; set; } = default!;

        /// <summary>
        /// Матрица трансформации для проекции
        /// </summary>
        [JsonPropertyName("proj:transform")]
        public List<double> ProjTransform { get; set; } = default!;

        /// <summary>
        /// Спецификация CARD4L (например, "ST" для температуры поверхности, "SR" для отражательной способности поверхности)
        /// </summary>
        [JsonPropertyName("card4l:specification")]
        public string Card4lSpecification { get; set; } = default!;

        /// <summary>
        /// Версия спецификации CARD4L
        /// </summary>
        [JsonPropertyName("card4l:specification_version")]
        public string Card4lSpecificationVersion { get; set; } = default!;
    }
}
