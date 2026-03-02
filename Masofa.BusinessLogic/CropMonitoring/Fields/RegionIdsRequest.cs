using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;

namespace Masofa.BusinessLogic.CropMonitoring.Fields
{
    public class RegionIdsRequest<TField> where TField : BaseEntity
    {
        public List<Guid> RegionIds { get; set; } = [];
        public BaseGetQuery<TField> Query { get; set; } = new BaseGetQuery<TField>();
    }
}
