using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;

public partial class State
{
    public Guid Id { get; set; }

    public int Code { get; set; }

    public string? Describe { get; set; }

    public virtual ICollection<Date> DatePlantStateNavigations { get; set; } = new List<Date>();

    public virtual ICollection<Date> DateRelativeStateNavigations { get; set; } = new List<Date>();

    public virtual ICollection<Date> DateStateNavigations { get; set; } = new List<Date>();

    public virtual ICollection<Date> DateTrueStateNavigations { get; set; } = new List<Date>();

    public virtual ICollection<LoadTask> LoadTasks { get; set; } = new List<LoadTask>();

    public virtual ICollection<SceneKey> SceneKeys { get; set; } = new List<SceneKey>();
}
