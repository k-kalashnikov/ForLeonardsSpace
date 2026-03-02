namespace Masofa.Common.Attributes
{
    /// <summary>
    /// Indicates that the decorated property represents a value to be included in reports.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ReportValueAttribute : Attribute
    {
        public string? ColorTable { get; set; }
    }
}
