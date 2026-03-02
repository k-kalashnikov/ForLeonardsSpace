namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Сельхоз операции
    /// </summary>
    /// <remarks>
    /// Справочник сельскохозяйственных операций
    /// </remarks>
    public class AgroOperation : NamedDictionaryItem { }

    public class AgroOperationHistory : BaseHistoryEntity<AgroOperation> { }
}

