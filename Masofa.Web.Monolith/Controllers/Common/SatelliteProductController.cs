using Masofa.Common.Resources;
using Masofa.BusinessLogic.Common.Satellite;
using Masofa.BusinessLogic.Satellite;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Common
{
    /// <summary>
    /// Provides API endpoints for managing satellite products, including retrieving, filtering, and modifying satellite
    /// product data.
    /// </summary>
    /// <remarks>This controller supports operations such as retrieving satellite product indices, applying
    /// custom filters,  adding or removing tags, and obtaining summary information about satellite products.  It is
    /// secured with role-based authorization and requires the caller to have one of the specified roles.</remarks>
    [Route("common/[controller]")]
    [ApiExplorerSettings(GroupName = "Common")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Application, Subscriber, Operator")]
    public class SatelliteProductController : BaseCrudController<SatelliteProduct, MasofaCommonDbContext>
    {
        public SatelliteProductController(
            IFileStorageProvider fileStorageProvider,
            MasofaCommonDbContext dbContext,
            ILogger<SatelliteProductController> logger,
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

        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public async Task<ActionResult<IndicexBySatelliteProductViewModel>> GetIndices(Guid Id)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetIndices)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Id.ToString()), requestPath);
                    return BadRequest(ModelState);
                }

                return await Mediator.Send(new GetIndicexBySatelliteProductRequest()
                {
                    SatelliteProductId = Id
                });
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
        /// Получает спутниковые продукты с расширенной фильтрацией по региону, типу спутника и дате
        /// </summary>
        /// <param name="request">Параметры запроса</param>
        /// <returns>Список продуктов с метаданными и общим количеством</returns>
        /// <response code="200">Успешно получен список продуктов</response>
        /// <response code="400">Некорректные параметры запроса</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<CustomGetSatelliteProductsResponse>> CustomGet([FromBody] CustomGetSatelliteProductsRequest request)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(CustomGet)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, request.SatelliteType.Value.ToString()), requestPath);
                    return BadRequest(ModelState);
                }

                var response = await Mediator.Send(request);
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
        /// Получает список спутниковых продуктов вместе с информацией об их индексах
        /// </summary>
        /// <param name="request">Параметры фильтрации (Date, Mission)</param>
        /// <returns>Список продуктов с индексами</returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<GetSatelliteProductsWithIndicesResponse>> GetWithIndices([FromBody] GetSatelliteProductsWithIndicesRequest request)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetWithIndices)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, "Invalid Model"), requestPath);
                    return BadRequest(ModelState);
                }

                var response = await Mediator.Send(request);
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
        /// Сводная информация по спутниковым продуктам
        /// </summary>
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult> Summary()
        {
            var response = await Mediator.Send(new GetSatelliteSummaryRequest());
            return Ok(response);
        }

        /// <summary>
        /// Добавляет тег к спутниковому продукту
        /// </summary>
        /// <param name="productId">ID спутникового продукта</param>
        /// <param name="tagName">Название тега</param>
        /// <returns>Результат операции</returns>
        /// <response code="200">Тег успешно добавлен</response>
        /// <response code="400">Некорректные параметры запроса</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("[action]/{productId}")]
        public async Task<ActionResult<bool>> AddTag(Guid productId, [FromBody] string tagName)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(AddTag)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (string.IsNullOrWhiteSpace(tagName))
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, tagName), requestPath);
                    return BadRequest("Tag name cannot be empty");
                }

                var result = await Mediator.Send(new AddTagToSatelliteProductRequest
                {
                    SatelliteProductId = productId,
                    TagName = tagName.Trim()
                });

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

        /// <summary>
        /// Удаляет тег у спутникового продукта
        /// </summary>
        /// <param name="productId">ID спутникового продукта</param>
        /// <param name="tagId">ID тега</param>
        /// <returns>Результат операции</returns>
        /// <response code="200">Тег успешно удален</response>
        /// <response code="400">Некорректные параметры запроса</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpDelete]
        [Route("[action]/{productId}/{tagId}")]
        public async Task<ActionResult<bool>> RemoveTag(Guid productId, Guid tagId)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(RemoveTag)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var result = await Mediator.Send(new RemoveTagFromSatelliteProductRequest
                {
                    SatelliteProductId = productId,
                    TagId = tagId
                });

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
