using Masofa.Common.Attributes;

namespace Masofa.Common.Models.Satellite.Indices
{
    [PartitionedTable]
    public class NdwiPoint : BaseIndexPoint
    {
        public float BZero3 { get; set; }
        public float BZero8 { get; set; }
        public float EPS { get; set; }
        public override float Value
        {
            get
            {
                if (BZero8 + BZero3 == 0)
                {
                    EPS = 1e-9f;
                }

                return ((BZero3 - BZero8) / (BZero3 + BZero8 + EPS));
            }
        }
    }

    public class NdwiPolygon : BaseIndexPolygon { }

    public class NdwiPolygonRelation
    {
        public Guid Id { get; set; }

        public Guid NdwiId { get; set; }
        public Guid? RegionId { get; set; }
        public Guid? FieldId { get; set; }
        public Guid? SeasonId { get; set; }
    }
}
