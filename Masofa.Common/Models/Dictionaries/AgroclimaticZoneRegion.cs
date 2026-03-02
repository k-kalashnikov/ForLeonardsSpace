using Masofa.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Common.Models.Dictionaries
{
    public class AgroclimaticZoneRegion
    {
        /// <summary>
        /// Идентификатор связи агроклиматической зоны с регионами
        /// </summary> 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [SystemFieldNotForImport]
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор агроклиматической зоны
        /// </summary>
        public Guid AgroclimaticZoneId { get; set; }

        /// <summary>
        /// Идентификатор региона
        /// </summary> 
        public Guid RegionId { get; set; }
    }
}
