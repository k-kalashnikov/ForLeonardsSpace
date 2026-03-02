using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;

public partial class Queue
{
    public Guid Id { get; set; }

    public LocalDate Date { get; set; }

    public long Priority { get; set; }

    public int SensorCode { get; set; }
}
