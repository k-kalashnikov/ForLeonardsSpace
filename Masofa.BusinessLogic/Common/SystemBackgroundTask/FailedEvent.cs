//using Masofa.Common.Models;
//using Masofa.Common.Models.System;
//using MediatR;

//namespace Masofa.BusinessLogic.Common.SystemBackgroundTask
//{
//    /// <summary>
//    /// Событие неудачного выполнения фоновой задачи
//    /// </summary>
//    public class FailedEvent : INotification
//    {
//        /// <summary>
//        /// Выполненная задача
//        /// </summary>
//        [TaskParameter("Выполненная задача", true)]
//        public Masofa.Common.Models.System.SystemBackgroundTask Task { get; set; }

//        /// <summary>
//        /// Результат выполнения
//        /// </summary>
//        [TaskParameter("Результат выполнения задачи", true)]
//        public SystemBackgroundTaskResult Result { get; set; }

//        /// <summary>
//        /// Исключение, которое произошло
//        /// </summary>
//        [TaskParameter("Исключение, которое произошло", true)]
//        public Exception Exception { get; set; }

//        /// <summary>
//        /// Время выполнения в миллисекундах
//        /// </summary>
//        [TaskParameter("Время выполнения в миллисекундах", true)]
//        public long ExecutionTimeMs { get; set; }

//        /// <summary>
//        /// Попытка выполнения (для retry логики)
//        /// </summary>
//        [TaskParameter("Номер попытки выполнения", true)]
//        public int AttemptNumber { get; set; }

//        public FailedEvent(
//            Masofa.Common.Models.System.SystemBackgroundTask task, 
//            SystemBackgroundTaskResult result, 
//            Exception exception, 
//            long executionTimeMs, 
//            int attemptNumber = 1)
//        {
//            Task = task;
//            Result = result;
//            Exception = exception;
//            ExecutionTimeMs = executionTimeMs;
//            AttemptNumber = attemptNumber;
//        }
//    }
//}
