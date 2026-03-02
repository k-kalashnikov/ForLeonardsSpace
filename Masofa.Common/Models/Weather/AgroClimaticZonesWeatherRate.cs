using Masofa.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Weather;

[PartitionedTable]
[Table("AgroClimaticZonesWeatherRates")]
public partial class AgroClimaticZonesWeatherRate
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public DateTime? Date { get; set; }

    public double? TempRate { get; set; }

    public double? PrecRate { get; set; }
}
