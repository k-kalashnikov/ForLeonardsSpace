using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Weather;

public partial class XslsUzUnputColumn
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public string? XlsColumnName { get; set; }

    public string? DbTableName { get; set; }

    public string? DbColumnName { get; set; }
}
