using Masofa.BusinessLogic.FieldSatellite.Commands;
using Masofa.DataAccess;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Masofa.Common.Helper;

namespace Masofa.BusinessLogic.CropMonitoring.Fields
{
    /// <summary>
    /// Обработчик событий обновления поля
    /// </summary>
    public class FieldUpdateEventHandler : INotificationHandler<BaseUpdateEvent<Masofa.Common.Models.CropMonitoring.Field, MasofaCropMonitoringDbContext>>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<FieldUpdateEventHandler> _logger;
        private readonly IConfiguration _configuration;
        private readonly MasofaDictionariesDbContext _dictionariesContext;
        private readonly MasofaCropMonitoringDbContext _cropMonitoringContext;

        public FieldUpdateEventHandler(
            IMediator mediator,
            ILogger<FieldUpdateEventHandler> logger,
            IConfiguration configuration,
            MasofaDictionariesDbContext dictionariesContext,
            MasofaCropMonitoringDbContext cropMonitoringContext)
        {
            _mediator = mediator;
            _logger = logger;
            _configuration = configuration;
            _dictionariesContext = dictionariesContext;
            _cropMonitoringContext = cropMonitoringContext;
        }

        public async Task Handle(BaseUpdateEvent<Masofa.Common.Models.CropMonitoring.Field, MasofaCropMonitoringDbContext> notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Обработка события обновления поля: {FieldId}", notification.CurrentModel.Id);

                var isWithinMainRegion = GeometryCalculationHelper.IsFieldWithinCountryBoundaries(notification.CurrentModel, _configuration);

                if (!isWithinMainRegion)
                {
                    _logger.LogWarning("Поле {FieldId} не входит в главный регион, помечаем как Hidden", notification.CurrentModel.Id);
                    
                    var fieldToUpdate = await _cropMonitoringContext.Fields
                        .FirstOrDefaultAsync(f => f.Id == notification.CurrentModel.Id, cancellationToken);
                    
                    if (fieldToUpdate != null)
                    {
                        fieldToUpdate.Status = Masofa.Common.Models.StatusType.Hiden;
                        await _cropMonitoringContext.SaveChangesAsync(cancellationToken);
                        _logger.LogInformation("Поле {FieldId} помечено как Hidden", notification.CurrentModel.Id);
                    }
                    return;
                }

                if (ShouldTriggerRecalculation(notification.CurrentModel, notification.OldModel))
                {
                    await HandleRecalculationAsync(notification.CurrentModel, notification.OldModel, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке события обновления поля: {FieldId}", notification.CurrentModel.Id);
                throw;
            }
        }

        private bool ShouldTriggerRecalculation(Masofa.Common.Models.CropMonitoring.Field field, Masofa.Common.Models.CropMonitoring.Field originalField)
        {
            if (field.Polygon == null && originalField.Polygon == null)
                return false;

            if (field.Polygon == null || originalField.Polygon == null)
                return true;

            return !field.Polygon.Equals(originalField.Polygon);
        }

        private async Task HandleRecalculationAsync(Masofa.Common.Models.CropMonitoring.Field field, Masofa.Common.Models.CropMonitoring.Field originalField, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Пересчет маппинга продуктов при изменении поля {FieldId}", field.Id);

            // Пересчитываем маппинг для изменённого поля
            await _mediator.Send(new RecalculateFieldProductsCommand(field.Id, cancellationToken), cancellationToken);

            // Существующий код пересчёта глобальных границ
            await _mediator.Send(new RecalculateGlobalBoundariesCommand(cancellationToken), cancellationToken);
        }

    }
}
