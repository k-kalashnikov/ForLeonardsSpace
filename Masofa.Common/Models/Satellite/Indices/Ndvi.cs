using Masofa.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Common.Models.Satellite.Indices
{

    [PartitionedTable]
    public class NdviPoint : BaseIndexPoint
    {
        public float BZero8 { get; set; }
        public float BZero4 { get; set; }
        public float EPS { get; set; }

        public override float Value
        {
            get
            {
                if (BZero8 + BZero4 == 0)
                {
                    EPS = 1e-9f;
                }
                
                return ((BZero8 - BZero4) / (BZero8 + BZero4 + EPS));
            }
        }
    }

    [PartitionedTable]
    public class NdviPolygon : BaseIndexPolygon
    {
        
    }

    public class NdviPolygonRelation
    {
        public Guid Id { get; set; }

        public Guid NdViId { get; set; }
        public Guid? RegionId { get; set; }
        public Guid? FieldId { get; set; }
        public Guid? SeasonId { get; set; }
    }
}
