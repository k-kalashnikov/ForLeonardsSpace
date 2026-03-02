using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Identity.Roles;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Identity;
using Masofa.Common.ViewModels.Account;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Masofa.Web.Monolith.Controllers.Identity
{
    /// <summary>
    /// Provides endpoints for managing roles and retrieving role-related data in the system.
    /// </summary>
    /// <remarks>This controller includes actions for retrieving users assigned to specific roles,  fetching
    /// all roles in the system, and retrieving roles along with their associated users. All actions require
    /// authentication using the JWT Bearer scheme.</remarks>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("identity/[controller]")]
    [ApiExplorerSettings(GroupName = "Identity")]
    public class RoleController : BaseController
    {
        private UserManager<User> UserManager { get; set; }
        private RoleManager<Role> RoleManager { get; set; }

        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public RoleController(
            ILogger<RoleController> logger,
            IConfiguration configuration,
            IMediator mediator,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IBusinessLogicLogger businessLogicLogger) : base(
                logger,
                configuration,
                mediator)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            BusinessLogicLogger = businessLogicLogger;
        }

        /// <summary>
        /// Получает список пользователей, назначенных на указанную роль
        /// </summary>
        /// <param name="roleName">Название роли</param>
        /// <returns>Список пользователей с указанной ролью</returns>
        /// <response code="200">Список пользователей успешно получен</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">Роль с указанным именем не найдена</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<ProfileInfoViewModel>>> GetUsersInRole(string roleName)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetUsersInRole)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!(await RoleManager.RoleExistsAsync(roleName)))
                {
                    var errorMsg = LogMessageResource.RoleNotFound(roleName);
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }


                var users = (await UserManager.GetUsersInRoleAsync(roleName)).ToList();

                var result = new List<ProfileInfoViewModel>();
                foreach (var user in users) 
                {
                    var temp = new ProfileInfoViewModel()
                    {
                        Id = user.Id,
                        Email = user.Email,
                        UserName = user.UserName,
                        Roles = (await UserManager.GetRolesAsync(user)).ToList(),
                        FirstName= user.FirstName,
                        LastName= user.LastName
                    };
                    result.Add(temp);
                }

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.Count.ToString()), requestPath);
                return result;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Получает список всех ролей в системе
        /// </summary>
        /// <returns>Список всех ролей</returns>
        /// <response code="200">Список ролей успешно получен</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Role>>> GetRoles()
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetRoles)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var roles = await RoleManager.Roles.ToListAsync();
                await BusinessLogicLogger.LogInformationAsync($"Finish request in {requestPath} with result: {Newtonsoft.Json.JsonConvert.SerializeObject(roles)}", requestPath);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Получает список всех ролей с пользователями в одном запросе
        /// </summary>
        /// <returns>Список ролей с пользователями</returns>
        /// <response code="200">Список ролей успешно получен</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<RoleWithUsers>>> GetRolesWithUsers()
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetRolesWithUsers)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                
                var result = await Mediator.Send(new RoleWithUsersGetRequest());
                
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.Count.ToString()), requestPath);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
