using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;

public partial class Date
{
    public Guid Id { get; set; }

    public Guid Uid { get; set; }

    public int? SensorCode { get; set; }

    public LocalDate Date1 { get; set; }

    public int? State { get; set; }

    public string? Details { get; set; }

    public int? TrueState { get; set; }

    public string? TrueDetails { get; set; }

    public LocalDateTime? TrueCreatedAt { get; set; }

    public int? PlantState { get; set; }

    public string? PlantDetails { get; set; }

    public LocalDateTime? PlantCreatedAt { get; set; }

    public int? GerminationState { get; set; }

    public string? GerminationDetails { get; set; }

    public LocalDateTime? GerminationCreatedAt { get; set; }

    public int? RelativeState { get; set; }

    public string? RelativeDetails { get; set; }

    public LocalDateTime? RelativeCreatedAt { get; set; }

    public LocalDateTime? CreatedAt { get; set; }

    public Guid? AppId { get; set; }

    public LocalDateTime? FieldModified { get; set; }

    public string? NdviHistogram { get; set; }

    public string? GerminationHistogram { get; set; }

    public double? South { get; set; }

    public double? North { get; set; }

    public double? West { get; set; }

    public double? East { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public int? SrcNumber { get; set; }

    public int? Attempts { get; set; }

    public string? RelativeHistogram { get; set; }

    public double? SouthNdvi { get; set; }

    public double? NorthNdvi { get; set; }

    public double? WestNdvi { get; set; }

    public double? EastNdvi { get; set; }

    public int? Restored { get; set; }

    public int? RestoredSceneKey { get; set; }

    public int? RtvicState { get; set; }

    public string? RtvicDetails { get; set; }

    public LocalDateTime? RtvicCreatedAt { get; set; }

    public int? NdmiState { get; set; }

    public string? NdmiDetails { get; set; }

    public LocalDateTime? NdmiCreatedAt { get; set; }

    public int? NdwiState { get; set; }

    public string? NdwiDetails { get; set; }

    public LocalDateTime? NdwiCreatedAt { get; set; }

    public int? GndviState { get; set; }

    public string? GndviDetails { get; set; }

    public LocalDateTime? GndviCreatedAt { get; set; }

    public int? SaviState { get; set; }

    public string? SaviDetails { get; set; }

    public LocalDateTime? SaviCreatedAt { get; set; }

    public int? VariState { get; set; }

    public string? VariDetails { get; set; }

    public LocalDateTime? VariCreatedAt { get; set; }

    public int? SourceState { get; set; }

    public string? SourceDetails { get; set; }

    public LocalDateTime? SourceCreatedAt { get; set; }

    public long? Priority { get; set; }

    public int? CviState { get; set; }

    public string? CviDetails { get; set; }

    public LocalDateTime? CviCreatedAt { get; set; }

    public int? LaiState { get; set; }

    public string? LaiDetails { get; set; }

    public LocalDateTime? LaiCreatedAt { get; set; }

    public string? CviHistogram { get; set; }

    public string? CviGerminationHistogram { get; set; }

    public string? CviRelativeHistogram { get; set; }

    public string? LaiHistogram { get; set; }

    public string? LaiGerminationHistogram { get; set; }

    public string? LaiRelativeHistogram { get; set; }

    public Guid? SceneKeyId { get; set; }

    public bool? Test { get; set; }

    public int? EviState { get; set; }

    public string? EviDetails { get; set; }

    public LocalDateTime? EviCreatedAt { get; set; }

    public int? ArviState { get; set; }

    public string? ArviDetails { get; set; }

    public LocalDateTime? ArviCreatedAt { get; set; }

    public virtual State? PlantStateNavigation { get; set; }

    public virtual State? RelativeStateNavigation { get; set; }

    public virtual Sensor? SensorCodeNavigation { get; set; }

    public virtual State? StateNavigation { get; set; }

    public virtual State? TrueStateNavigation { get; set; }
}
