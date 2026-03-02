namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Страховые случаи
    /// </summary>
    /// <remarks>
    /// Справочник страховых случаев
    /// </remarks>
    public class InsuranceCase : NamedDictionaryItem { }

    public class InsuranceCaseHistory : BaseHistoryEntity<InsuranceCase> { }
}