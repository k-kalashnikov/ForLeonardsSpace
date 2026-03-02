using Masofa.Common.Resources;
using MediatR;
using Masofa.BusinessLogic.IBMWeather.Commands;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Quartz;

namespace Masofa.Web.Monolith.Jobs.IBMWeather;

/// <summary>
/// Джоба для загрузки текущих данных IBM Weather каждый час
/// </summary>
[DisallowConcurrentExecution]
public class LoadCurrentDataJob : IJob
{
    private readonly IMediator _mediator;
    private readonly IBusinessLogicLogger _logger;

    public LoadCurrentDataJob(IMediator mediator, IBusinessLogicLogger logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var jobName = nameof(LoadCurrentDataJob);
        
        try
        {
            await _logger.LogInformationAsync(LogMessageResource.JobStarted(jobName), jobName);

            var command = new LoadCurrentDataCommand(
                ibmMeteoStationId: null, // Загружаем для всех активных станций
                forceUpdate: false);

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
