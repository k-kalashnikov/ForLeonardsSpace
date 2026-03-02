using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Identity;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Identity.Roles
{
    /// <summary>
    /// Модель пользователя с полным именем для ролей
    /// </summary>
    public class UserFullName
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// Модель роли с пользователями
    /// </summary>
    public class RoleWithUsers
    {
        public LocalizationString RoleName { get; set; }
        public List<UserFullName> Users { get; set; } = new List<UserFullName>();
    }

    /// <summary>
    /// Запрос для получения всех ролей с пользователями в одном запросе
    /// </summary>
    [RequestPermission(ActionType = "Read")]
    public class RoleWithUsersGetRequest : IRequest<List<RoleWithUsers>>
    {
    }

    public class RoleWithUsersGetRequestHandler : IRequestHandler<RoleWithUsersGetRequest, List<RoleWithUsers>>
    {
        private MasofaIdentityDbContext MasofaIdentityDbContext { get; set; }
        private ILogger<RoleWithUsersGetRequestHandler> Logger { get; set; }
        private UserManager<User> UserManager { get; set; }
        private RoleManager<Role> RoleManager { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public RoleWithUsersGetRequestHandler(
            MasofaIdentityDbContext masofaIdentityDbContext, 
            ILogger<RoleWithUsersGetRequestHandler> logger,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IBusinessLogicLogger businessLogicLogger)
        {
            MasofaIdentityDbContext = masofaIdentityDbContext;
            Logger = logger;
            UserManager = userManager;
            RoleManager = roleManager;
            BusinessLogicLogger = businessLogicLogger;
        }

        public async Task<List<RoleWithUsers>> Handle(RoleWithUsersGetRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                
                var roles = await RoleManager.Roles.ToListAsync(cancellationToken);
                var result = new List<RoleWithUsers>();

                foreach (var role in roles)
                {
                    var usersInRole = await UserManager.GetUsersInRoleAsync(role.Name ?? string.Empty);
                    
                    var roleWithUsers = new RoleWithUsers
                    {
                        RoleName = role.Names,
                        Users = usersInRole.Select(u => new UserFullName
                        {
                            Id = u.Id,
                            Email = u.Email,
                            FullName = string.IsNullOrEmpty(u.FirstName) && string.IsNullOrEmpty(u.LastName) 
                                ? u.UserName 
                                : $"{u.FirstName} {u.LastName}".Trim()
                        }).ToList()
                    };
                    
                    result.Add(roleWithUsers);
                }

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.Count.ToString()), requestPath);
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
}
