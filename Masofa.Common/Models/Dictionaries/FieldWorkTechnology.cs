namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Справочник технологий полевых работ (посадка, уход, сбор урожая)
    /// </summary>
    /// <remarks>
    /// Справочник технологий полевых работ (посадка, уход, сбор урожая)
    /// </remarks>
    public class FieldWorkTechnology : NamedDictionaryItem { }

    public class FieldWorkTechnologyHistory : BaseHistoryEntity<FieldWorkTechnology> { }
}
