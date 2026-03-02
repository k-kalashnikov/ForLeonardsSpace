using Masofa.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Weather;

[PartitionedTable]
[Table("Jobs")]
public partial class Job
{
    public Guid Id { get; set; }

    public DateTime Date { get; set; }

    public string? Action { get; set; }

    public Guid? ProviderId { get; set; }

    public Guid? JobStatusId { get; set; }

    public string? Application { get; set; }

    public string? Result { get; set; }

    public string? Path { get; set; }
}
