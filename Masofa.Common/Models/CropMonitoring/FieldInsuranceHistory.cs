namespace Masofa.Common.Models.CropMonitoring
{
    public class FieldInsuranceHistory : BaseEntity
    {
        public Guid FieldId { get; set; }
        public Guid AgricultureProducerId { get; set; }
        public DateOnly PeriodStart { get; set; }
        public DateOnly PeriodEnd { get; set; }
        public double? SumInsured { get; set; }
        public double? InsurancePremium { get; set; }
        public double? Payments { get; set; }
        public string? Comment { get; set; }
    }

    public class FieldInsuranceHistoryHistory : BaseHistoryEntity<FieldInsuranceHistory> { }
}
