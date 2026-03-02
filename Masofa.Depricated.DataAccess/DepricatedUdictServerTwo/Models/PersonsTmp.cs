using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models;

public partial class PersonsTmp
{
    public Guid Id { get; set; }

    public string? Pinfl { get; set; }

    public bool Visible { get; set; }

    public string? OrderCode { get; set; }

    public string? ExtData { get; set; }

    public string? Comment { get; set; }

    public Instant CreateDate { get; set; }

    public string CreateUser { get; set; } = null!;

    public Instant UpdateDate { get; set; }

    public string UpdateUser { get; set; } = null!;

    public string? NameUz { get; set; }

    public string? NameEn { get; set; }

    public string? NameRu { get; set; }

    public Guid? UserId { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? MailingAddress { get; set; }

    public Guid? MainRegionId { get; set; }

    public string? Phones { get; set; }

    public string? PhysicalAddress { get; set; }

    public string? Telegram { get; set; }
}
