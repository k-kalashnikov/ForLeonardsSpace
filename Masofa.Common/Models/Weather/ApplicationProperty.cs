using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Weather;

public partial class ApplicationProperty 
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public string Application { get; set; } = null!;

    public string Key { get; set; } = null!;

    public bool? Active { get; set; }

    public string? Value { get; set; }

    public string? Description { get; set; }
}
