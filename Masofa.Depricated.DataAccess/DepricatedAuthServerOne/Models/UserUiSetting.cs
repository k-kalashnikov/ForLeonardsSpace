using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedAuthServerOne.Models;

public partial class UserUiSetting
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Content { get; set; } = null!;

    public string Key { get; set; } = null!;
}
