namespace Masofa.Common.Models.Dictionaries
{
    /// <summary>
    /// Статусы задач
    /// </summary>
    /// <remarks>
    /// Справочник статусов задач
    /// </remarks>
    public class TaskStatus : NamedDictionaryItem { }

    public class TaskStatusHistory : BaseHistoryEntity<TaskStatus> { }
}

