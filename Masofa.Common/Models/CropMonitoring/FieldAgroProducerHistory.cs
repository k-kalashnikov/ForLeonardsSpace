namespace Masofa.Common.Models.CropMonitoring
{
    public class FieldAgroProducerHistory : BaseEntity
    {
        public Guid FieldId { get; set; }
        public Guid AgricultureProducerId { get; set; }
        public DateOnly PeriodStart { get; set; }
        public DateOnly PeriodEnd { get; set; }
    }

    public class FieldAgroProducerHistoryHistory : BaseHistoryEntity<FieldAgroProducerHistory> { }
}
