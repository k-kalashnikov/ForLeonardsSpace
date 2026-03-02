using Masofa.Common.Models.SystemCrical;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Masofa.Cli.DevopsUtil.Commands.IBMWeather;

/// <summary>
/// CLI команда для загрузки прогнозных данных IBM Weather
/// </summary>
[BaseCommand("IBM Weather Load Forecast Data", "Загрузка прогнозных данных IBM Weather")]
public class LoadForecastDataCommand : IBaseCommand
{
    private readonly ILogger<LoadForecastDataCommand> Logger;
    private readonly IMediator Mediator;

    public LoadForecastDataCommand(
        ILogger<LoadForecastDataCommand> logger,
        IMediator mediator)
    {
        Logger = logger;
        Mediator = mediator;
    }

    public async Task Execute()
    {
        Logger.LogInformation("Start IBM Weather Load Forecast Data");

        try
        {
            Console.WriteLine("Запуск загрузки прогнозных данных для всех активных станций...");

            var command = new Masofa.BusinessLogic.IBMWeather.Commands.LoadForecastDataCommand(
                ibmMeteoStationId: null,
                forceUpdate: false,
                loadDailyForecast: true,
                loadHodData: true);

            await Mediator.Send(command);

            Console.WriteLine("Загрузка прогнозных данных завершена успешно!");
            Logger.LogInformation("End IBM Weather Load Forecast Data");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Ошибка при загрузке прогнозных данных");
            Console.WriteLine($"Ошибка при загрузке прогнозных данных: {ex.Message}");
            throw;
        }
    }

    public void Dispose()
    {
    }

    public Task Execute(string[] args)
    {
        throw new NotImplementedException();
    }
}
