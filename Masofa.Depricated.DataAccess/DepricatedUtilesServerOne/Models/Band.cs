using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;

public partial class Band
{
    public Guid Id { get; set; }

    public Guid? Uid { get; set; }

    public int? SensorCode { get; set; }

    public Guid? AppId { get; set; }

    public LocalDate Date { get; set; }

    public string Label { get; set; } = null!;

    public double? NoData { get; set; }

    public LocalDateTime? CreateTime { get; set; }

    public string? Details { get; set; }

    public int? State { get; set; }

    public string? CopernicusJson { get; set; }

    public int? PlantStateCode { get; set; }

    public string? PlantDetails { get; set; }

    public LocalDateTime? PlantCreatedAt { get; set; }

    public int? Attempts { get; set; }

    public short? Cloudy { get; set; }

    public int? TrueStateCode { get; set; }

    public string? TrueDetails { get; set; }

    public LocalDateTime? TrueCreatedAt { get; set; }

    public int? SaviStateCode { get; set; }

    public string? SaviDetails { get; set; }

    public LocalDateTime? SaviCreatedAt { get; set; }

    public int? LaiStateCode { get; set; }

    public string? LaiDetails { get; set; }

    public LocalDateTime? LaiCreatedAt { get; set; }

    public int? EviStateCode { get; set; }

    public string? EviDetails { get; set; }

    public LocalDateTime? EviCreatedAt { get; set; }

    public int? ArviStateCode { get; set; }

    public string? ArviDetails { get; set; }

    public LocalDateTime? ArviCreatedAt { get; set; }

    public int? NdwiStateCode { get; set; }

    public string? NdwiDetails { get; set; }

    public LocalDateTime? NdwiCreatedAt { get; set; }

    public virtual Sensor? SensorCodeNavigation { get; set; }
}
