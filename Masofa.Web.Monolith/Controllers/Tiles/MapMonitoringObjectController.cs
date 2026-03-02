using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Gis;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Tiles
{
    /// <summary>
    /// Контроллер для работы с файлами мониторинга карт
    /// </summary>
    [Route("tiles/[controller]")]
    [ApiExplorerSettings(GroupName = "Tiles")]
    public class MapMonitoringObjectController : BaseCrudController<MapMonitoringObject, MasofaTileDbContext>
    {
        public MapMonitoringObjectController(
            IFileStorageProvider fileStorageProvider,
            MasofaTileDbContext dbContext,
            ILogger<MapMonitoringObjectController> logger,
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
