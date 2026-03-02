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
    /// Provides API endpoints for managing <see cref="Masofa.Common.Models.CropMonitoring.FieldAgroOperation">field agro-operations</see> within the Crop Monitoring module.
    /// </summary>
    /// <remarks>This controller supports CRUD operations for <see cref="Masofa.Common.Models.CropMonitoring.FieldAgroOperation">field agro-operations</see>, leveraging the base
    /// functionality  provided by <see cref="BaseCrudController{TEntity, TDbContext}"/>. It is secured with JWT-based
    /// authentication  and role-based authorization, requiring users to have one of the following roles: Admin,
    /// SystemAdmin,  ModuleAdmin, or Operator.</remarks>
    [Route("cropMonitoring/[controller]")]
    [ApiExplorerSettings(GroupName = "CropMonitoring")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
    public class FieldAgroOperationController : BaseCrudController<Masofa.Common.Models.CropMonitoring.FieldAgroOperation, Masofa.DataAccess.MasofaCropMonitoringDbContext>
    {
        public FieldAgroOperationController(IFileStorageProvider fileStorageProvider, 
            MasofaCropMonitoringDbContext dbContext, 
            ILogger<FieldAgroOperationController> logger, 
            IConfiguration configuration, 
            IMediator mediator, 
            IBusinessLogicLogger businessLogicLogger, 
            IHttpContextAccessor httpContextAccessor) : base(fileStorageProvider, dbContext, logger, configuration, mediator, businessLogicLogger, httpContextAccessor)
        {

        }
    }
}
