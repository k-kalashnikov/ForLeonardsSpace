using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.MasofaAnaliticReport;
using Masofa.Common.Models.Reports;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Reports
{
    /// <summary>
    /// Farmer Report Controller
    /// </summary>
    [Route("common/[controller]")]
    [ApiExplorerSettings(GroupName = "Common")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
    public class FarmerReportController : BaseReportCrudController<FarmerReport, FarmerRecomendationReport, MasofaAnaliticReportDbContext>
    {
        /// <summary>
        /// Farmer Report Controller Constructor
        /// </summary>
        /// <param name="fileStorageProvider"></param>
        /// <param name="dbContext"></param>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="mediator"></param>
        /// <param name="businessLogicLogger"></param>
        /// <param name="httpContextAccessor"></param>
        public FarmerReportController(
            IFileStorageProvider fileStorageProvider,
            MasofaAnaliticReportDbContext dbContext,
            ILogger<FarmerReportController> logger,
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
