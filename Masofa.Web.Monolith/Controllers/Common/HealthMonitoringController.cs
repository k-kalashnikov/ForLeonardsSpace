using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Masofa.Web.Monolith.Controllers.Common
{
    /// <summary>
    /// Provides endpoints for monitoring the health of the system.
    /// </summary>
    /// <remarks>This controller is part of the "Common" API group and is accessible to users with the "Admin"
    /// role. It includes functionality to retrieve health check results for system monitoring purposes.</remarks>
    [Route("common/[controller]")]
    [ApiExplorerSettings(GroupName = "Common")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class HealthMonitoringController : BaseController
    {
        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
        public HealthMonitoringController(ILogger<HealthMonitoringController> logger, IConfiguration configuration, IMediator mediator, MasofaCommonDbContext masofaCommonDbContext) : base(logger, configuration, mediator)
        {
            MasofaCommonDbContext = masofaCommonDbContext;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("[action]")]
        public async Task<ActionResult<List<Masofa.Common.Models.SystemCrical.HealthCheckResult>>> GetHealthCheck()
        {
            try
            {
                return await MasofaCommonDbContext.HealthCheckResults.Where(m => m.DateTime >= DateTime.UtcNow.AddDays(-7)).ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
