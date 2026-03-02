using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.SystemCrical
{
    /// <summary>
    /// Результат выполнения фоновой задачи
    /// </summary>
    public class SystemBackgroundTaskResult : BaseEntity
    {
        /// <summary>
        /// Тип результата
        /// </summary>
        public SystemBackgroundTaskResultType ResultType { get; set; }

        /// <summary>
        /// ID задачи
        /// </summary>
        public Guid SystemBackgroundTaskId { get; set; }

        /// <summary>
        /// JSON с результатом выполнения задачи
        /// </summary>
        public string? TaskResultJson { get; set; }
        public string? TaskResultJsonTypeName { get; set; }

        /// <summary>
        /// Тип объекта для десериализации результата (не сохраняется в БД)
        /// </summary>
        [NotMapped]
        public Type? TaskResultJsonType { get; set; }
    }

    /// <summary>
    /// Типы результатов выполнения фоновых задач
    /// </summary>
    public enum SystemBackgroundTaskResultType
    {
        /// <summary>
        /// Успешное выполнение
        /// </summary>
        Success = 0,

        /// <summary>
        /// Ошибка выполнения
        /// </summary>
        Failed = 1
    }

    public class SystemBackgroundTaskResultHistory : BaseHistoryEntity<SystemBackgroundTaskResult> { }
}
