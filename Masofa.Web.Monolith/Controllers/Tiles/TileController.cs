using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Tiles;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Tiles
{
    /// <summary>
    /// Контроллер для работы с тайлами
    /// </summary>
    [Route("tiles/[controller]")]
    [ApiExplorerSettings(GroupName = "Tiles")]
    public class TileController : BaseCrudController<Tile, MasofaTileDbContext>
    {
        public TileController(
            IFileStorageProvider fileStorageProvider,
            MasofaTileDbContext dbContext,
            ILogger<TileController> logger,
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
