using Masofa.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Common.Models.Satellite.Indices
{
    [PartitionedTable]
    public class MndwiPoint : BaseIndexPoint
    {
        public float BZero3 { get; set; }
        public float B11 { get; set; }
        public float EPS { get; set; }
        public override float Value
        {
            get
            {
                if (BZero3 + B11 == 0)
                {
                    EPS = 1e-9f;
                }
                return ((BZero3 - B11) / (BZero3 + B11 + EPS));
            }
        }
    }

    [PartitionedTable]
    public class MndwiPolygon : BaseIndexPolygon
    {

    }

    public class MndwiPolygonRelation
    {
        public Guid Id { get; set; }

        public Guid MndWiId { get; set; }
        public Guid? RegionId { get; set; }
        public Guid? FieldId { get; set; }
        public Guid? SeasonId { get; set; }
    }
}
