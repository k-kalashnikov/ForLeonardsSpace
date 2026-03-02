using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUfieldsServerOne.Models;

public partial class Fertilizer
{
    public Guid Id { get; set; }

    public LocalDate? Date { get; set; }

    public Guid? TypeId { get; set; }

    public Guid? ProductId { get; set; }

    public string? Comment { get; set; }

    public LocalDateTime? CreateDate { get; set; }

    public string? CreateUser { get; set; }

    public LocalDateTime? ModifyDate { get; set; }

    public string? ModifyUser { get; set; }

    public int? Count { get; set; }

    public Guid? Units { get; set; }

    public Guid? SeasonId { get; set; }
}
