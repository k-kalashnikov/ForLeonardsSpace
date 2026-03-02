using Masofa.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Common.Models.Satellite.Indices
{
    [PartitionedTable]
    public class NdmiPoint : BaseIndexPoint
    {
        public float BZero8 { get; set; }
        public float B11 { get; set; }
        public float EPS { get; set; }
        public override float Value
        {
            get
            {
                if (BZero8 + B11 == 0) 
                {
                    EPS = 1e-9f;
                }

                return ((BZero8 - B11) / (BZero8 + B11 + EPS));
            }
        }
    }

    [PartitionedTable]
    public class NdmiPolygon : BaseIndexPolygon
    {

    }

    public class NdmiPolygonRelation
    {
        public Guid Id { get; set; }

        public Guid NdMiId { get; set; }
        public Guid? RegionId { get; set; }
        public Guid? FieldId { get; set; }
        public Guid? SeasonId { get; set; }
    }
}
