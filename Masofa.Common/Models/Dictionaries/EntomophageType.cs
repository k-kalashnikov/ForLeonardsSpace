namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Виды энтомофагов
    /// </summary>
    /// <remarks>
    /// Справочник энтомофагов
    /// </remarks>
    public class EntomophageType : NamedDictionaryItem
    {
        /// <summary>
        /// Наименование на латыни
        /// </summary>
        public string? NameLa { get; set; }
    }

    public class EntomophageTypeHistory : BaseHistoryEntity<EntomophageType> { }
}
