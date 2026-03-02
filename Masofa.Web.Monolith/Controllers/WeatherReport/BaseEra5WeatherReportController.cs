using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Era;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Era;
using Masofa.Common.Services.FileStorage;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Masofa.Web.Monolith.Controllers.WeatherReport
{
    /// <summary>
    /// Базовый CRUD Контроллер, которые реализует запросы по получению, созданию, обновлению и удалению сущностей наследников <see cref="../Masofa.Common/Models/BaseEntity.cs">Masofa.Common.Models.BaseEntity</see>
    /// </summary>
    /// <typeparam name="TModel">Тип сущности</typeparam>
    /// <typeparam name="TDbContext">Тип DbContext, в котором храниться сущность</typeparam>
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Microsoft.AspNetCore.Mvc.Route("[controller]")]
    public class BaseEra5WeatherReportController<TModel, TDbContext> : BaseController
        where TModel : BaseEra5WeatherReport
        where TDbContext : DbContext
    {
        protected TDbContext DbContext { get; set; }
        protected IFileStorageProvider FileStorageProvider { get; set; }
        protected IBusinessLogicLogger BusinessLogicLogger { get; set; }
        protected IHttpContextAccessor HttpContextAccessor { get; set; }

        public BaseEra5WeatherReportController(
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
        public virtual async Task<ActionResult<List<TModel>>> GetByQuery([FromBody] BaseEra5WeatherReportQuery<TModel> query)
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
                var getRequest = new BaseEra5WeatherReportGetRequest<TModel>()
                {
                    Query = query
                };

                return await Mediator.Send(getRequest);
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
