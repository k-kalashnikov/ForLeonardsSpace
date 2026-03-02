using Masofa.Common.Models.SystemCrical;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Masofa.Cli.DevopsUtil.Commands.IBMWeather;

/// <summary>
/// CLI команда для загрузки алертов IBM Weather
/// </summary>
[BaseCommand("IBM Weather Load Alerts", "Загрузка алертов IBM Weather")]
public class LoadAlertsCommand : IBaseCommand
{
    private readonly ILogger<LoadAlertsCommand> Logger;
    private readonly IMediator Mediator;

    public LoadAlertsCommand(
        ILogger<LoadAlertsCommand> logger,
        IMediator mediator)
    {
        Logger = logger;
        Mediator = mediator;
    }

    public async Task Execute()
    {
        Logger.LogInformation("Start IBM Weather Load Alerts");

        try
        {
            Console.WriteLine("Запуск загрузки алертов для всех активных станций...");

            var command = new Masofa.BusinessLogic.IBMWeather.Commands.LoadAlertsCommand(
                ibmMeteoStationId: null,
                forceUpdate: false);

            await Mediator.Send(command);

            Console.WriteLine("Загрузка алертов завершена успешно!");
            Logger.LogInformation("End IBM Weather Load Alerts");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Ошибка при загрузке алертов");
            Console.WriteLine($"Ошибка при загрузке алертов: {ex.Message}");
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
