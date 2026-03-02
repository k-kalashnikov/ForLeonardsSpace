using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;

public partial class App
{
    public Guid Id { get; set; }

    public bool? Alive { get; set; }

    public LocalDateTime? LastStartedAt { get; set; }

    public string? State { get; set; }

    public string? Host { get; set; }

    public string? Uuid { get; set; }

    public LocalDateTime? LastStopAt { get; set; }

    public string? Properties { get; set; }

    public virtual ICollection<Country> Countries { get; set; } = new List<Country>();

    public virtual ICollection<Download> Downloads { get; set; } = new List<Download>();

    public virtual ICollection<Field> Fields { get; set; } = new List<Field>();

    public virtual ICollection<LoadTask> LoadTasks { get; set; } = new List<LoadTask>();
}
