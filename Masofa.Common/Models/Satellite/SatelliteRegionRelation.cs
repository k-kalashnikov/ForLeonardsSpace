using Masofa.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Common.Models.Satellite
{
    public class SatelliteRegionRelation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [SystemFieldNotForImport]
        public Guid Id { get; set; }
        public Guid SatelliteProductId { get; set; }
        public Guid RegionId { get; set; }
    }
}
