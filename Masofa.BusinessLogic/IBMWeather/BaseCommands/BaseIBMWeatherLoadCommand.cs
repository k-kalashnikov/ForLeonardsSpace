using Masofa.Common.Resources;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Client.IBMWeather;
using Masofa.DataAccess;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Masofa.BusinessLogic.IBMWeather.BaseCommands;

/// <summary>
/// Базовый класс для всех команд загрузки данных IBM Weather
/// </summary>
public abstract class BaseIBMWeatherLoadCommand : IRequest
{
}

/// <summary>
/// Базовый обработчик для всех команд загрузки данных IBM Weather
/// </summary>
public abstract class BaseIBMWeatherLoadHandler<TCommand> : IRequestHandler<TCommand>
    where TCommand : BaseIBMWeatherLoadCommand
{
    protected readonly IBMWeatherApiUnitOfWork _unitOfWork;
    protected readonly MasofaIBMWeatherDbContext _dbContext;
    protected readonly IBusinessLogicLogger _logger;
    protected readonly IBMWeatherServiceOptions _options;

    protected BaseIBMWeatherLoadHandler(
        IBMWeatherApiUnitOfWork unitOfWork,
        MasofaIBMWeatherDbContext dbContext,
        IBusinessLogicLogger logger,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
        _logger = logger;
        
        // Конфигурация UnitOfWork
        if (!_unitOfWork.IsConfigured)
        {
            // Инициализация опций из конфигурации
            _options = new IBMWeatherServiceOptions
            {
                ApiKey = configuration.GetValue<string>("IBMWeather:ApiKey") ?? throw new InvalidOperationException("IBMWeather:ApiKey not configured"),
                BaseUrl = configuration.GetValue<string>("IBMWeather:BaseUrl") ?? "https://api.weather.com/v3",
                Language = configuration.GetValue<string>("IBMWeather:Language") ?? "en-US",
                Format = configuration.GetValue<string>("IBMWeather:Format") ?? "json",
                Units = configuration.GetValue<string>("IBMWeather:Units") ?? "s"
            };
            _unitOfWork.Configure(_options);
        }
    }

    public abstract Task Handle(TCommand request, CancellationToken cancellationToken);

    /// <summary>
    /// Логирование начала операции
    /// </summary>
    protected async Task LogOperationStartAsync(string operationName, string details = "")
    {
        var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
        await _logger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), $"{GetType().Name}.Handle");
    }

    /// <summary>
    /// Логирование завершения операции
    /// </summary>
    protected async Task LogOperationEndAsync(string operationName, int processedCount = 0, string details = "")
    {
        var message = processedCount > 0 
            ? $"Завершена операция: {operationName}. Обработано: {processedCount} {details}"
            : $"Завершена операция: {operationName} {details}";
        
        await _logger.LogInformationAsync(message, $"{GetType().Name}.Handle");
    }

    /// <summary>
    /// Логирование ошибки
    /// </summary>
    protected async Task LogErrorAsync(string operationName, Exception ex, string details = "")
    {
        await _logger.LogErrorAsync(LogMessageResource.OperationError(operationName,ex.Message,details), $"{GetType().Name}.Handle");
    }

    /// <summary>
    /// Сохранение изменений в БД с обработкой ошибок
    /// </summary>
    protected async Task<int> SaveChangesAsync()
    {
        try
        {
            return await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await LogErrorAsync("SaveChanges", ex);
            throw;
        }
    }
}
