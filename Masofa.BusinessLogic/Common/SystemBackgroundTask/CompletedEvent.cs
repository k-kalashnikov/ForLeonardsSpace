//using Masofa.Common.Models.System;
//using MediatR;
//using Microsoft.Extensions.Logging;

//namespace Masofa.BusinessLogic.Common.SystemBackgroundTask
//{
//    /// <summary>
//    /// Событие успешного выполнения фоновой задачи
//    /// </summary>
//    public class CompletedEvent : INotification
//    {
//        /// <summary>
//        /// Выполненная задача
//        /// </summary>
//        public Masofa.Common.Models.System.SystemBackgroundTask Task { get; set; }

//        /// <summary>
//        /// Результат выполнения
//        /// </summary>
//        public SystemBackgroundTaskResult Result { get; set; }

//        /// <summary>
//        /// Время выполнения в миллисекундах
//        /// </summary>
//        public long ExecutionTimeMs { get; set; }

//        /// <summary>
//        /// Дополнительные данные результата
//        /// </summary>
//        public object? ResultData { get; set; }

//        public CompletedEvent(
//            Masofa.Common.Models.System.SystemBackgroundTask task, 
//            SystemBackgroundTaskResult result, 
//            long executionTimeMs, 
//            object? resultData = null)
//        {
//            Task = task;
//            Result = result;
//            ExecutionTimeMs = executionTimeMs;
//            ResultData = resultData;
//        }
//    }

//    /// <summary>
//    /// Обработчик события успешного выполнения фоновой задачи
//    /// </summary>
//    public class CompletedEventHandler : INotificationHandler<CompletedEvent>
//    {
//        private readonly ILogger<CompletedEventHandler> _logger;

//        public CompletedEventHandler(
//            ILogger<CompletedEventHandler> logger)
//        {
//            _logger = logger;
//        }

//        public async Task Handle(CompletedEvent notification, CancellationToken cancellationToken)
//        {
//            try
//            {
//                _logger.LogInformation(
//                    "Task completed successfully: {TaskId} - {TaskName} - Execution time: {ExecutionTimeMs}ms",
//                    notification.Task.Id,
//                    notification.Task.NameRu,
//                    notification.ExecutionTimeMs);



//                // Логируем дополнительную информацию
//                if (notification.ResultData != null)
//                {
//                    _logger.LogDebug(
//                        "Task result data: {TaskId} - {ResultData}",
//                        notification.Task.Id,
//                        notification.ResultData);
//                }

//                // Здесь можно добавить дополнительную логику:
//                // - Отправка уведомлений
//                // - Обновление метрик
//                // - Запуск зависимых задач
//                // - Обновление статусов в других системах
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex,
//                    "Error handling task completion event: {TaskId}",
//                    notification.Task.Id);
//            }
//        }
//    }
//}
