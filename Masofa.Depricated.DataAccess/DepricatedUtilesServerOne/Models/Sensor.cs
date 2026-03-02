using System;
using System.Collections.Generic;

namespace Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;

public partial class Sensor
{
    public Guid Id { get; set; }

    public int Code { get; set; }

    public string? Describe { get; set; }

    public virtual ICollection<Band> Bands { get; set; } = new List<Band>();

    public virtual ICollection<Date> Dates { get; set; } = new List<Date>();

    public virtual ICollection<Ndvi> Ndvis { get; set; } = new List<Ndvi>();

    public virtual ICollection<SceneKey> SceneKeys { get; set; } = new List<SceneKey>();

    public virtual ICollection<Shape> Shapes { get; set; } = new List<Shape>();

    public virtual ICollection<Zone> Zones { get; set; } = new List<Zone>();
}
