using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models;

public partial class DictionaryCell
{
    public Guid Id { get; set; }

    public Guid DictionaryId { get; set; }

    public Guid ColumnId { get; set; }

    public Guid RowId { get; set; }

    public string? CellData { get; set; }
}
