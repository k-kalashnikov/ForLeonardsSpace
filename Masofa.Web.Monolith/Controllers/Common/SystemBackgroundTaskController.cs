using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Services;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Attributes;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using Masofa.Web.Monolith.Helpers;
using Masofa.Web.Monolith.Jobs;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Masofa.Web.Monolith.Controllers.Common
{
    /// <summary>
    /// Provides API endpoints for managing and retrieving information about system background tasks.
    /// </summary>
    /// <remarks>This controller is part of the "Common" API group and requires authentication using the JWT
    /// Bearer scheme. It is accessible to users with the roles "Admin", "SystemAdmin", "ModuleAdmin", or
    /// "Operator".</remarks>
    [Route("common/[controller]")]
    [ApiExplorerSettings(GroupName = "Common")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
    public class SystemBackgroundTaskController : BaseCrudController<SystemBackgroundTask, MasofaCommonDbContext>
    {
        public SystemBackgroundTaskController(
            IFileStorageProvider fileStorageProvider,
            MasofaCommonDbContext dbContext,
            ILogger<SystemBackgroundTaskController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            IHttpContextAccessor httpContextAccessor) : base(fileStorageProvider, dbContext, logger, configuration, mediator, businessLogicLogger, httpContextAccessor)
        {

        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<PosibleExecuteType>>> GetPosibleExecuteTypes()
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetPosibleExecuteTypes)}";

            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var result = new List<PosibleExecuteType>();
                var jobTypes = typeof(BaseJob<>).Assembly
                    .GetTypes()
                    .Where(m => m.GetCustomAttribute<SystemBackgroundTaskAttribute>() != null)
                    .ToList();

                var jobResult = jobTypes.Select(m =>
                {
                    var attribute = m.GetCustomAttribute<SystemBackgroundTaskAttribute>();
                    var localizedNames = BackgroundTaskLocalization.GetLocalization(attribute.LocalizationKey);

                    return new PosibleExecuteType()
                    {
                        ExecuteTypeName = m.FullName,
                        GroupName = "jobs",
                        Names = localizedNames,
                        TaskType = SystemBackgroundTaskType.Schedule,
                        ParametrsJsonSchema = new List<ParametrsJsonSchemaFields>()
                    };
                }).ToList();

                result.AddRange(jobResult);
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
        
        private List<ParametrsJsonSchemaFields> GetParametrsJsonSchemaFields<TParams>()
        {
            return new List<ParametrsJsonSchemaFields>();
        }
    }
}
