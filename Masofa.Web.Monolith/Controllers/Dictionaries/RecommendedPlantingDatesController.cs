using Masofa.BusinessLogic.Features.RecommendedPlantingDates;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Resources;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Dictionaries
{
    [Route("dictionaries/[controller]")]
    [ApiExplorerSettings(GroupName = "Dictionaries")]
    public class RecommendedPlantingDatesController : BaseDictionaryController<RecommendedPlantingDates, MasofaDictionariesDbContext>
    {
        public RecommendedPlantingDatesController(
            IFileStorageProvider fileStorageProvider,
            MasofaDictionariesDbContext dbContext,
            ILogger<RecommendedPlantingDatesController> logger,
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
        /// Специальный метод создания с ручной сериализацией сезонов
        /// </summary>
        [HttpPost("CreateCustom")]
        public async Task<ActionResult<Guid>> CreateCustom([FromBody] RecommendedPlantingDates model)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(CreateCustom)}";
            if (model == null) return BadRequest("Model is null");

            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var command = new CreateRecommendedPlantingDateCommand { Model = model };
                var result = await Mediator.Send(command);
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.ToString()), requestPath);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                var msg = $"Validation Error: {ex.Message}";
                await BusinessLogicLogger.LogWarningAsync(msg, requestPath);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Специальный метод редактирования с ручной сериализацией сезонов
        /// </summary>
        [HttpPut("EditCustom")]
        public async Task<ActionResult<Guid>> EditCustom([FromBody] RecommendedPlantingDates model)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(EditCustom)}";
            if (model == null || model.Id == Guid.Empty) return BadRequest("Model or ID is invalid");

            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var command = new EditRecommendedPlantingDateCommand { Model = model };
                var result = await Mediator.Send(command);
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.ToString()), requestPath);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                var msg = $"Validation Error: {ex.Message}";
                await BusinessLogicLogger.LogWarningAsync(msg, requestPath);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Специальный метод получения списка с ручной десериализацией сезонов
        /// Обязательная фильтрация по культуре.
        /// </summary>
        [HttpGet("GetAllCustom")]
        public async Task<ActionResult<List<RecommendedPlantingDates>>> GetAllCustom(Guid cropId)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetAllCustom)}";
            if (cropId == Guid.Empty)
            {
                return BadRequest("CropId is required for filtering.");
            }

            try
            {
                var query = new GetRecommendedPlantingDatesCustomQuery(cropId);
                var result = await Mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(500, ex.Message);
            }
        }
    }
}