using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Satellite.Indices
{
    [Route("satellite/[controller]")]
    [ApiExplorerSettings(GroupName = "SatelliteIndices")]
    public class GndviController : BaseIndexesController<GndviPoint, GndviPolygon, GndviSeasonReport, GndviSharedReport>
    {
        public GndviController(ILogger<GndviController> logger, IConfiguration configuration, IMediator mediator, MasofaIndicesDbContext masofaIndicesDbContext, IFileStorageProvider fileStorageProvider, IBusinessLogicLogger businessLogicLogger, IHttpContextAccessor httpContextAccessor) : base(logger, configuration, mediator, masofaIndicesDbContext, fileStorageProvider, businessLogicLogger, httpContextAccessor)
        {
        }
    }
}
