using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Identity;
using Masofa.Common.ViewModels.Account;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Identity.Users
{
    /// <summary>
    /// Базовая команда для получения списка пользователей с фильтом сортировкой и отступом
    /// </summary>
    [RequestPermission(ActionType = "Read")]
    public class UserGetRequest : IRequest<List<ProfileInfoViewModel>>
    {
        /// <summary>
        /// Модель запрос
        /// </summary>
        public UserGetQuery Query { get; set; }
    }

    public class UserGetRequestHandler : IRequestHandler<UserGetRequest, List<ProfileInfoViewModel>>
    {
        private MasofaIdentityDbContext MasofaIdentityDbContext { get; set; }
        private ILogger<UserGetRequestHandler> Logger { get; set; }
        private UserManager<User> UserManager { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public UserGetRequestHandler(MasofaIdentityDbContext masofaIdentityDbContext, ILogger<UserGetRequestHandler> logger, UserManager<User> userManager, IBusinessLogicLogger businessLogicLogger)
        {
            MasofaIdentityDbContext = masofaIdentityDbContext;
            Logger = logger;
            UserManager = userManager;
            BusinessLogicLogger = businessLogicLogger;
        }

        public async Task<List<ProfileInfoViewModel>> Handle(UserGetRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                IQueryable<User> resultQuery = MasofaIdentityDbContext.Users;

                if (request.Query.Filters.Any())
                {
                    foreach (var item in request.Query.Filters)
                    {
                        resultQuery = resultQuery
                            .ApplyFiltering(item);
                    }
                }

                if (request.Query.Take.HasValue)
                {
                    resultQuery = resultQuery
                        .Skip(request.Query.Offset)
                        .Take(request.Query.Take.Value);
                }

                if (!string.IsNullOrEmpty(request.Query.SortBy))
                {
                    resultQuery = resultQuery
                        .ApplyOrdering(request.Query.SortBy, request.Query.Sort);
                }
                var result = resultQuery?.ToList()
                    .Select(m => new ProfileInfoViewModel()
                    {
                        Id = m.Id,
                        Email = m.Email,
                        Roles = UserManager.GetRolesAsync(m).Result.ToList(),
                        FirstName = m.FirstName,
                        LastName = m.LastName,
                        UserName = m.UserName,
                        IsActive = m.IsActive,
                        Approved = m.Approved,
                        EmailConfirmed = m.EmailConfirmed,
                        LockoutStart = m.ScheduledLockoutStart?.DateTime,
                        LockoutEnd = m.ScheduledLockoutEnd?.DateTime,
                        UserBusinessType = m.UserBusinessType,
                        CreateAt = m.CreateAt,
                        LastUpdateAt = m.LastUpdateAt,
                        CreateUser = m.CreateUser,
                        LastUpdateUser = m.LastUpdateUser
                    }).ToList()
                    ?? new List<ProfileInfoViewModel>();
                return result;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                throw ex;
            }
        }
    }

    /// <summary>
    /// Модель пользователя для фильтров - содержит только необходимые поля
    /// </summary>
    public class UserFilter
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Запрос для получения пользователей в формате фильтра
    /// </summary>
    [RequestPermission(ActionType = "Read")]
    public class UserFilterGetRequest : IRequest<List<UserFilter>>
    {
    }

    public class UserFilterGetRequestHandler : IRequestHandler<UserFilterGetRequest, List<UserFilter>>
    {
        private MasofaIdentityDbContext MasofaIdentityDbContext { get; set; }
        private ILogger<UserFilterGetRequestHandler> Logger { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public UserFilterGetRequestHandler(MasofaIdentityDbContext masofaIdentityDbContext, ILogger<UserFilterGetRequestHandler> logger, IBusinessLogicLogger businessLogicLogger)
        {
            MasofaIdentityDbContext = masofaIdentityDbContext;
            Logger = logger;
            BusinessLogicLogger = businessLogicLogger;
        }

        public async Task<List<UserFilter>> Handle(UserFilterGetRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                
                var users = await MasofaIdentityDbContext.Users
                    .AsNoTracking()
                    .Select(u => new UserFilter
                    {
                        Id = u.Id,
                        FullName = string.IsNullOrEmpty(u.FirstName) && string.IsNullOrEmpty(u.LastName) 
                            ? u.UserName 
                            : $"{u.FirstName} {u.LastName}".Trim()
                    })
                    .ToListAsync(cancellationToken);

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, users.Count.ToString()), requestPath);
                return users;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                throw ex;
            }
        }
    }
}
