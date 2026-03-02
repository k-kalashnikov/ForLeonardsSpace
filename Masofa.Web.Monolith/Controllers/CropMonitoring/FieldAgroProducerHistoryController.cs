using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.CropMonitoring
{
    /// <summary>
    /// Provides API endpoints for managing the history of <see cref="Masofa.Common.Models.CropMonitoring.FieldAgroProducerHistory">field-agro-producer</see>  relationships  in the Crop Monitoring
    /// module.
    /// </summary>
    /// <remarks>This controller is part of the Crop Monitoring module and is used to perform CRUD operations 
    /// on <see cref="Masofa.Common.Models.CropMonitoring.FieldAgroProducerHistory"/> entities.  It requires
    /// authentication using the JWT Bearer scheme and is restricted to users with  the roles "Admin", "SystemAdmin",
    /// "ModuleAdmin", or "Operator".</remarks>
    [Route("cropMonitoring/[controller]")]
    [ApiExplorerSettings(GroupName = "CropMonitoring")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
    public class FieldAgroProducerHistoryController : BaseCrudController<Masofa.Common.Models.CropMonitoring.FieldAgroProducerHistory, Masofa.DataAccess.MasofaCropMonitoringDbContext>
    {
        public FieldAgroProducerHistoryController(IFileStorageProvider fileStorageProvider, MasofaCropMonitoringDbContext dbContext, ILogger<FieldAgroProducerHistoryController> logger, IConfiguration configuration, IMediator mediator, IBusinessLogicLogger businessLogicLogger, IHttpContextAccessor httpContextAccessor) : base(fileStorageProvider, dbContext, logger, configuration, mediator, businessLogicLogger, httpContextAccessor)
        {

        }
    }
}
