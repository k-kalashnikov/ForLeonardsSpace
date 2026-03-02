using Masofa.Common.Models.Identity;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Masofa.BusinessLogic.Identity.LockPermissionHandler
{
    /// <summary>
    /// Запрос для сохранения блокировок пользователя
    /// </summary>
    public class SaveUserLockPermissionsRequest : IRequest<bool>
    {
        public SaveUserLockPermissionsDto Data { get; set; } = new();
    }

    /// <summary>
    /// Обработчик для сохранения блокировок пользователя
    /// </summary>
    public class SaveUserLockPermissionsRequestHandler : IRequestHandler<SaveUserLockPermissionsRequest, bool>
    {
        private readonly MasofaIdentityDbContext _dbContext;

        public SaveUserLockPermissionsRequestHandler(MasofaIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(SaveUserLockPermissionsRequest request, CancellationToken cancellationToken)
        {
            var userId = request.Data.UserId;
            var entityPermissions = request.Data.EntityPermissions;

            // Получаем существующие блокировки для пользователя
            var existingPermissions = await _dbContext.LockPermissions
                .Where(lp => lp.UserId == userId)
                .ToListAsync(cancellationToken);

            // Создаем список операций для выполнения
            var operationsToDelete = new List<LockPermission>();
            var operationsToCreate = new List<LockPermission>();

            // Проходим по каждой сущности в запросе
            foreach (var entityPermission in entityPermissions)
            {
                var entityTypeName = entityPermission.EntityTypeName;
                var lockPermissionType = entityPermission.LockPermissionType;

                // Находим существующие блокировки для этой сущности
                var existingForEntity = existingPermissions
                    .Where(lp => lp.EntityTypeName == entityTypeName)
                    .ToList();

                if (lockPermissionType == 1 && entityPermission.EntityBlocked)
                {
                    // Блокировка всей сущности
                    if (!existingForEntity.Any(lp => lp.LockPermissionType == LockPermissionType.Entity))
                    {
                        operationsToCreate.Add(new LockPermission
                        {
                            Id = Guid.NewGuid(),
                            UserId = userId,
                            EntityTypeName = entityTypeName,
                            LockPermissionType = LockPermissionType.Entity,
                            EntityAction = null,
                            EntityId = null,
                            CreateAt = DateTime.UtcNow,
                            Status = Masofa.Common.Models.StatusType.Active,
                            LastUpdateAt = DateTime.UtcNow,
                            CreateUser = userId,
                            LastUpdateUser = userId
                        });
                    }
                    // Удаляем все остальные блокировки для этой сущности
                    operationsToDelete.AddRange(existingForEntity.Where(lp => lp.LockPermissionType != LockPermissionType.Entity));
                }
                else if (lockPermissionType == 2 && entityPermission.InstanceBlocked)
                {
                    // Блокировка конкретных экземпляров
                    if (!existingForEntity.Any(lp => lp.LockPermissionType ==  LockPermissionType.Instance))
                    {
                        operationsToCreate.Add(new LockPermission
                        {
                            Id = Guid.NewGuid(),
                            UserId = userId,
                            EntityTypeName = entityTypeName,
                            LockPermissionType = LockPermissionType.Instance,
                            EntityAction = null,
                            EntityId = null,
                            CreateAt = DateTime.UtcNow,
                            Status = Masofa.Common.Models.StatusType.Active,
                            LastUpdateAt = DateTime.UtcNow,
                            CreateUser = userId,
                            LastUpdateUser = userId
                        });
                    }
                    // Удаляем все остальные блокировки для этой сущности
                    operationsToDelete.AddRange(existingForEntity.Where(lp => lp.LockPermissionType != LockPermissionType.Instance));
                }
                else if (lockPermissionType == 0 && entityPermission.BlockedActions.Any())
                {
                    // Блокировка конкретных действий
                    var existingActions = existingForEntity
                        .Where(lp => lp.LockPermissionType == 0)
                        .Select(lp => lp.EntityAction)
                        .ToList();

                    // Добавляем новые действия
                    foreach (var action in entityPermission.BlockedActions)
                    {
                        if (!existingActions.Contains(action))
                        {
                            operationsToCreate.Add(new LockPermission
                            {
                                Id = Guid.NewGuid(),
                                UserId = userId,
                                EntityTypeName = entityTypeName,
                                LockPermissionType = 0,
                                EntityAction = action,
                                EntityId = null,
                                CreateAt = DateTime.UtcNow,
                                Status = Masofa.Common.Models.StatusType.Active,
                                LastUpdateAt = DateTime.UtcNow,
                                CreateUser = userId,
                                LastUpdateUser = userId
                            });
                        }
                    }

                    // Удаляем действия, которые больше не заблокированы
                    var actionsToDelete = existingActions.Except(entityPermission.BlockedActions).ToList();
                    operationsToDelete.AddRange(existingForEntity
                        .Where(lp => lp.LockPermissionType == 0 && actionsToDelete.Contains(lp.EntityAction)));
                }
                else
                {
                    // Если ничего не выбрано, удаляем все блокировки для этой сущности
                    operationsToDelete.AddRange(existingForEntity);
                }
            }

            // Удаляем блокировки для сущностей, которых нет в запросе
            var requestedEntityTypes = entityPermissions.Select(ep => ep.EntityTypeName).ToList();
            var entitiesToRemove = existingPermissions
                .Where(lp => !requestedEntityTypes.Contains(lp.EntityTypeName))
                .ToList();
            operationsToDelete.AddRange(entitiesToRemove);

            // Выполняем операции удаления
            if (operationsToDelete.Any())
            {
                _dbContext.LockPermissions.RemoveRange(operationsToDelete);
            }

            // Выполняем операции создания
            if (operationsToCreate.Any())
            {
                await _dbContext.LockPermissions.AddRangeAsync(operationsToCreate, cancellationToken);
            }

            // Сохраняем изменения
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
