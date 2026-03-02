using Masofa.BusinessLogic.Extentions;
using Masofa.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.BusinessLogic.Common.History
{
    public class HistoryBaseUpdateEventHandler<TModel, TDbContext> : INotificationHandler<BaseUpdateEvent<TModel, TDbContext>>
        where TModel : BaseEntity
        where TDbContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;

        public HistoryBaseUpdateEventHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Handle(BaseUpdateEvent<TModel, TDbContext> notification, CancellationToken cancellationToken)
        {
            var historyEntityType = typeof(BaseHistoryEntity<>).Assembly.GetTypes().FirstOrDefault(t => t.IsSubclassOf(typeof(BaseHistoryEntity<TModel>)));
            if (historyEntityType == null)
            {
                return;
            }

            var historyContextType = historyEntityType.GetDbContextTypeForEntity();
            if (historyContextType == null)
            {
                return;
            }

            if (Activator.CreateInstance(historyEntityType) is not BaseHistoryEntity<TModel> historyEntry)
            {
                return;
            }

            await using var scope = _serviceProvider.CreateAsyncScope();
            if (scope.ServiceProvider.GetService(historyContextType) is not DbContext historyContext)
            {
                return;
            }

            var setMethod = typeof(DbContext).GetMethods()
                .First(m => m.Name == "Set" && m.GetParameters().Length == 0);
            var genericSetMethod = setMethod.MakeGenericMethod(historyEntityType);
            var dbSet = genericSetMethod.Invoke(historyContext, null) as dynamic;
            if (dbSet == null)
            {
                return;
            }

            var addMethod = dbSet.GetType().GetMethod("Add", new[] { historyEntityType });
            if (addMethod == null)
            {
                return;
            }

            var newEntity = notification.CurrentModel;
            var oldEntity = notification.OldModel;
            historyEntry.OwnerId = newEntity.Id;
            historyEntry.NewModel = newEntity;
            historyEntry.OldModel = oldEntity;

            addMethod.Invoke(dbSet, new[] { historyEntry });

            await historyContext.SaveChangesAsync(cancellationToken);
        }
    }
}
