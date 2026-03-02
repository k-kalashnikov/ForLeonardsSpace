using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;
using NetTopologySuite.Geometries;

namespace Masofa.Web.Monolith.ViewModels.Bids
{
    public class BidCreateViewModel
    {
        public Guid? ParentId { get; set; }

        public Guid BidTypeId { get; set; }

        public Guid? ForemanId { get; set; }

        public Guid? WorkerId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime DeadlineDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public Guid? FieldId { get; set; }
        
        public Guid? RegionId { get; set; }
        
        public Guid CropId { get; set; }

        public Guid? VarietyId { get; set; }

        public string? Comment { get; set; }

        public string? Description { get; set; }

        public double Lat { get; set; }

        public double Lng { get; set; }

        public long Number { get; set; }

        public DateTime? FieldPlantingDate { get; set; }
        
        public string? Customer { get; set; }

        public Guid? FileResultId { get; set; }

        public Guid BidTemplateId { get; set; }
        
        public bool IsUnvalidBid { get; set; }
        
        public BidStateType BidState { get; set; } = BidStateType.New;
        
        public StatusType Status { get; set; } = StatusType.Active;

        public string? Polygon { get; set; } = string.Empty;

        public bool? Publish { get; set; } = true;

        public bool? Archive { get; set; }
    }

    //public DateTime DeadLineDate { get; set; }

    //public string Customer { get; set; } = default!;

    //public Guid? FieldId { get; set; }

    //public Guid? RegionId { get; set; }

    //public DateTime? FieldPlantingDate { get; set; }

    //public double Lat { get; set; }

    //public double Lng { get; set; }

    //public string? Description { get; set; }

    //public string? Comment { get; set; }

    //public bool IsPublished { get; set; }
}
