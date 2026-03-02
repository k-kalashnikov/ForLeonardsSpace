using Masofa.Common.Attributes;

namespace Masofa.Common.Models.Satellite.Sentinel
{

    [PartitionedTable]
    public class SentinelInspireMetadata : BaseEntity
    {
        // General Info
        public string FileIdentifier { get; set; }
        public string LanguageCode { get; set; }
        public string CharacterSetCode { get; set; }
        public string HierarchyLevelCode { get; set; }
        public DateTime DateStamp { get; set; }
        public string MetadataStandardName { get; set; }
        public string MetadataStandardVersion { get; set; }

        // Contact
        public string OrganisationName { get; set; }
        public string Email { get; set; }
        public string RoleCode { get; set; }

        // Geographic
        public decimal WestBoundLongitude { get; set; }
        public decimal EastBoundLongitude { get; set; }
        public decimal SouthBoundLatitude { get; set; }
        public decimal NorthBoundLatitude { get; set; }

        // Reference System
        public string ReferenceSystemCode { get; set; }
        public string ReferenceSystemCodeSpace { get; set; }
    }

    public class SentinelInspireMetadataHistory : BaseHistoryEntity<SentinelInspireMetadata> { }
}