using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Tiles;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Tiles
{
    [Route("tiles/[controller]")]
    [ApiExplorerSettings(GroupName = "Tiles")]
    public class TileLayerController : BaseCrudController<TileLayer, MasofaTileDbContext>
    {
        public TileLayerController(IFileStorageProvider fileStorageProvider, MasofaTileDbContext dbContext, ILogger<TileLayerController> logger, IConfiguration configuration, IMediator mediator, IBusinessLogicLogger businessLogicLogger, IHttpContextAccessor httpContextAccessor) : base(fileStorageProvider, dbContext, logger, configuration, mediator, businessLogicLogger, httpContextAccessor)
        {
        }
    }
}
    