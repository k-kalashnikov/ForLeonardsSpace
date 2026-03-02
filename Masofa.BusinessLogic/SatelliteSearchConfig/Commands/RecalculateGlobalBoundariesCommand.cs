using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using MediatR;
using Masofa.DataAccess;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Satellite;
using Microsoft.EntityFrameworkCore;
using Masofa.Common.Helper;

namespace Masofa.BusinessLogic.FieldSatellite.Commands
{
    /// <summary>
    /// Команда для пересчета глобальных границ поиска по всем полям
    /// </summary>
    public class RecalculateGlobalBoundariesCommand : IRequest
    {
        public CancellationToken CancellationToken { get; set; } = default;

        public RecalculateGlobalBoundariesCommand(CancellationToken cancellationToken = default)
        {
            CancellationToken = cancellationToken;
        }
    }

    /// <summary>
    /// Обработчик команды для пересчета глобальных границ поиска
    /// </summary>
    public class RecalculateGlobalBoundariesHandler : IRequestHandler<RecalculateGlobalBoundariesCommand>
    {
        private readonly MasofaCropMonitoringDbContext _cropMonitoringContext;
        private readonly IBusinessLogicLogger _logger;

        public RecalculateGlobalBoundariesHandler(
            MasofaCropMonitoringDbContext cropMonitoringContext,
            IBusinessLogicLogger logger)
        {
            _cropMonitoringContext = cropMonitoringContext;
            _logger = logger;
        }

        public async Task Handle(RecalculateGlobalBoundariesCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";

            try
            {
                _logger.LogInformationAsync("LogMessageResource.GlobalBoundariesRecalculationStarted()", requestPath);

                // Получаем все поля с полигонами
                var fields = await _cropMonitoringContext.Fields
                    .Where(f => f.Polygon != null && f.Status == Masofa.Common.Models.StatusType.Active)
                    .ToListAsync(request.CancellationToken);

                if (!fields.Any())
                {
                    _logger.LogWarningAsync("LogMessageResource.NoFieldsForGlobalBoundaries()", requestPath);
                    return;
                }

                // Деактивируем старые конфигурации
                await DeactivateOldConfigsAsync(request.CancellationToken);

                // Создаем новую конфигурацию
                var newConfig = new SatelliteSearchConfig
                {
                    Id = Guid.NewGuid(),
                    CreateAt = DateTime.UtcNow,
                    IsActive = true,
                    FieldsCount = fields.Count
                };

                // Получаем полигоны полей
                var fieldPolygons = fields.Select(f => f.Polygon).Where(p => p != null).ToList();

                // Создаем глобальную область поиска
                var globalPolygon = GeometryCalculationHelper.CreateGlobalSearchArea(fieldPolygons);

                if (globalPolygon != null)
                {
                    newConfig.SentinelPolygon = globalPolygon;

                    // Вычисляем MBR для Landsat
                    var (lowerLeft, upperRight) = GeometryCalculationHelper.CalculateLandsatMBR(globalPolygon);
                    newConfig.LandsatLeftDown = lowerLeft;
                    newConfig.LandsatRightUp = upperRight;

                    // Сохраняем конфигурацию
                    _cropMonitoringContext.SatelliteSearchConfigs.Add(newConfig);
                    await _cropMonitoringContext.SaveChangesAsync(request.CancellationToken);

                    _logger.LogInformationAsync(LogMessageResource.GlobalBoundariesRecalculated(fields.Count), requestPath);
                }
                else
                {
                    _logger.LogErrorAsync("LogMessageResource.GenericError(requestPath, ex.Message)", requestPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogErrorAsync(LogMessageResource.GenericError(requestPath, ex.Message), requestPath);
                throw;
            }
        }

        private async Task DeactivateOldConfigsAsync(CancellationToken cancellationToken)
        {
            var oldConfigs = await _cropMonitoringContext.SatelliteSearchConfigs
                .Where(c => c.IsActive)
                .ToListAsync(cancellationToken);

            foreach (var config in oldConfigs)
            {
                config.IsActive = false;
            }

            await _cropMonitoringContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformationAsync(LogMessageResource.OldConfigsDeactivated(oldConfigs.Count), "RecalculateGlobalBoundariesHandler.DeactivateOldConfigsAsync");
        }
    }
}
