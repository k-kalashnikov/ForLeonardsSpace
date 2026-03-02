using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.CropMonitoring
{
    /// <summary>
    /// Представляет аграрный сезон, связанный с определённым полем и культурой.
    /// </summary>
    /// <remarks>
    /// Содержит информацию о сроках посева, сбора урожая и характеристиках урожайности.
    /// </remarks>
    public partial class Season : BaseEntity
    {
        /// <summary>
        /// Дата начала сезона (например, дата подготовки почвы или начала наблюдений).
        /// </summary>
        public DateOnly? StartDate { get; set; }

        /// <summary>
        /// Дата окончания сезона.
        /// </summary>
        public DateOnly? EndDate { get; set; }

        /// <summary>
        /// Заголовок или название сезона (например, «Весна 2025»).
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Фактическая дата посева культуры.
        /// </summary>
        public DateOnly? PlantingDate { get; set; }

        /// <summary>
        /// Фактическая дата сбора урожая.
        /// </summary>
        public DateOnly? HarvestingDate { get; set; }

        /// <summary>
        /// Идентификатор поля, к которому относится сезон.
        /// </summary>
        public Guid? FieldId { get; set; }

        /// <summary>
        /// Широта географического расположения поля.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Долгота географического расположения поля.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Идентификатор культуры (например, пшеница, кукуруза).
        /// <see cref="Masofa.Common.Models.Dictionaries.Crop"></see>
        /// </summary>
        public Guid? CropId { get; set; }

        /// <summary>
        /// Идентификатор сорта культуры (например, конкретный сорт пшеницы).
        /// <see cref="Masofa.Common.Models.Dictionaries.Variety"></see>
        /// </summary>
        public Guid? VarietyId { get; set; }

        /// <summary>
        /// Планируемая дата посева.
        /// </summary>
        public DateOnly? PlantingDatePlan { get; set; }

        /// <summary>
        /// Площадь поля в гектарах (на момент сезона).
        /// </summary>
        public double? FieldArea { get; set; }

        /// <summary>
        /// Планируемая дата сбора урожая.
        /// </summary>
        public DateOnly? HarvestingDatePlan { get; set; }

        /// <summary>
        /// Урожайность в гектар (фактическая, т/га или др.).
        /// </summary>
        public double? YieldHaFact { get; set; }

        /// <summary>
        /// Общий объём урожая (фактический, в тоннах или других единицах).
        /// </summary>
        public double? YieldFact { get; set; }

        /// <summary>
        /// Урожайность в гектар (планируемая, т/га или др.).
        /// </summary>
        public double? YieldHaPlan { get; set; }

        /// <summary>
        /// Общий объём урожая (планируемая, в тоннах или других единицах).
        /// </summary>
        public double? YieldPlan { get; set; }

        /// <summary>
        /// Дата обновления расчета ИИ.
        /// </summary>
        public DateTime? CalculationUpdateDate { get; set; }

        /// <summary>
        /// Дата начала периода мониторинга.
        /// </summary>
        public DateOnly? MonitoringPerioidStart { get; set; }

        /// <summary>
        /// Дата окончания периода мониторинга.
        /// </summary>
        public DateOnly? MonitoringPerioidEnd { get; set; }

        /// <summary>
        /// Интервал мониторинга.
        /// </summary>
        public int? MonitoringInterval { get; set; }

        /// <summary>
        /// Тип корневой системы
        /// </summary>
        public Guid? RootSystemTypeId { get; set; }

        /// <summary>
        /// Дата начала гос. контракта
        /// </summary>
        public DateOnly? GovernmentContractStartDate { get; set; }

        /// <summary>
        /// Дата конца гос. контракта
        /// </summary>
        public DateOnly? GovernmentContractEndDate { get; set; }

        /// <summary>
        /// Номер гос. контракта
        /// </summary>
        public string? GovernmentContractNumber { get; set; }

        /// <summary>
        /// Координаты границ посева.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Polygon? Polygon { get; set; }

        [NotMapped]
        public string? PolygonJson
        {
            get
            {
                return (Polygon == null || Polygon.IsEmpty) ? null : Polygon.AsText();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Polygon = null;
                }
                else
                {
                    try
                    {
                        var reader = new NetTopologySuite.IO.WKTReader();
                        var geometry = reader.Read(value);
                        if (geometry is Polygon polygon)
                        {
                            Polygon = polygon;
                        }
                        else
                        {
                            Polygon = null;
                        }
                    }
                    catch
                    {
                        Polygon = null;
                    }
                }
            }
        }
    }

    public class SeasonHistory : BaseHistoryEntity<Season> { }
}
