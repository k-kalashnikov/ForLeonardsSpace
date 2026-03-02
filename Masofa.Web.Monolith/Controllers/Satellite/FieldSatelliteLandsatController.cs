using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.CropMonitoring.Fields;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System.Text.Json;

namespace Masofa.Web.Monolith.Controllers
{
    /// <summary>
    /// Контроллер для работы с Landsat спутниковыми продуктами полей
    /// </summary>
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
    [Route("satellite/[controller]")]
    [ApiExplorerSettings(GroupName = "SatelliteLandsat")]
    public class FieldSatelliteLandsatController : BaseController
    {
        private MasofaCropMonitoringDbContext CropMonitoringDbContext { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private readonly ISchedulerFactory _schedulerFactory;

        public FieldSatelliteLandsatController(
            IFileStorageProvider fileStorageProvider,
            MasofaCropMonitoringDbContext cropMonitoringDbContext,
            ILogger<FieldSatelliteLandsatController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            ISchedulerFactory schedulerFactory) : base(logger, configuration, mediator)
        {
            CropMonitoringDbContext = cropMonitoringDbContext;
            BusinessLogicLogger = businessLogicLogger;
            _schedulerFactory = schedulerFactory;
        }

        /// <summary>
        /// Получить Landsat спутниковые продукты для поля
        /// </summary>
        /// <param name="fieldId">Идентификатор поля</param>
        /// <returns>Список Landsat спутниковых продуктов</returns>
        [HttpGet("products/{fieldId}")]
        public async Task<ActionResult<FieldSatelliteLandsatProductsResponse>> GetFieldSatelliteLandsatProducts(Guid fieldId)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetFieldSatelliteLandsatProducts)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                // Проверяем существование поля
                var field = await CropMonitoringDbContext.Fields
                    .FirstOrDefaultAsync(f => f.Id == fieldId);

                if (field == null)
                {
                    return NotFound($"Field with ID {fieldId} not found");
                }

                var request = new GetFieldSatelliteLandsatProductsRequest
                {
                    FieldId = fieldId
                };

                var response = await Mediator.Send(request);
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, response?.ToString()), requestPath);
                return Ok(response);
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
        /// Получение всех продуктов Landsat для поля из маппинга
        /// </summary>
        /// <param name="fieldId">Идентификатор поля</param>
        /// <returns>Список продуктов Landsat</returns>
        [HttpGet("mapping/{fieldId}")]
        public async Task<ActionResult<GetFieldProductsResponse>> GetFieldLandsatProducts(Guid fieldId)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetFieldLandsatProducts)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var request = new GetFieldProductsRequest
                {
                    FieldId = fieldId,
                    SatelliteType = ProductSourceType.Landsat
                };

                var response = await Mediator.Send(request);
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, response.Products?.ToString()), requestPath);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogErrorAsync(msg, requestPath);
                Logger.LogError(ex, msg, fieldId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Проверить наличие Landsat спутниковых продуктов для поля по датам
        /// </summary>
        /// <param name="fieldId">Идентификатор поля</param>
        /// <param name="startAt">Дата начала поиска</param>
        /// <param name="finishAt">Дата окончания поиска</param>
        /// <returns>Информация о наличии продуктов</returns>
        [HttpPost]
        [Route("[action]/{fieldId}")]
        public async Task<ActionResult> CheckFieldSatelliteLandsatProducts(
            Guid fieldId,
            [FromQuery] DateOnly startAt,
            [FromQuery] DateOnly finishAt)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(CheckFieldSatelliteLandsatProducts)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                
                if (startAt >= finishAt)
                {
                    var errorMsg = LogMessageResource.InvalidDateRange();
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return BadRequest(errorMsg);
                }

                // Проверяем существование поля
                var field = await CropMonitoringDbContext.Fields
                    .FirstOrDefaultAsync(f => f.Id == fieldId);

                if (field == null)
                {
                    return NotFound($"Field with ID {fieldId} not found");
                }

                // Получаем продукты для поля из маппинга
                var request = new GetFieldProductsRequest
                {
                    FieldId = fieldId,
                    SatelliteType = ProductSourceType.Landsat
                };

                var response = await Mediator.Send(request);

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, response.Products.ToString()), requestPath);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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
