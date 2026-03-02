using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;

public partial class ProcessingBand
{
    public Guid Id { get; set; }

    public Guid? AppId { get; set; }

    public LocalDateTime? CreatedAt { get; set; }
}
