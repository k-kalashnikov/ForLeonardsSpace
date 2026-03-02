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
    /// Обработчик событий создания поля
    /// </summary>
    public class FieldCreateEventHandler : INotificationHandler<BaseCreateEvent<Masofa.Common.Models.CropMonitoring.Field, MasofaCropMonitoringDbContext>>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<FieldCreateEventHandler> _logger;
        private readonly IConfiguration _configuration;
        private readonly MasofaDictionariesDbContext _dictionariesContext;
        private readonly MasofaCropMonitoringDbContext _cropMonitoringContext;

        public FieldCreateEventHandler(
            IMediator mediator,
            ILogger<FieldCreateEventHandler> logger,
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

        public async Task Handle(BaseCreateEvent<Masofa.Common.Models.CropMonitoring.Field, MasofaCropMonitoringDbContext> notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Обработка события создания поля: {FieldId}", notification.Model.Id);

                var isWithinMainRegion = GeometryCalculationHelper.IsFieldWithinCountryBoundaries(notification.Model, _configuration);

                if (!isWithinMainRegion)
                {
                    _logger.LogWarning("Поле {FieldId} не входит в главный регион, помечаем как Hidden", notification.Model.Id);
                    
                    var fieldToUpdate = await _cropMonitoringContext.Fields
                        .FirstOrDefaultAsync(f => f.Id == notification.Model.Id, cancellationToken);
                    
                    if (fieldToUpdate != null)
                    {
                        fieldToUpdate.Status = Masofa.Common.Models.StatusType.Hiden;
                        await _cropMonitoringContext.SaveChangesAsync(cancellationToken);
                        _logger.LogInformation("Поле {FieldId} помечено как Hidden", notification.Model.Id);
                    }
                    return;
                }

                if (notification.Model.Polygon != null)
                {
                    await HandleRecalculationAsync(notification.Model, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке события создания поля: {FieldId}", notification.Model.Id);
                throw;
            }
        }

        private async Task HandleRecalculationAsync(Masofa.Common.Models.CropMonitoring.Field field, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Пересчет маппинга продуктов при создании поля {FieldId}", field.Id);

            // Существующий код пересчёта глобальных границ
            await _mediator.Send(new RecalculateGlobalBoundariesCommand(cancellationToken), cancellationToken);

            // Пересчитываем маппинг для нового поля
            await _mediator.Send(new RecalculateFieldProductsCommand(field.Id, cancellationToken), cancellationToken);
        }

    }
}
