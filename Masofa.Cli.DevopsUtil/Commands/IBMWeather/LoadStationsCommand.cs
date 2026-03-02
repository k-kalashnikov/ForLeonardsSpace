using Masofa.Common.Models.SystemCrical;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Masofa.Cli.DevopsUtil.Commands.IBMWeather;

/// <summary>
/// CLI команда для загрузки метеостанций IBM Weather
/// </summary>
[BaseCommand("IBM Weather Load Stations", "Загрузка метеостанций IBM Weather")]
public class LoadStationsCommand : IBaseCommand
{
    private readonly ILogger<LoadStationsCommand> Logger;
    private readonly IMediator Mediator;

    public LoadStationsCommand(
        ILogger<LoadStationsCommand> logger,
        IMediator mediator)
    {
        Logger = logger;
        Mediator = mediator;
    }

    public async Task Execute()
    {
        Logger.LogInformation("Start IBM Weather Load Stations");

        try
        {
            Console.WriteLine("Запуск загрузки метеостанций IBM Weather...");

            var command = new Masofa.BusinessLogic.IBMWeather.Commands.LoadStationsCommand(
                regionId: null,
                latitude: 41,
                longitude: 69,
                countryCode: null,
                locationType: null);

            await Mediator.Send(command);

            Console.WriteLine("Загрузка метеостанций завершена успешно!");
            Logger.LogInformation("End IBM Weather Load Stations");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Ошибка при загрузке метеостанций");
            Console.WriteLine($"Ошибка при загрузке метеостанций: {ex.Message}");
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
