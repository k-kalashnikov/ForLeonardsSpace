using Masofa.Common.Attributes;

namespace Masofa.Common.Models.Satellite.Indices
{
    [PartitionedTable]
    public class OsaviPoint : BaseIndexPoint
    {
        public float BZero8 { get; set; }

        public float BZero4 { get; set; }

        public float EPS { get; set; }

        public float OSAVI_X = 0.16f;

        public override float Value
        {
            get
            {
                if (BZero8 + BZero4 == 0)
                {
                    EPS = 1e-9f;
                }

                return ((BZero8 - BZero4) / (BZero8 + BZero4 + OSAVI_X + EPS));
            }
        }
    }

    [PartitionedTable]
    public class OsaviPolygon : BaseIndexPolygon 
    { 

    }

    public class OsaviPolygonRelation
    {
        public Guid Id { get; set; }

        public Guid OsaViId { get; set; }
        public Guid? RegionId { get; set; }
        public Guid? FieldId { get; set; }
        public Guid? SeasonId { get; set; }
    }
}
