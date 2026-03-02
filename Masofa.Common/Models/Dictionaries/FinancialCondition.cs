namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Финансовые условия
    /// </summary>
    /// <remarks>
    /// Справочник финансовых условий
    /// </remarks>
    public class FinancialCondition : NamedDictionaryItem { }

    public class FinancialConditionHistory : BaseHistoryEntity<FinancialCondition> { }
}