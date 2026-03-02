using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Dictionaries
{
    /// <summary>
    /// Предоставляет API-методы для управления содержаниями заявок
    /// </summary>
    [Route("dictionaries/[controller]")]
    [ApiExplorerSettings(GroupName = "Dictionaries")]
    public class BidContentController : BaseDictionaryController<BidContent, MasofaDictionariesDbContext>
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="BidContentController"/>.
        /// </summary>
        /// <param name="fileStorageProvider">Провайдер для работы с хранилищем файлов</param>
        /// <param name="dbContext">Контекст базы данных</param>

        /// <param name="logger">Логгер</param>
        /// <param name="configuration">Конфигурация приложения</param>
        public BidContentController(
            IFileStorageProvider fileStorageProvider,
            MasofaDictionariesDbContext dbContext,
            ILogger<BidContentController> logger,
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
    }
}
