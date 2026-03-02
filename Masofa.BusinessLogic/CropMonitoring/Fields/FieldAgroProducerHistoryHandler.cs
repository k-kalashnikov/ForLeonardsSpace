using Masofa.Common.Models.CropMonitoring;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.BusinessLogic.CropMonitoring.Fields
{
    /// <summary>
    /// Обработчик событий обновления поля, для автоматического создания FieldAgroProducerHistory.
    /// </summary>
    public class FieldAgroProducerHistoryHandler : INotificationHandler<BaseUpdateEvent<Field, MasofaCropMonitoringDbContext>>
    {
        private readonly MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext;

        public FieldAgroProducerHistoryHandler(MasofaCropMonitoringDbContext dbContext)
        {
            MasofaCropMonitoringDbContext = dbContext;
        }

        public async Task Handle(BaseUpdateEvent<Field, MasofaCropMonitoringDbContext> notification, CancellationToken cancellationToken)
        {
            var oldModel = notification.OldModel;
            var currentModel = notification.CurrentModel;

            if (oldModel.AgricultureProducerId == currentModel.AgricultureProducerId)
                return;

            if (oldModel.AgricultureProducerId == null)
                return;

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            DateOnly periodStart;

            var lastHistoryRecord = await MasofaCropMonitoringDbContext.Set<FieldAgroProducerHistory>()
                .Where(h => h.FieldId == currentModel.Id)
                .OrderByDescending(h => h.PeriodEnd)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastHistoryRecord != null)
            {
                periodStart = lastHistoryRecord.PeriodEnd.AddDays(1);
            }
            else
            {
                periodStart = DateOnly.FromDateTime(oldModel.CreateAt);
            }

            var historyEntry = new FieldAgroProducerHistory
            {
                FieldId = currentModel.Id,
                AgricultureProducerId = oldModel.AgricultureProducerId.Value,
                PeriodStart = periodStart,
                PeriodEnd = today
            };

            await MasofaCropMonitoringDbContext.FieldAgroProducerHistories.AddAsync(historyEntry, cancellationToken);
            await MasofaCropMonitoringDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
