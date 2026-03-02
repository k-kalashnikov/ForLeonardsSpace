using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models;

public partial class DictionaryColumn
{
    public Guid Id { get; set; }

    public Guid DictionaryId { get; set; }

    public string Name { get; set; } = null!;

    public Guid DataType { get; set; }
}
