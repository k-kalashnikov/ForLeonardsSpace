using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Era;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Era;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Masofa.Web.Monolith.Controllers.Era
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("weather/[controller]")]
    [ApiExplorerSettings(GroupName = "Weather")]
    public class EraWeatherDataController : BaseController
    {
        private MasofaEraDbContext EraDbContext { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public EraWeatherDataController(
            ILogger<EraWeatherDataController> logger,
            IConfiguration configuration,
            IMediator mediator,
            MasofaEraDbContext eraDbContext,
            IBusinessLogicLogger businessLogicLogger) : base(
                logger,
                configuration,
                mediator)
        {
            EraDbContext = eraDbContext;
            BusinessLogicLogger = businessLogicLogger;
        }

        [HttpGet]
        [Route("[action]/{id}")]
        public virtual async Task<ActionResult<EraWeatherData>> GetById(Guid id)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetById)}";
            try
            {
                var data = await EraDbContext.EraWeatherData.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);

                if (data == null)
                {
                    return NotFound($"EraWeatherData with Id = {id.ToString()} not found");
                }

                return Ok(data);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public virtual async Task<ActionResult<List<EraWeatherData>>> GetByQuery([FromBody] EraWeatherDataGetQuery query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetByQuery)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(query)), requestPath);
                    return BadRequest(ModelState);
                }
                var getRequest = new EraWeatherDataGetRequest()
                {
                    Query = query
                };
                return await Mediator.Send(getRequest);
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
