namespace Masofa.Common.Models.Satellite
{
    public class LandsatProductMetadata : BaseEntity
    {
        public string ProductId { get; set; } = default!;
        public string? SpacecraftId { get; set; }
        public string? SensorId { get; set; }
        public string? ProcessingLevel { get; set; }
        public DateTime? AcquisitionDate { get; set; }
        public DateTime? SceneCenterTime { get; set; }
        public string? Path { get; set; }
        public string? Row { get; set; }
        public double? CloudCover { get; set; }
        public string? DataType { get; set; }
        public string? CollectionCategory { get; set; }
        public string? WrsPathRow { get; set; }
        public string? MetadataFile { get; set; }
        public string? ProductRefId { get; set; }
    }

    public class LandsatProductMetadataHistory : BaseHistoryEntity<LandsatProductMetadata> { }
}
