namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Виды вредителей
    /// </summary>
    /// <remarks>
    /// Справочник видов вредителей
    /// </remarks>
    public class PestType : NamedDictionaryItem
    {
        /// <summary>
        /// Наименование на латыни
        /// </summary>
        public string? NameLa { get; set; }
    }

    public class PestTypeHistory : BaseHistoryEntity<PestType> { }
}
