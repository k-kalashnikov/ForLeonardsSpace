using Masofa.Common.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.SystemCrical
{
    [Table("LogMessages")]
    [PartitionedTable]
    public class LogMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public string Message { get; set; } = string.Empty;
        public LogLevelType LogMessageType { get; set; }
        public Guid CallStackId { get; set; }
        public string Path { get; set; } = string.Empty;
        public int Order { get; set; }
    }

    public enum LogLevelType
    {
        Debug = 0,
        Information = 1,
        Warning = 2,
        Error = 3,
        Critical = 4
    }
}
