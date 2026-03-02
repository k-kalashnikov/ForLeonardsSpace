using Masofa.Common.Resources;
using Masofa.BusinessLogic;
using Masofa.BusinessLogic.Extentions;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Helper;
using Masofa.Common.Models;
using Masofa.Common.Services.FileStorage;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace Masofa.Web.Monolith.Controllers
{
    /// <summary>
    /// Базовый CRUD Контроллер, которые реализует запросы по получению, созданию, обновлению и удалению сущностей наследников <see cref="../Masofa.Common/Models/BaseEntity.cs">Masofa.Common.Models.BaseEntity</see>
    /// </summary>
    /// <typeparam name="TModel">Тип сущности</typeparam>
    /// <typeparam name="TDbContext">Тип DbContext, в котором храниться сущность</typeparam>
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Microsoft.AspNetCore.Mvc.Route("[controller]")]
    public abstract class BaseCrudController<TModel, TDbContext> : BaseController
        where TModel : BaseEntity
        where TDbContext : DbContext
    {
        protected TDbContext DbContext { get; set; }
        protected IFileStorageProvider FileStorageProvider { get; set; }
        protected IBusinessLogicLogger BusinessLogicLogger { get; set; }
        protected IHttpContextAccessor HttpContextAccessor { get; set; }

        public BaseCrudController(
            IFileStorageProvider fileStorageProvider,
            TDbContext dbContext,
            ILogger logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            IHttpContextAccessor httpContextAccessor) : base(logger, configuration, mediator)
        {
            DbContext = dbContext;
            FileStorageProvider = fileStorageProvider;
            BusinessLogicLogger = businessLogicLogger;
            HttpContextAccessor = httpContextAccessor;
        }


        /// <summary>
        /// Получает список сущностей по заданному запросу с фильтрацией, сортировкой и пагинацией
        /// </summary>
        /// <param name="query">Объект запроса с параметрами фильтрации, сортировки и пагинации</param>
        /// <returns>Список найденных сущностей</returns>
        /// <response code="200">Успешно получен список сущностей</response>
        /// <response code="400">Некорректные параметры запроса</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public virtual async Task<ActionResult<List<TModel>>> GetByQuery([FromBody] BaseGetQuery<TModel> query)
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
                var getRequest = new BaseGetRequest<TModel, TDbContext>()
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

        /// <summary>
        /// Получает список публичных сущностей по заданному запросу с фильтрацией, сортировкой и пагинацией
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public virtual async Task<ActionResult<List<TModel>>> GetPublicQuery([FromBody] BaseGetQuery<TModel> query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetPublicQuery)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, query.ToString()), requestPath);
                    return BadRequest(ModelState);
                }

                var filters = (query.Filters ?? new List<FieldFilter>()).ToList();
                filters.Add(new FieldFilter
                {
                    FilterField = typeof(BaseEntity).GetProperty("IsPublic").Name,
                    FilterValue = true,
                    FilterOperator = FilterOperator.Equals
                });

                var modifiedQuery = new BaseGetQuery<TModel>
                {
                    Filters = filters,
                    Take = query.Take,
                    Offset = query.Offset,
                    SortBy = query.SortBy,
                    Sort = query.Sort
                };

                var getRequest = new BaseGetRequest<TModel, TDbContext>()
                {
                    Query = modifiedQuery
                };

                var published = await Mediator.Send(getRequest);

                return published;
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {requestPath}";
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Получает сущность по уникальному идентификатору
        /// </summary>
        /// <param name="id">Уникальный идентификатор сущности</param>
        /// <returns>Найденная сущность</returns>
        /// <response code="200">Сущность успешно найдена</response>
        /// <response code="404">Сущность с указанным ID не найдена</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("[action]/{id}")]
        public virtual async Task<ActionResult<TModel>> GetById(Guid id)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetById)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var getRequest = new BaseGetByIdRequest<TModel, TDbContext>()
                {
                    Id = id
                };
                var result = await Mediator.Send(getRequest);
                if (result == null)
                {
                    var errorMsg = LogMessageResource.EntityNotFound(typeof(TModel),id.ToString());
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(result)), requestPath);
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
        /// Создает новую сущность в системе
        /// </summary>
        /// <param name="model">Данные для создания новой сущности</param>
        /// <returns>Уникальный идентификатор созданной сущности</returns>
        /// <response code="200">Сущность успешно создана</response>
        /// <response code="400">Некорректные данные модели</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="403">Недостаточно прав для создания</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ActionResult<Guid>> Create([FromBody] TModel model)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Create)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(model)), requestPath);
                    return BadRequest(ModelState);
                }
                var createRequest = new BaseCreateCommand<TModel, TDbContext>()
                {
                    Model = model,
                    Author = User.Identity.Name
                };

                var result = await Mediator.Send(createRequest);
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.ToString()), requestPath);
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
        /// Обновляет существующую сущность в системе
        /// </summary>
        /// <param name="model">Данные для обновления сущности (должен содержать ID)</param>
        /// <returns>Обновленная сущность</returns>
        /// <response code="200">Сущность успешно обновлена</response>
        /// <response code="400">Некорректные данные модели</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="403">Недостаточно прав для обновления</response>
        /// <response code="404">Сущность с указанным ID не найдена</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPut]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ActionResult<TModel>> Update([FromBody] TModel model)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Update)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(model)), requestPath);
                    return BadRequest(ModelState);
                }
                var updateRequest = new BaseUpdateCommand<TModel, TDbContext>()
                {
                    Model = model,
                    Author = User.Identity.Name
                };
                var result = await Mediator.Send(updateRequest);
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(result)), requestPath);
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
        /// Удаляет сущность из системы по уникальному идентификатору
        /// </summary>
        /// <param name="id">Уникальный идентификатор сущности для удаления</param>
        /// <returns>Результат операции удаления (true - успешно, false - не удалось)</returns>
        /// <response code="200">Сущность успешно удалена</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="403">Недостаточно прав для удаления</response>
        /// <response code="404">Сущность с указанным ID не найдена</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpDelete]
        [Microsoft.AspNetCore.Mvc.Route("[action]/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ActionResult<bool>> Delete(Guid id)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Delete)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var deleteRequest = new BaseDeleteCommand<TModel, TDbContext>()
                {
                    Id = id,
                    Author = User.Identity.Name
                };
                var result = await Mediator.Send(deleteRequest);
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(result)), requestPath);
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
        /// Retrieves the total count of items that match the specified query criteria.
        /// </summary>
        /// <remarks>This method processes the query using the mediator pattern to retrieve the total
        /// count of matching items. Ensure that the query object is properly constructed and that the model state is
        /// valid before calling this method.</remarks>
        /// <param name="query">The query object containing the criteria for filtering the items. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the total count of items
        /// matching the query criteria. Returns a <see cref="BadRequestObjectResult"/> if the model state is invalid,
        /// or a <see cref="StatusCodeResult"/> with status 500 if an internal server error occurs.</returns>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public virtual async Task<ActionResult<int>> GetTotalCount([FromBody] BaseGetQuery<TModel> query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetTotalCount)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(query)), requestPath);
                    return BadRequest(ModelState);
                }
                var getRequest = new BaseGetRequest<TModel, TDbContext>()
                {
                    Query = query
                };

                return (await Mediator.Send(getRequest)).Count();
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
        /// Импортирует сущности из CSV файла
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public virtual async Task<ActionResult<bool>> ImportFromCSV(IFormFile formFile)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ImportFromCSV)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var importCommand = new BaseImportFromCSVCommand<TModel, TDbContext>()
                {
                    FormFile = formFile,
                    Author = User.Identity.Name
                };
                await Mediator.Send(importCommand);
                await BusinessLogicLogger.LogInformationAsync($"Finish request in {requestPath} with result: {true}", requestPath);
                return true;
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
        /// Экспортирует сущности в CSV файл
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public virtual async Task<ActionResult<byte[]>> ExportFromCSV([FromBody] BaseGetQuery<TModel> query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ImportFromCSV)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(query)), requestPath);
                    return BadRequest(ModelState);
                }
                var getRequest = new BaseGetRequest<TModel, TDbContext>()
                {
                    Query = query
                };
                var result = await Mediator.Send(getRequest);
                return File(BaseEntityExportHelper<TModel>.ToCSV(result), "text/csv");
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
        /// Получает историю изменений сущности по её идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public async Task<IActionResult> GetHistory(Guid id, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetHistory)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var historyEntityType = typeof(BaseHistoryEntity<>).Assembly.GetTypes()
                .FirstOrDefault(t => t.IsSubclassOf(typeof(BaseHistoryEntity<TModel>)));

                if (historyEntityType == null)
                {
                    var errorMsg = LogMessageResource.HistoryNotFound(typeof(TModel).Name);
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }

                var historyContextType = historyEntityType.GetDbContextTypeForEntity();
                if (historyContextType == null)
                {
                    var errorMsg = $"HistoryDbContextType for entity with type {typeof(TModel).Name} not found";
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }

                await using var scope = HttpContext.RequestServices.CreateAsyncScope();
                if (scope.ServiceProvider.GetService(historyContextType) is not DbContext historyContext)
                {
                    var errorMsg = $"HistoryDbContext for entity with type {typeof(TModel).Name} not found";
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }

                var setMethod = typeof(DbContext).GetMethod("Set", Type.EmptyTypes)!
                    .MakeGenericMethod(historyEntityType);

                if (setMethod.Invoke(historyContext, null) is not IQueryable dbSet)
                {
                    var errorMessage = $"Error gettting DbSet for {nameof(historyContext)}";
                    var msg = LogMessageResource.GenericError(requestPath, errorMessage);
                    await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                    Logger.LogCritical(msg);
                    return StatusCode(StatusCodes.Status500InternalServerError, msg);
                }

                var param = Expression.Parameter(historyEntityType, "h");
                var ownerProp = Expression.Property(param, "OwnerId");
                var equal = Expression.Equal(ownerProp, Expression.Constant(id));

                var predicate = Expression.Lambda(equal, param);

                var whereMethod = typeof(Queryable).GetMethods()
                    .First(m => m.Name == "Where" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(historyEntityType);

                var filteredQuery = (IQueryable)whereMethod.Invoke(null, [dbSet, predicate]);

                var createdProp = Expression.Property(param, "CreateAt");
                var orderLambda = Expression.Lambda(createdProp, param);

                var orderByMethod = typeof(Queryable).GetMethods()
                    .First(m => m.Name == "OrderByDescending" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(historyEntityType, typeof(DateTime));

                var orderedQuery = (IQueryable)orderByMethod.Invoke(null, [filteredQuery, orderLambda]);

                var listMethod = typeof(Enumerable).GetMethod("ToList")!
                    .MakeGenericMethod(historyEntityType);

                var result = listMethod.Invoke(null, [orderedQuery]) as IEnumerable;

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

        //[HttpGet]
        //[Route("[action]")]
        //public virtual ActionResult<int> GetGridData()
        //{
        //    try
        //    {
        //        return DbContext.Set<TModel>()
        //            .Count();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogCritical(ex, $"Something wrong in {GetType().FullName}=>{nameof(GetGridData)}");
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        //[HttpPost]
        //[Route("[action]")]
        //public virtual ActionResult AttachFile(AttachFileViewModel viewModel)
        //{
        //    try
        //    {

        //    }catch (Exception ex)
        //    {

        //    }
        //}

        ///// <summary>
        ///// Return all element from DataBase
        ///// </summary>
        //[HttpGet]
        //[Route("[action]")]
        //public virtual ActionResult<IEnumerable<TModel>> GetAll()
        //{
        //    try
        //    {
        //        var result = DbContext
        //            .Set<TModel>()
        //            .Where(m => m.Status == StatusType.Active)
        //            ?.ToList() ?? new List<TModel>();
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogCritical(ex, $"Something wrong in {GetType().FullName}=>{nameof(GetAll)}");
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        /// <summary>
        /// Экспортв в Excel файл сущностей по заданному запросу с фильтрацией, сортировкой и пагинацией
        /// </summary>
        /// <param name="query">Объект запроса с параметрами фильтрации, сортировки и пагинации</param>
        /// <returns>Excel файл</returns>
        /// <response code="200">Успешно получен файл Excel</response>
        /// <response code="400">Некорректные параметры запроса</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public virtual async Task<IActionResult> ExportToExcel([FromBody] BaseGetQuery<TModel> query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ExportToExcel)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, query.ToString()), requestPath);
                    return BadRequest(ModelState);
                }
                var getRequest = new BaseGetRequest<TModel, TDbContext>()
                {
                    Query = query
                };
                var models = await Mediator.Send(getRequest);

                var fileName = $"{typeof(TModel).Name}_{DateTime.Now:yyyy-MM-dd_HH-mm}.xlsx";

                var excelBytes = await Mediator.Send(new BaseExportToExcelCommand<TModel, TDbContext>()
                {
                    Models = models
                });

                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Импорт из Excel файла сущностей
        /// </summary>
        /// <returns>Флаг успешности импорта сущностей</returns>
        /// <response code="200">Успешно импортированы сущности</response>
        /// <response code="400">Некорректные параметры запроса</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public virtual async Task<ActionResult<bool>> ImportFromExcel(IFormFile formFile)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ImportFromExcel)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                //var importCommand = new BaseImportFromCSVCommand<TModel, TDbContext>()
                var importCommand = new BaseImportFromExcelCommand<TModel, TDbContext>()
                {
                    FormFile = formFile,
                    Author = User.Identity?.Name ?? string.Empty
                };
                await Mediator.Send(importCommand);
                await BusinessLogicLogger.LogInformationAsync($"Finish request in {requestPath} with result: {true}", requestPath);
                return true;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public virtual async Task<ActionResult<byte[]>> ImportCsvTemplate()
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ImportCsvTemplate)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var importCommand = new BaseImportCsvTemplateCommand<TModel, TDbContext>();
                var result = await Mediator.Send(importCommand);
                var fileName = $"{typeof(TModel).Name}_Import_Template.csv";
                return File(result, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public virtual async Task<ActionResult<byte[]>> ImportExcelTemplate()
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ImportExcelTemplate)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var importCommand = new BaseImportExcelTemplateCommand<TModel, TDbContext>();
                var result = await Mediator.Send(importCommand);
                var fileName = $"{typeof(TModel).Name}_Import_Template.xlsx";
                return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
