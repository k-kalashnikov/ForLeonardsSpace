using Masofa.BusinessLogic.Identity.LockPermissionHandler;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Identity;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Identity
{
    /// <summary>
    /// Контроллер для работы с запретами для пользователей
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin")]
    [Route("identity/[controller]")]
    [ApiExplorerSettings(GroupName = "Identity")]
    public class LockPermissionController : BaseCrudController<LockPermission, MasofaIdentityDbContext>
    {
        public LockPermissionController(
            IFileStorageProvider fileStorageProvider,
            MasofaIdentityDbContext dbContext,
            ILogger<LockPermissionController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            IHttpContextAccessor httpContextAccessor) : base(
                fileStorageProvider,
                dbContext,
                logger,
                configuration,
                mediator,
                businessLogicLogger,
                httpContextAccessor)
        {
        }

        /// <summary>
        /// Получить группированные LockPermission по пользователям
        /// </summary>
        [HttpPost("grouped")]
        public async Task<ActionResult<GetLockPermissionGroupedResponse>> GetGroupedLockPermissions([FromBody] GetLockPermissionGroupedRequest request)
        {
            var result = await Mediator.Send(request);
            return Ok(result);
        }

        /// <summary>
        /// Сохранить модель блокировок для пользователя
        /// </summary>
        [HttpPost("save-user-permissions")]
        public async Task<ActionResult> SaveUserLockPermissions([FromBody] SaveUserLockPermissionsDto data)
        {
            var request = new SaveUserLockPermissionsRequest
            {
                Data = data
            };
            var result = await Mediator.Send(request);
            return Ok(result);
        }
    }
}
