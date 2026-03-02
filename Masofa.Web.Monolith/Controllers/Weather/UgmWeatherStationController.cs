using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Ugm;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Weather
{
    /// <summary>
    /// Предоставляет API-методы для управления погодными станциями УГМ
    /// </summary>
    [Route("weather/[controller]")]
    [ApiExplorerSettings(GroupName = "Weather")]
    public class UgmWeatherStationController : BaseCrudController<UgmWeatherStation, MasofaUgmDbContext>
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UgmWeatherStationController"/>.
        /// </summary>
        /// <param name="fileStorageProvider">Провайдер для работы с хранилищем файлов</param>
        /// <param name="dbContext">Контекст базы данных</param>
        /// <param name="logger">Логгер</param>
        /// <param name="configuration">Конфигурация приложения</param>
        /// <param name="mediator">Медиатор</param>
        /// <param name="businessLogicLogger">Бизнес логгер</param>
        /// <param name="httpContextAccessor">Доступ к контексту</param>
        public UgmWeatherStationController(
            IFileStorageProvider fileStorageProvider,
            MasofaUgmDbContext dbContext,
            ILogger<UgmWeatherStationController> logger,
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
