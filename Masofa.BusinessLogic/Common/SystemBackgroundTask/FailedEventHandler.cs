//using Masofa.Common.Models.System;
//using Masofa.Common.Services;
//using MediatR;
//using Microsoft.Extensions.Logging;

//namespace Masofa.BusinessLogic.Common.SystemBackgroundTask
//{
//    /// <summary>
//    /// Обработчик события неудачного выполнения фоновой задачи
//    /// </summary>
//    public class FailedEventHandler : INotificationHandler<FailedEvent>
//    {
//        private readonly ISystemBackgroundTaskScheduler _taskScheduler;
//        private readonly ILogger<FailedEventHandler> _logger;

//        public FailedEventHandler(
//            ISystemBackgroundTaskScheduler taskScheduler,
//            ILogger<FailedEventHandler> logger)
//        {
//            _taskScheduler = taskScheduler;
//            _logger = logger;
//        }

//        public async Task Handle(FailedEvent notification, CancellationToken cancellationToken)
//        {
//            try
//            {
//                _logger.LogWarning(
//                    "Task failed: {TaskId} - {TaskName} - Attempt: {AttemptNumber} - Execution time: {ExecutionTimeMs}ms - Error: { ex.Message}", 
//                    notification.Task.Id, 
//                    notification.Task.NameRu, 
//                    notification.AttemptNumber,
//                    notification.ExecutionTimeMs,
//                    notification.Exception.Message);

//                // Проверяем, можно ли повторить задачу
//                if (notification.Task.IsRetryable && notification.Task.CurrentRetryCount < notification.Task.MaxRetryCount)
//                {
//                    _logger.LogInformation(
//                        "Scheduling retry for task: {TaskId} - {TaskName} - Retry {CurrentRetry}/{MaxRetries}", 
//                        notification.Task.Id, 
//                        notification.Task.NameRu, 
//                        notification.Task.CurrentRetryCount + 1, 
//                        notification.Task.MaxRetryCount);

//                    // Планируем повтор через 30 секунд
//                    var retryDelay = TimeSpan.FromSeconds(30);
//                    var success = await _taskScheduler.ScheduleRetryAsync(notification.Task, retryDelay);
                    
//                    if (success)
//                    {
//                        _logger.LogInformation(
//                            "Retry scheduled successfully for task: {TaskId} - {TaskName} - Will retry in {Delay}ms", 
//                            notification.Task.Id, 
//                            notification.Task.NameRu, 
//                            retryDelay.TotalMilliseconds);
//                    }
//                    else
//                    {
//                        _logger.LogError(
//                            "Failed to schedule retry for task: {TaskId} - {TaskName}", 
//                            notification.Task.Id, 
//                            notification.Task.NameRu);
//                    }
//                }
//                else
//                {
//                    _logger.LogError(
//                        "Task failed permanently: {TaskId} - {TaskName} - Max retries reached or task not retryable", 
//                        notification.Task.Id, 
//                        notification.Task.NameRu);

//                    // Здесь можно добавить логику для:
//                    // - Отправки уведомлений администратору
//                    // - Деактивации задачи
//                    // - Создания инцидента
//                    // - Логирования в систему мониторинга
//                }

//                // Логируем полную информацию об ошибке
//                _logger.LogDebug(
//                    "Task failure details: {TaskId} - Exception: {Exception}", 
//                    notification.Task.Id, 
//                    notification.Exception.ToString());
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, 
//                    "Error handling task failure event: {TaskId}", 
//                    notification.Task.Id);
//            }
//        }
//    }
//}
