using Masofa.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Weather;

[PartitionedTable]
[Table("Logs")]
public partial class Log
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public DateTime? Date { get; set; }

    public Guid? JobId { get; set; }

    public string? JobStatus { get; set; }

    public Guid? ProviderId { get; set; }

    public string? Details { get; set; }

    public double? ContentSize { get; set; }

    public string? UserInfo { get; set; }
}
