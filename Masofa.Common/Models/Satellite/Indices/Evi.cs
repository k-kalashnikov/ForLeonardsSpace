using Masofa.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Common.Models.Satellite.Indices
{
    [PartitionedTable]
    public class EviPoint : BaseIndexPoint
    {
        public float BZero8 { get; set; }
        public float BZero4 { get; set; }
        public float? BZero2 { get; set; }
        public float EPS { get; set; } = 1e-6f;

        public const float EVI_G = 2.5f;
        public const float EVI_C1 = 6.0f;
        public const float EVI_C2 = 7.5f;
        public const float EVI_L = 1.0f;

        public override float Value
        {
            get
            {
                var blue = BZero2 ?? 0f;

                var num = (BZero8 - BZero4);

                var denom = (BZero8 + (EVI_C1 * BZero4) - (EVI_C2 * blue) + EVI_L + EPS);

                if (Math.Abs(denom) < EPS)
                    denom = EPS;

                var evi = EVI_G * (num / denom);

                // можно поджать диапазон к [-1, 1]:
                // evi = Math.Clamp(evi, -1f, 1f);

                return evi;
            }
        }

    }

    [PartitionedTable]
    public class EviPolygon : BaseIndexPolygon
    {

    }

    public class EviPolygonRelation
    {
        public Guid Id { get; set; }

        public Guid EviId { get; set; }
        public Guid? RegionId { get; set; }
        public Guid? FieldId { get; set; }
        public Guid? SeasonId { get; set; }
    }
}
