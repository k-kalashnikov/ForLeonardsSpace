using Masofa.Common.Attributes;

namespace Masofa.Common.Models.Satellite.Indices
{
    [PartitionedTable]
    public class ArviPoint : BaseIndexPoint
    {
        public float BZero8 { get; set; }
        public float BZero4 { get; set; }
        public float? BZero2 { get; set; }

        public override float Value
        {
            get
            {
                var blue = BZero2 ?? 0f;
                return ((BZero8 - (2 * BZero4) + blue) / (BZero8 + (2 * BZero4) + blue));
            }
        }
    }

    [PartitionedTable]
    public class ArviPolygon : BaseIndexPolygon
    {

    }

    public class ArviPolygonRelation
    {
        public Guid Id { get; set; }

        public Guid ArviId { get; set; }
        public Guid? RegionId { get; set; }
        public Guid? FieldId { get; set; }
        public Guid? SeasonId { get; set; }
    }
}
