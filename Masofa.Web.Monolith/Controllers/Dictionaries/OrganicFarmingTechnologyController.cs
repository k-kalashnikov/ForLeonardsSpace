using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Dictionaries
{
    /// <summary>
    /// Предоставляет API-методы для управления: Справочник технологий органического земледелия
    /// </summary>
    [Route("dictionaries/[controller]")]
    [ApiExplorerSettings(GroupName = "Dictionaries")]
    public class OrganicFarmingTechnologyController : BaseDictionaryController<OrganicFarmingTechnology, MasofaDictionariesDbContext>
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="OrganicFarmingTechnologyController"/>.
        /// </summary>
        public OrganicFarmingTechnologyController(
            IFileStorageProvider fileStorageProvider,
            MasofaDictionariesDbContext dbContext,
            ILogger<OrganicFarmingTechnologyController> logger,
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
