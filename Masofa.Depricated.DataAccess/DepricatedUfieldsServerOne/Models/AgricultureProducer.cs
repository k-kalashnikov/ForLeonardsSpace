using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUfieldsServerOne.Models;

public partial class AgricultureProducer
{
    public Guid Id { get; set; }

    public Guid? FieldId { get; set; }

    public LocalDate? StartDate { get; set; }

    public LocalDate? EndDate { get; set; }

    public Guid? AgricultureProducerId { get; set; }

    public LocalDateTime? CreateDate { get; set; }

    public string? CreateUser { get; set; }

    public LocalDateTime? ModifyDate { get; set; }

    public string? ModifyUser { get; set; }
}
