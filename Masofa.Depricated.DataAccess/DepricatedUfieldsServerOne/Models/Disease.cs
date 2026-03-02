using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUfieldsServerOne.Models;

public partial class Disease
{
    public Guid Id { get; set; }

    public LocalDate? IdentificationDate { get; set; }

    public Guid? TypeId { get; set; }

    public Guid? NameId { get; set; }

    public LocalDate? ProcessingDate { get; set; }

    public string? Comment { get; set; }

    public LocalDateTime? CreateDate { get; set; }

    public string? CreateUser { get; set; }

    public LocalDateTime? ModifyDate { get; set; }

    public string? ModifyUser { get; set; }

    public Guid? SeasonId { get; set; }
}
