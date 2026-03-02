namespace Masofa.Common.Models.Era
{
    public class EraWeatherStation : BaseEntity
    {
        /// <summary>
        /// Координаты точки
        /// </summary>
        public required NetTopologySuite.Geometries.Point Point { get; set; }

        /// <summary>
        /// Привязка к региону наименьшей площади
        /// </summary>
        public Guid RegionId { get; set; }

        /// <summary>
        /// Привязка к полю наименьшей площади
        /// </summary>
        public Guid FieldId { get; set; }

        /// <summary>
        /// Идентификатор агроклиматической зоны
        /// </summary>
        public Guid? AgroclimaticZoneId { get; set; }
    }
}
