namespace Masofa.Common.Attributes
{
    /// <summary>
    /// Indicates that the decorated class represents a partitioned table in the data model.
    /// </summary>
    /// <remarks>This attribute is used to annotate classes that correspond to partitioned tables,  typically
    /// in a database or similar data storage system. It is intended for use in  scenarios where partitioning is a key
    /// aspect of the table's design or behavior.</remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class PartitionedTableAttribute : Attribute
    {

    }
}