namespace Masofa.Common.Models.CropMonitoring
{
    /// <summary>
    /// Лог преобразования загружаемого поля в посев
    /// </summary>
    public class ImportedFieldLog : BaseEntity
    {
        /// <summary>
        /// Идентификатор посева
        /// </summary>
        public Guid SeasonId { get; set; }

        /// <summary>
        /// Пересекаемые посевы
        /// </summary>
        public List<Guid> IntersectedSeasons { get; set; } = [];

        /// <summary>
        /// Пересекаемые посевы
        /// </summary>
        public List<Guid> CoveredSeasons { get; set; } = [];

        /// <summary>
        /// Пересекаемые посевы
        /// </summary>
        public List<Guid> CoveredBySeasons { get; set; } = [];

        /// <summary>
        /// Пересекаемые посевы
        /// </summary>
        public List<Guid> EqualPolygonsSeasons { get; set; } = [];


    }
}
