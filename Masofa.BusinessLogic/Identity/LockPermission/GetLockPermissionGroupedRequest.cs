using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Identity;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Identity.LockPermissionHandler
{
    /// <summary>
    /// Запрос для получения группированных LockPermission по пользователям
    /// </summary>
    [RequestPermission(ActionType = "Read")]
    public class GetLockPermissionGroupedRequest : IRequest<GetLockPermissionGroupedResponse>
    {
        /// <summary>
        /// Количество записей для получения
        /// </summary>
        public int Take { get; set; } = 10;

        /// <summary>
        /// Смещение для пагинации
        /// </summary>
        public int Offset { get; set; } = 0;

        /// <summary>
        /// Фильтр по ID пользователя
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Фильтр по типу сущности
        /// </summary>
        public string? EntityTypeName { get; set; }

        /// <summary>
        /// Фильтр по действию
        /// </summary>
        public string? EntityAction { get; set; }

        /// <summary>
        /// Фильтр по типу разрешения
        /// </summary>
        public LockPermissionType? LockPermissionType { get; set; }

        /// <summary>
        /// Поиск по имени пользователя
        /// </summary>
        public string? UserNameSearch { get; set; }
    }

    /// <summary>
    /// Обработчик запроса группированных LockPermission
    /// </summary>
    public class GetLockPermissionGroupedRequestHandler : IRequestHandler<GetLockPermissionGroupedRequest, GetLockPermissionGroupedResponse>
    {
        private readonly MasofaIdentityDbContext _dbContext;
        private readonly ILogger<GetLockPermissionGroupedRequestHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public GetLockPermissionGroupedRequestHandler(
            MasofaIdentityDbContext dbContext,
            ILogger<GetLockPermissionGroupedRequestHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<GetLockPermissionGroupedResponse> Handle(GetLockPermissionGroupedRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                
                _logger.LogInformation("Getting grouped lock permissions with filters: UserId={UserId}, EntityTypeName={EntityTypeName}, EntityAction={EntityAction}, LockPermissionType={LockPermissionType}, UserNameSearch={UserNameSearch}",
                    request.UserId, request.EntityTypeName, request.EntityAction, request.LockPermissionType, request.UserNameSearch);

                // Базовый запрос с фильтрами
                var query = _dbContext.LockPermissions.AsQueryable();

                // Применяем фильтры
                if (request.UserId.HasValue)
                {
                    query = query.Where(lp => lp.UserId == request.UserId.Value);
                }

                if (!string.IsNullOrEmpty(request.EntityTypeName))
                {
                    query = query.Where(lp => lp.EntityTypeName.Contains(request.EntityTypeName));
                }

                if (!string.IsNullOrEmpty(request.EntityAction))
                {
                    query = query.Where(lp => lp.EntityAction != null && lp.EntityAction.Contains(request.EntityAction));
                }

                if (request.LockPermissionType.HasValue)
                {
                    query = query.Where(lp => lp.LockPermissionType == request.LockPermissionType.Value);
                }

                // Фильтр по имени пользователя через JOIN
                if (!string.IsNullOrEmpty(request.UserNameSearch))
                {
                    query = query.Where(lp => _dbContext.Users.Any(u => u.Id == lp.UserId && 
                        (u.UserName.Contains(request.UserNameSearch) || 
                         u.Email.Contains(request.UserNameSearch) ||
                         (u.FirstName + " " + u.LastName).Contains(request.UserNameSearch))));
                }

                // Получаем уникальных пользователей с учетом фильтров
                var userIds = await query
                    .Select(lp => lp.UserId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                // Общее количество уникальных пользователей
                var totalUsers = userIds.Count;

                // Применяем пагинацию к пользователям (не к запретам!)
                var pagedUserIds = userIds
                    .Skip(request.Offset)
                    .Take(request.Take)
                    .ToList();

                // Получаем все запреты для выбранных пользователей
                var lockPermissions = await _dbContext.LockPermissions
                    .Where(lp => pagedUserIds.Contains(lp.UserId))
                    .OrderBy(lp => lp.UserId)
                    .ThenBy(lp => lp.EntityTypeName)
                    .ThenBy(lp => lp.EntityAction)
                    .ToListAsync(cancellationToken);

                // Группируем по пользователям
                var groupedData = new List<LockPermissionGroupedDto>();

                foreach (var userId in pagedUserIds)
                {
                    var userPermissions = lockPermissions.Where(lp => lp.UserId == userId).ToList();
                    
                    // Получаем информацию о пользователе
                    var user = await _dbContext.Users
                        .Where(u => u.Id == userId)
                        .Select(u => new UserInfo
                        {
                            Id = u.Id,
                            UserName = u.UserName,
                            Email = u.Email,
                            FullName = $"{u.FirstName} {u.LastName}".Trim()
                        })
                        .FirstOrDefaultAsync(cancellationToken);

                    if (user != null)
                    {
                        groupedData.Add(new LockPermissionGroupedDto
                        {
                            User = user,
                            Permissions = userPermissions,
                            TotalPermissions = userPermissions.Count
                        });
                    }
                }

                var response = new GetLockPermissionGroupedResponse
                {
                    Data = groupedData,
                    TotalRecords = totalUsers, // Количество пользователей, не записей!
                    PageSize = request.Take,
                    CurrentPage = (request.Offset / request.Take) + 1
                };

                _logger.LogInformation("Found {Count} users with lock permissions (Total: {TotalUsers} users)", 
                    groupedData.Count, totalUsers);

                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath,"result"), requestPath);
                return response;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                _logger.LogError(ex, "Error while getting grouped lock permissions");
                throw;
            }
        }
    }
}