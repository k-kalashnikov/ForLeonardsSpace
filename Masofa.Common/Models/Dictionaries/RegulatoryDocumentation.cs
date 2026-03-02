namespace Masofa.Common.Models.Dictionaries
{
    public class RegulatoryDocumentation : NamedDictionaryItem
    {
        /// <summary>
        /// Идентификатор файла в хранилище
        /// </summary>
        public Guid? FileStorageId { get; set; }
    }

    public class RegulatoryDocumentationHistory : BaseHistoryEntity<RegulatoryDocumentation> { }
}
