using Masofa.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Weather;

[PartitionedTable]
[Table("RegionsWeathers")]
public partial class RegionsWeather
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public Guid RegionId { get; set; }

    public Guid ProviderId { get; set; }

    public DateTime Date { get; set; }

    public double? Temp { get; set; }

    public double? Precipitation { get; set; }

    public double? TempDev { get; set; }

    public double? PrecDev { get; set; }
}
