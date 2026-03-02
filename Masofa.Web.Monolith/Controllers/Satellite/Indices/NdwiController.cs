using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Satellite.Indices
{
    [Route("satellite/[controller]")]
    [ApiExplorerSettings(GroupName = "SatelliteIndices")]
    public class NdwiController : BaseIndexesController<NdwiPoint, NdwiPolygon, NdwiSeasonReport, NdwiSharedReport>
    {
        public NdwiController(
            ILogger<NdwiController> logger,
            IConfiguration configuration,
            IMediator mediator,
            MasofaIndicesDbContext masofaIndicesDbContext,
            IFileStorageProvider fileStorageProvider,
            IBusinessLogicLogger businessLogicLogger,
            IHttpContextAccessor httpContextAccessor) : base(
                logger,
                configuration,
                mediator,
                masofaIndicesDbContext,
                fileStorageProvider,
                businessLogicLogger,
                httpContextAccessor)
        {
        }
    }
}
