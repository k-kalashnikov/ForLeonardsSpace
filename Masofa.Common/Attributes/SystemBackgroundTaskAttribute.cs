namespace Masofa.Common.Attributes
{
    /// <summary>
    /// Specifies the type of a background task based on its importance.
    /// </summary>
    /// <remarks>Use this enumeration to categorize background tasks as either critical or non-critical.
    /// Critical tasks are those that must be completed to ensure proper application functionality, while non-critical
    /// tasks can be deferred or deprioritized.</remarks>
    public enum BackgroundTaskType
    {
        Critical,
        NotCritical
    }

    /// <summary>
    /// Specifies that a method or class is associated with a system background task.
    /// </summary>
    /// <remarks>This attribute is used to indicate that the decorated method or class is intended to
    /// represent a specific type of background task within the system. The <see cref="BackgroundTaskType"/>  provided
    /// during initialization determines the type of background task being represented.</remarks>
    public class SystemBackgroundTaskAttribute : Attribute
    {
        public BackgroundTaskType TaskType { get; }
        public string LocalizationKey { get; }
        public string Module { get; }

        public SystemBackgroundTaskAttribute(BackgroundTaskType taskType, string localizationKey, string module)
        {
            TaskType = taskType;
            LocalizationKey = localizationKey ?? throw new ArgumentNullException(nameof(localizationKey));
            Module = module;
        }
    }
}
