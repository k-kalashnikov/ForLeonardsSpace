using NetTopologySuite.Geometries;

namespace Masofa.Common.Models.CropMonitoring
{
    /// <summary>
    /// Анализ профиля почвы поля
    /// </summary>
    public class FieldSoilProfileAnalysis : BaseEntity
    {
        /// <summary>
        /// Идентификатор поля
        /// </summary>
        public Guid FieldId { get; set; }

        /// <summary>
        /// Дата анализа
        /// </summary>
        public DateOnly AnalysisDate { get; set; }

        /// <summary>
        /// Координаты точки анализа
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Point? Coordinates { get; set; }

        /// <summary>
        /// Результаты анализа (текстовое описание)
        /// </summary>
        public string? AnalysisResults { get; set; }

        /// <summary>
        /// Глубина плодородного слоя (метры)
        /// </summary>
        public double? FertileLayerDepth { get; set; }

        /// <summary>
        /// Гумус, % (плодородность)
        /// </summary>
        public double? Humus { get; set; }

        /// <summary>
        /// Кислотность (pH)
        /// </summary>
        public decimal? Ph { get; set; }

        /// <summary>
        /// Засоление почвы (+осолонцевание-sodicity-накопление обменного натрия)
        /// </summary>
        public double? SoilSalinity { get; set; }

        /// <summary>
        /// Хлоридное засоление
        /// </summary>
        public double? ChlorideSalinity { get; set; }

        /// <summary>
        /// Сульфатное засоление
        /// </summary>
        public double? SulfateSalinity { get; set; }

        /// <summary>
        /// Карбонатное засоление
        /// </summary>
        public double? CarbonateSalinity { get; set; }

        /// <summary>
        /// Макроэлементы (общий показатель)
        /// </summary>
        public double? Macronutrients { get; set; }

        /// <summary>
        /// Азот
        /// </summary>
        public double? Nitrogen { get; set; }

        /// <summary>
        /// Фосфор
        /// </summary>
        public double? Phosphorus { get; set; }

        /// <summary>
        /// Калий
        /// </summary>
        public double? Potassium { get; set; }

        /// <summary>
        /// Сера
        /// </summary>
        public double? Sulfur { get; set; }

        /// <summary>
        /// Микроэлементы (общий показатель)
        /// </summary>
        public double? Micronutrients { get; set; }

        /// <summary>
        /// Выводы по результатам анализа - ссылка на справочник SoilProfileAnalysis
        /// </summary>
        public Guid? SoilProfileAnalysisId { get; set; }

        /// <summary>
        /// Отнесение к виду почвы (тип и структура почвы)
        /// </summary>
        public double? SoilClassification { get; set; }

        /// <summary>
        /// Балл бонитета
        /// </summary>
        public double? BonitScore { get; set; }
    }

    public class FieldSoilProfileAnalysisHistory : BaseHistoryEntity<FieldSoilProfileAnalysis> { }
}

