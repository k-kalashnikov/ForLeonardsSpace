using Masofa.Common.Attributes;
using Masofa.Common.Models.Satellite.Auxiliary;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite.Sentinel
{

    [PartitionedTable]
    [Table("SentinelProductQualityMetadata")]
    public class SentinelProductQualityMetadata : BaseEntity
    {
        // Earth Explorer Header
        public string FileName { get; set; }

        public string FileDescription { get; set; }

        public string Notes { get; set; }

        public string Mission { get; set; }

        public string FileClass { get; set; }

        public string FileType { get; set; }

        // Validity Period
        public string ValidityStart { get; set; }
        public string ValidityStop { get; set; }

        public string FileVersion { get; set; }

        // Source Info
        public string System { get; set; }
        public string Creator { get; set; }
        public string CreatorVersion { get; set; }
        public string CreationDate { get; set; }

        // Data Block
        public string Type { get; set; }

        // Report
        public string GippVersion { get; set; }
        public string GlobalStatus { get; set; }
        public DateTime Date { get; set; }

        // CheckList
        public string ParentId { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }

        // Check
        [NotMapped]
        public List<Check> Checks { get; set; }
        public string ChecksRaw
        {
            get => Checks == null ? null : string.Join("~", Checks.Select(x => x.ToString()));
            set => Checks = string.IsNullOrEmpty(value) ? null : value.Split('~').Select(Check.FromString).Where(x => x != null).ToList();
        }
    }

    public class SentinelProductQualityMetadataHistory : BaseHistoryEntity<SentinelProductQualityMetadata> { }
}