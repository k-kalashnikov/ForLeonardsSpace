using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models;


public partial class DataType
{
    public Guid Id { get; set; }

    public string SqlType { get; set; } = null!;

    public string Name { get; set; } = null!;
}
