using Masofa.BusinessLogic;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Helper;
using Masofa.Common.Models;
using Masofa.Common.Services.FileStorage;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Masofa.Web.Monolith.Controllers.Dictionaries
{
    [ApiController]
    public class BaseDictionaryController<TModel, TDbContext> : BaseCrudController<TModel, TDbContext>
        where TModel : BaseEntity
        where TDbContext : DbContext
    {
        public BaseDictionaryController(IFileStorageProvider fileStorageProvider, TDbContext dbContext, ILogger logger, IConfiguration configuration, IMediator mediator, IBusinessLogicLogger businessLogicLogger, IHttpContextAccessor httpContextAccessor) : base(fileStorageProvider, dbContext, logger, configuration, mediator, businessLogicLogger, httpContextAccessor)
        {
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
        public override async Task<ActionResult<List<TModel>>> GetByQuery([FromBody] BaseGetQuery<TModel> query)
        {
            return await base.GetByQuery(query);
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
        public override async Task<ActionResult<TModel>> GetById(Guid id)
        {
            return await base.GetById(id);
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
        public override async Task<ActionResult<Guid>> Create([FromBody] TModel model)
        {
            return await base.Create(model);
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
        public override async Task<ActionResult<TModel>> Update([FromBody] TModel model)
        {
            return await base.Update(model);
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
        public override async Task<ActionResult<bool>> Delete(Guid id)
        {
            return await base.Delete(id);
        }

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public override async Task<ActionResult<int>> GetTotalCount([FromBody] BaseGetQuery<TModel> query)
        {
            return await base.GetTotalCount(query);
        }

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
        public override async Task<ActionResult<bool>> ImportFromCSV(IFormFile formFile)
        {
            return await base.ImportFromCSV(formFile);
        }

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
        public override async Task<ActionResult<byte[]>> ExportFromCSV([FromBody] BaseGetQuery<TModel> query)
        {
            return await base.ExportFromCSV(query);
        }
    }
}
