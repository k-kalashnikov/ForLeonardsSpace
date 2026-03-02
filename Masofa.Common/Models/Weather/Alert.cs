using Masofa.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Weather;

[PartitionedTable]
[Table("Alerts")]
public partial class Alert
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public Guid RegionId { get; set; }

    public int? TypeId { get; set; }

    public Guid ProviderId { get; set; }

    public DateTime Date { get; set; }

    public double? Value { get; set; }

    public Guid? AgroClimaticZonesId { get; set; }
}
