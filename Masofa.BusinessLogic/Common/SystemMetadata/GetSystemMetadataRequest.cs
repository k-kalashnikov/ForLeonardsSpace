using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.SystemMetadata;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Masofa.BusinessLogic.Common.SystemMetadata
{
    /// <summary>
    /// Запрос для получения метаданных системы
    /// </summary>
    [RequestPermission(ActionType = "Read")]
    public class GetSystemMetadataRequest : IRequest<SystemMetadataDto>
    {
    }

    /// <summary>
    /// Обработчик запроса метаданных системы
    /// </summary>
    public class GetSystemMetadataRequestHandler : IRequestHandler<GetSystemMetadataRequest, SystemMetadataDto>
    {
        private readonly ILogger<GetSystemMetadataRequestHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public GetSystemMetadataRequestHandler(
            ILogger<GetSystemMetadataRequestHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<SystemMetadataDto> Handle(GetSystemMetadataRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                
                _logger.LogInformation("Starting to collect system metadata...");

                var result = new SystemMetadataDto();

                // Получаем простой список всех сущностей
                result.Entities = GetSystemEntities();

                // Получаем простой список всех действий
                result.Actions = GetSystemActions();

                // Получаем типы разрешений
                result.PermissionTypes = GetPermissionTypes();

                _logger.LogInformation($"Found {result.Entities.Count} entities and {result.Actions.Count} actions");

                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath,result.ToString()), requestPath);
                return result;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                _logger.LogError(ex, "Error while collecting system metadata");
                throw;
            }
        }

        /// <summary>
        /// Получить простой список всех сущностей
        /// </summary>
        private List<EntityInfo> GetSystemEntities()
        {
            var entities = new List<EntityInfo>();

            try
            {
                // Получаем все сборки, которые могут содержать модели
                var assemblies = new[]
                {
                    typeof(Masofa.Common.Models.BaseEntity).Assembly, // Masofa.Common.Models
                    typeof(Masofa.DataAccess.MasofaIdentityDbContext).Assembly // Masofa.DataAccess
                };

                foreach (var assembly in assemblies)
                {
                    _logger.LogInformation($"Scanning assembly: {assembly.FullName}");

                    // Получаем все типы, которые наследуются от BaseEntity
                    var entityTypes = assembly.GetTypes()
                        .Where(t => t.IsClass && 
                                   !t.IsAbstract && 
                                   t.IsSubclassOf(typeof(Masofa.Common.Models.BaseEntity)))
                        .ToList();

                    _logger.LogInformation($"Found {entityTypes.Count} entity types in assembly {assembly.GetName().Name}");

                    foreach (var entityType in entityTypes)
                    {
                        var entityInfo = new EntityInfo
                        {
                            FullName = entityType.FullName ?? string.Empty,
                            Name = entityType.Name,
                            DisplayName = GetEntityDisplayName(entityType),
                            CanBeLocked = CanEntityBeLocked(entityType)
                        };

                        entities.Add(entityInfo);
                    }
                }

                // Сортируем по отображаемому имени
                entities = entities.OrderBy(e => e.DisplayName).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while scanning entities");
            }

            return entities;
        }

        /// <summary>
        /// Получить простой список всех действий из зарегистрированных MediatR хендлеров
        /// </summary>
        private List<ActionInfo> GetSystemActions()
        {
            var actions = new List<ActionInfo>();

            try
            {
                // Получаем все сборки, которые могут содержать MediatR хендлеры
                var assemblies = new[]
                {
                    // Сканируем сборку бизнес-логики по известному базовому запросу
                    typeof(Masofa.BusinessLogic.BaseGetRequest<,>).Assembly,
                    // И текущую сборку с запросом метаданных
                    typeof(Masofa.BusinessLogic.Common.SystemMetadata.GetSystemMetadataRequest).Assembly
                };

                foreach (var assembly in assemblies)
                {
                    _logger.LogInformation($"Scanning assembly for MediatR handlers: {assembly.FullName}");

                    // Получаем все типы, которые реализуют IRequestHandler
                    var handlerTypes = assembly.GetTypes()
                        .Where(t => t.IsClass && !t.IsAbstract)
                        .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && 
                            (i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                             i.GetGenericTypeDefinition() == typeof(IRequestHandler<>))))
                        .ToList();

                    _logger.LogInformation($"Found {handlerTypes.Count} MediatR handlers in assembly {assembly.GetName().Name}");

                    foreach (var handlerType in handlerTypes)
                    {
                        var interfaces = handlerType.GetInterfaces()
                            .Where(i => i.IsGenericType && 
                                (i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                                 i.GetGenericTypeDefinition() == typeof(IRequestHandler<>)))
                            .ToList();

                        foreach (var interfaceType in interfaces)
                        {
                            var requestType = interfaceType.GetGenericArguments().First();
                            var actionName = ExtractActionName(requestType);
                            
                            if (!string.IsNullOrEmpty(actionName) && !actions.Any(a => a.Name == actionName))
                            {
                                actions.Add(new ActionInfo
                                {
                                    Name = actionName,
                                    DisplayName = actionName
                                });
                            }
                        }
                    }
                }

                // Сортируем по имени
                actions = actions.OrderBy(a => a.Name).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while scanning MediatR handlers for actions");
                
                // Возвращаем базовые действия в случае ошибки
                actions = new List<ActionInfo>
                {
                    new ActionInfo { Name = "Read", DisplayName = "Read" },
                    new ActionInfo { Name = "Create", DisplayName = "Create" },
                    new ActionInfo { Name = "Update", DisplayName = "Update" },
                    new ActionInfo { Name = "Delete", DisplayName = "Delete" }
                };
            }

            return actions;
        }

        /// <summary>
        /// Получить типы разрешений
        /// </summary>
        private List<PermissionTypeInfo> GetPermissionTypes()
        {
            return new List<PermissionTypeInfo>
            {
                new PermissionTypeInfo
                {
                    Name = "Action",
                    DisplayName = "Action",
                    Description = "Block specific action on entity",
                    RequiresAction = true,
                    RequiresEntityId = false,
                    Value = 0
                },
                new PermissionTypeInfo
                {
                    Name = "Entity",
                    DisplayName = "Entity",
                    Description = "Block entire entity",
                    RequiresAction = false,
                    RequiresEntityId = false,
                    Value = 1
                },
                new PermissionTypeInfo
                {
                    Name = "Instance",
                    DisplayName = "Instance",
                    Description = "Block specific instance of entity",
                    RequiresAction = false,
                    RequiresEntityId = true,
                    Value = 2
                }
            };
        }

        /// <summary>
        /// Проверить, можно ли блокировать эту сущность
        /// </summary>
        private bool CanEntityBeLocked(Type entityType)
        {
            var namespaceName = entityType.Namespace ?? string.Empty;
            var entityName = entityType.Name;
            
            // Нельзя блокировать системные сущности
            if (namespaceName.Contains("Microsoft.AspNetCore.Identity"))
                return false;
            
            // Нельзя блокировать некоторые критические сущности
            var criticalEntities = new[] { "User", "Role", "LockPermission" };
            if (criticalEntities.Contains(entityName))
                return false;
            
            return true;
        }

        /// <summary>
        /// Получить отображаемое название сущности
        /// </summary>
        private string GetEntityDisplayName(Type entityType)
        {
            var entityName = entityType.Name;
            
            // Убираем суффиксы
            if (entityName.EndsWith("Entity"))
                entityName = entityName.Substring(0, entityName.Length - 6);
            if (entityName.EndsWith("Model"))
                entityName = entityName.Substring(0, entityName.Length - 5);
            
            return entityName;
        }


        /// <summary>
        /// Извлечь название действия из типа запроса
        /// </summary>
        private string ExtractActionName(Type requestType)
        {
            var typeName = requestType.Name;
            
            // Убираем суффиксы
            if (typeName.EndsWith("Query"))
                return typeName.Substring(0, typeName.Length - 5);
            if (typeName.EndsWith("Command"))
                return typeName.Substring(0, typeName.Length - 7);
            if (typeName.EndsWith("Request"))
                return typeName.Substring(0, typeName.Length - 7);
            
            return typeName;
        }
    }
}
