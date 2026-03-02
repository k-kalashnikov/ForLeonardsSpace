using Masofa.Common.Models.SystemCrical;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Masofa.Cli.DevopsUtil.Commands.IBMWeather;

/// <summary>
/// CLI команда для загрузки текущих данных IBM Weather
/// </summary>
[BaseCommand("IBM Weather Load Current Data", "Загрузка текущих данных IBM Weather")]
public class LoadCurrentDataCommand : IBaseCommand
{
    private readonly ILogger<LoadCurrentDataCommand> Logger;
    private readonly IMediator Mediator;

    public LoadCurrentDataCommand(
        ILogger<LoadCurrentDataCommand> logger,
        IMediator mediator)
    {
        Logger = logger;
        Mediator = mediator;
    }

    public async Task Execute()
    {
        Logger.LogInformation("Start IBM Weather Load Current Data");

        try
        {
            Console.WriteLine("Запуск загрузки текущих данных для всех активных станций...");

            var command = new Masofa.BusinessLogic.IBMWeather.Commands.LoadCurrentDataCommand(
                ibmMeteoStationId: null,
                forceUpdate: false);

            await Mediator.Send(command);

            Console.WriteLine("Загрузка текущих данных завершена успешно!");
            Logger.LogInformation("End IBM Weather Load Current Data");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Ошибка при загрузке текущих данных");
            Console.WriteLine($"Ошибка при загрузке текущих данных: {ex.Message}");
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
