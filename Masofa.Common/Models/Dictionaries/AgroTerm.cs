namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Аграрные термины и определения
    /// </summary>
    /// <remarks>
    /// Справочник аграрных терминов и их определений
    /// </remarks>
    public class AgroTerm : NamedDictionaryItem
    {
        /// <summary>
        /// Описание
        /// </summary>
        public LocalizationString Descriptions { get; set; }
    }

    public class AgroTermHistory : BaseHistoryEntity<AgroTerm> { }
}
