using Masofa.Common.Models;

namespace Masofa.BusinessLogic.CropMonitoring
{
    public class BoundingBoxRequest<TModel> where TModel : BaseEntity
    {
        public decimal West { get; set; }
        public decimal East { get; set; }
        public decimal South { get; set; }
        public decimal North { get; set; }
        public BaseGetQuery<TModel> Query { get; set; } = new BaseGetQuery<TModel>();
    }
}
