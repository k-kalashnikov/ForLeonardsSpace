using Masofa.Common.Resources;
using Masofa.BusinessLogic.Common;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Common
{
    /// <summary>
    /// Контроллер для управления данными БД (только чтение)
    /// </summary>
    [Route("common/[controller]")]
    [ApiExplorerSettings(GroupName = "Common")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
    public class AdminDataManagementController : BaseController
    {
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public AdminDataManagementController(
            ILogger<AdminDataManagementController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger)
            : base(logger, configuration, mediator)
        {
            _businessLogicLogger = businessLogicLogger;
        }

        /// <summary>
        /// Получает список всех DbContext'ов
        /// </summary>
        /// <returns>Список информации о DbContext'ах</returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<DbContextInfo>>> GetDbContexts()
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetDbContexts)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var result = await Mediator.Send(new GetDbContextsRequest());
                await _businessLogicLogger.LogInformationAsync($"Finish request in {requestPath} with result: {result.Count} contexts", requestPath);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, LogMessageResource.GenericError(requestPath,ex.Message));
            }
        }

        /// <summary>
        /// Получает список таблиц в DbContext
        /// </summary>
        /// <param name="dbContextFullName">Полное имя типа DbContext</param>
        /// <returns>Список информации о таблицах</returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<TableInfo>>> GetTables([FromQuery] string dbContextFullName)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetTables)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                
                if (string.IsNullOrEmpty(dbContextFullName))
                {
                    return BadRequest("DbContextFullName is required");
                }

                var result = await Mediator.Send(new GetTablesByDbContextRequest
                {
                    DbContextFullName = dbContextFullName
                });
                
                await _businessLogicLogger.LogInformationAsync($"Finish request in {requestPath} with result: {result.Count} tables", requestPath);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                await _businessLogicLogger.LogErrorAsync(LogMessageResource.GenericError(requestPath,ex.Message), requestPath);
                return BadRequest(LogMessageResource.GenericError(requestPath,ex.Message));
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, LogMessageResource.GenericError(requestPath,ex.Message));
            }
        }

        /// <summary>
        /// Получает структуру таблицы
        /// </summary>
        /// <param name="dbContextFullName">Полное имя типа DbContext</param>
        /// <param name="entityTypeName">Имя типа сущности</param>
        /// <returns>Список информации о колонках</returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<ColumnInfo>>> GetTableStructure([FromQuery] string dbContextFullName, [FromQuery] string entityTypeName)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetTableStructure)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                
                if (string.IsNullOrEmpty(dbContextFullName))
                {
                    return BadRequest("DbContextFullName is required");
                }

                if (string.IsNullOrEmpty(entityTypeName))
                {
                    return BadRequest("EntityTypeName is required");
                }

                var result = await Mediator.Send(new GetTableStructureRequest
                {
                    DbContextFullName = dbContextFullName,
                    EntityTypeName = entityTypeName
                });
                
                await _businessLogicLogger.LogInformationAsync($"Finish request in {requestPath} with result: {result.Count} columns", requestPath);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                await _businessLogicLogger.LogErrorAsync(LogMessageResource.GenericError(requestPath,ex.Message), requestPath);
                return BadRequest(LogMessageResource.GenericError(requestPath,ex.Message));
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, LogMessageResource.GenericError(requestPath,ex.Message));
            }
        }

        /// <summary>
        /// Получает данные таблицы с фильтрацией и пагинацией
        /// </summary>
        /// <param name="request">Параметры запроса</param>
        /// <returns>Данные таблицы</returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<TableDataResult>> GetTableData([FromBody] GetTableDataRequest request)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetTableData)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync($"Start request in {requestPath} for context: {request.DbContextFullName}, entity: {request.EntityTypeName}", requestPath);
                
                if (string.IsNullOrEmpty(request.DbContextFullName))
                {
                    return BadRequest("DbContextFullName is required");
                }

                if (string.IsNullOrEmpty(request.EntityTypeName))
                {
                    return BadRequest("EntityTypeName is required");
                }

                if (!ModelState.IsValid)
                {
                    await _businessLogicLogger.LogErrorAsync(LogMessageResource.TableDataModelValidationFailed(requestPath), requestPath);
                    return BadRequest(ModelState);
                }

                var result = await Mediator.Send(request);
                
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.TotalCount.ToString()), requestPath);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                await _businessLogicLogger.LogErrorAsync(LogMessageResource.GenericError(requestPath,ex.Message), requestPath);
                return BadRequest(LogMessageResource.GenericError(requestPath,ex.Message));
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, LogMessageResource.GenericError(requestPath,ex.Message));
            }
        }

        /// <summary>
        /// Получает упрощенную структуру таблицы (для визуализации)
        /// </summary>
        /// <param name="dbContextFullName">Полное имя типа DbContext</param>
        /// <param name="entityTypeName">Имя типа сущности</param>
        /// <returns>Упрощенная структура таблицы</returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<SimpleTableStructure>> GetSimpleTableStructure([FromQuery] string dbContextFullName, [FromQuery] string entityTypeName)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetSimpleTableStructure)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                
                if (string.IsNullOrEmpty(dbContextFullName))
                {
                    return BadRequest("DbContextFullName is required");
                }

                if (string.IsNullOrEmpty(entityTypeName))
                {
                    return BadRequest("EntityTypeName is required");
                }

                var result = await Mediator.Send(new GetSimpleTableStructureRequest
                {
                    DbContextFullName = dbContextFullName,
                    EntityTypeName = entityTypeName
                });
                
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.TableName), requestPath);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                await _businessLogicLogger.LogErrorAsync(LogMessageResource.GenericError(requestPath,ex.Message), requestPath);
                return BadRequest(LogMessageResource.GenericError(requestPath,ex.Message));
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, LogMessageResource.GenericError(requestPath,ex.Message));
            }
        }

        /// <summary>
        /// Получает все связи между таблицами в DbContext (для визуализации как в Navicat)
        /// </summary>
        /// <param name="dbContextFullName">Полное имя типа DbContext</param>
        /// <returns>Информация о связях между таблицами</returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<TableRelationshipsInfo>> GetTableRelationships([FromQuery] string dbContextFullName)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetTableRelationships)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                
                if (string.IsNullOrEmpty(dbContextFullName))
                {
                    return BadRequest("DbContextFullName is required");
                }

                var result = await Mediator.Send(new GetTableRelationshipsRequest
                {
                    DbContextFullName = dbContextFullName
                });
                
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath,result.ToString()), requestPath);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                await _businessLogicLogger.LogErrorAsync(LogMessageResource.GenericError(requestPath,ex.Message), requestPath);
                return BadRequest(LogMessageResource.GenericError(requestPath,ex.Message));
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, LogMessageResource.GenericError(requestPath,ex.Message));
            }
        }
    }
}

