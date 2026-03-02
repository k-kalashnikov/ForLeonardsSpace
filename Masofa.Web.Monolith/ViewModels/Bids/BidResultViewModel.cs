using System.ComponentModel.DataAnnotations;

namespace Masofa.Web.Monolith.ViewModels.Bids
{
    public class BidResultViewModel
    {
        [Required]
        public Guid BidId { get; set; }
        
        [Required]
        public IFormFile BidResult { get; set; }
    }
}
