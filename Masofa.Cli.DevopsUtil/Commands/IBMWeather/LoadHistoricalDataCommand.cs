using Masofa.Common.Models.SystemCrical;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Masofa.Cli.DevopsUtil.Commands.IBMWeather;

/// <summary>
/// CLI команда для загрузки исторических данных IBM Weather
/// </summary>
[BaseCommand("IBM Weather Load Historical Data", "Загрузка исторических данных IBM Weather")]
public class LoadHistoricalDataCommand : IBaseCommand
{
    private readonly ILogger<LoadHistoricalDataCommand> Logger;
    private readonly IMediator Mediator;

    public LoadHistoricalDataCommand(
        ILogger<LoadHistoricalDataCommand> logger,
        IMediator mediator)
    {
        Logger = logger;
        Mediator = mediator;
    }

    public async Task Execute()
    {
        Logger.LogInformation("Start IBM Weather Load Historical Data");

        try
        {
            Console.WriteLine("Запуск загрузки исторических данных для всех активных станций...");

            // Default to last 30 days for historical data
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-30);

            var command = new Masofa.BusinessLogic.IBMWeather.Commands.LoadHistoricalDataCommand(
                ibmMeteoStationId: Guid.Empty, // Will be handled by business logic to get all stations
                startDate: startDate,
                endDate: endDate,
                loadDailySummary: true,
                loadHourlyData: true);

            await Mediator.Send(command);

            Console.WriteLine("Загрузка исторических данных завершена успешно!");
            Logger.LogInformation("End IBM Weather Load Historical Data");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Ошибка при загрузке исторических данных");
            Console.WriteLine($"Ошибка при загрузке исторических данных: {ex.Message}");
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
