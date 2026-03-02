using Masofa.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Weather;

[PartitionedTable]
[Table("Reports")]
public partial class Report
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public Guid? ReportType { get; set; }

    public string? Name { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? Link { get; set; }

    public string? Description { get; set; }

    public string? SourceQuery { get; set; }
}
