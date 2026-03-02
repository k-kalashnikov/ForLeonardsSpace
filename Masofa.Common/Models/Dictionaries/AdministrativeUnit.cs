namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Типы административных единиц
    /// </summary>
    /// <remarks>
    /// Справочник типов административных единиц
    /// </remarks>
    public class AdministrativeUnit : NamedDictionaryItem
    {
        /// <summary>
		/// Уровень иерархии
		/// </summary>
        public int? Level { get; set; }
    }

    public class AdministrativeUnitHistory : BaseHistoryEntity<AdministrativeUnit> { }
}

