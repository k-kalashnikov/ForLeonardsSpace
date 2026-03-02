using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUfieldsServerOne.Models;

public partial class Machinery
{
    public Guid Id { get; set; }

    public LocalDate? Date { get; set; }

    public Guid? MachineryTypeId { get; set; }

    public Guid? OperationId { get; set; }

    public double? Usage { get; set; }

    public LocalDateTime? CreateDate { get; set; }

    public string? CreateUser { get; set; }

    public LocalDateTime? ModifyDate { get; set; }

    public string? ModifyUser { get; set; }

    public string? Comment { get; set; }

    public Guid? SeasonId { get; set; }
}
