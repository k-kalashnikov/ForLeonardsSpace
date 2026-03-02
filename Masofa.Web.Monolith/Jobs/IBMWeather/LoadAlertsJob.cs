using Masofa.Common.Resources;
using MediatR;
using Masofa.BusinessLogic.IBMWeather.Commands;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Quartz;

namespace Masofa.Web.Monolith.Jobs.IBMWeather;

/// <summary>
/// Джоба для загрузки алертов IBM Weather каждые 5-10 минут
/// </summary>
[DisallowConcurrentExecution]
public class LoadAlertsJob : IJob
{
    private readonly IMediator _mediator;
    private readonly IBusinessLogicLogger _logger;

    public LoadAlertsJob(IMediator mediator, IBusinessLogicLogger logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var jobName = nameof(LoadAlertsJob);
        
        try
        {
            await _logger.LogInformationAsync(LogMessageResource.JobStarted(jobName), jobName);

            var command = new LoadAlertsCommand(
                ibmMeteoStationId: null, // Загружаем для всех активных станций
                forceUpdate: true, // Принудительно обновляем алерты
                cancellationToken: context.CancellationToken);

            await _mediator.Send(command, context.CancellationToken);

            await _logger.LogInformationAsync(LogMessageResource.JobCompleted(jobName), jobName);
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(LogMessageResource.JobError(jobName,ex.Message), jobName);
            throw;
        }
    }
}
