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
    /// Provides API endpoints for managing field insurance case records (страховые случаи).
    /// </summary>
    /// <remarks>This controller supports CRUD operations for field insurance cases, leveraging the
    /// base functionality provided by <see cref="BaseCrudController{TEntity, TDbContext}"/>. It is secured with
    /// JWT-based authentication and role-based authorization, requiring users to have one of the following roles:
    /// Admin, SystemAdmin, ModuleAdmin, or Operator.</remarks>
    [Route("cropMonitoring/[controller]")]
    [ApiExplorerSettings(GroupName = "CropMonitoring")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
    public class FieldInsuranceCaseController : BaseCrudController<Masofa.Common.Models.CropMonitoring.FieldInsuranceCase, Masofa.DataAccess.MasofaCropMonitoringDbContext>
    {
        public FieldInsuranceCaseController(IFileStorageProvider fileStorageProvider, MasofaCropMonitoringDbContext dbContext, ILogger<FieldInsuranceCaseController> logger, IConfiguration configuration, IMediator mediator, IBusinessLogicLogger businessLogicLogger, IHttpContextAccessor httpContextAccessor) : base(fileStorageProvider, dbContext, logger, configuration, mediator, businessLogicLogger, httpContextAccessor)
        {

        }
    }
}

