using Masofa.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Common.Models.Satellite.Indices
{
    [PartitionedTable]
    public class GndviPoint : BaseIndexPoint
    {
        public float BZero8 { get; set; }
        public float BZero3 { get; set; }
        public float EPS { get; set; }
        public override float Value
        {
            get
            {
                if (BZero8 + BZero3 == 0)
                {
                    EPS = 1e-9f;
                }

                return ((BZero8 - BZero3) / (BZero8 + BZero3 + EPS));
            }
        }
    }

    [PartitionedTable]
    public class GndviPolygon : BaseIndexPolygon
    {

    }

    public class GndviPolygonRelation
    {
        public Guid Id { get; set; }

        public Guid GNdviId { get; set; }
        public Guid? RegionId { get; set; }
        public Guid? FieldId { get; set; }
        public Guid? SeasonId { get; set; }
    }
}
