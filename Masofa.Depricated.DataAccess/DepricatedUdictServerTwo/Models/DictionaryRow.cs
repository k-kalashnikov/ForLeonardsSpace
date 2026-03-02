using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models;


public partial class DictionaryRow
{
    public Guid Id { get; set; }

    public Guid DictionaryId { get; set; }

    public Guid? ParentRowId { get; set; }
}
