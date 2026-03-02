using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;

public partial class Field
{
    public Guid Uid { get; set; }

    public string Name { get; set; } = null!;

    public string? Region { get; set; }

    public double? North { get; set; }

    public double? South { get; set; }

    public double? West { get; set; }

    public double? East { get; set; }

    public LocalDateTime? Modified { get; set; }

    public LocalDateTime? CreatedAt { get; set; }

    public int? Status { get; set; }

    public LocalDate? CheckDate { get; set; }

    public LocalDate? UpdateDate { get; set; }

    public Guid? AppId { get; set; }

    public int? SentDatesAmount { get; set; }

    public long? TilesOrder { get; set; }

    public LocalDate? StartDate { get; set; }

    public bool? Germination { get; set; }

    public bool? Fallow { get; set; }

    public bool? Enabled { get; set; }

    public double? CpLat { get; set; }

    public double? CpLon { get; set; }

    public LocalDate? PlantingDate { get; set; }

    public int? SeasonMonths { get; set; }

    public bool? HasNewDate { get; set; }

    public bool? GenerateTiles { get; set; }

    public bool? SkipClouds { get; set; }

    public double? LSavi { get; set; }

    public bool? Copernicus { get; set; }

    public bool? Amazon { get; set; }

    public bool? Test { get; set; }

    public virtual App? App { get; set; }
}
