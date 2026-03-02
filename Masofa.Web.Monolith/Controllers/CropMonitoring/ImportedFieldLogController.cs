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
    /// Контроллер для работы с логами загружаемых полей
    /// </summary>
    [Route("cropMonitoring/[controller]")]
    [ApiExplorerSettings(GroupName = "CropMonitoring")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator, Foreman, FieldWorker")]
    public class ImportedFieldLogController : BaseCrudController<Masofa.Common.Models.CropMonitoring.ImportedFieldLog, MasofaCropMonitoringDbContext>
    {
        /// <summary>
        /// Конструктор контроллера для работы с логами загружаемых полей
        /// </summary>
        public ImportedFieldLogController(
            IFileStorageProvider fileStorageProvider,
            MasofaCropMonitoringDbContext dbContext,
            ILogger<ImportedFieldLogController> logger,
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
        { }
    }
}
