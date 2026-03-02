using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Dictionaries
{
    /// <summary>
    /// Предоставляет API-методы для управления: Реестр земельных ресурсов с указанием их плодородности
    /// </summary>
    [Route("dictionaries/[controller]")]
    [ApiExplorerSettings(GroupName = "Dictionaries")]
    public class LandResourceFertilityController : BaseDictionaryController<LandResourceFertility, MasofaDictionariesDbContext>
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="LandResourceFertilityController"/>.
        /// </summary>
        public LandResourceFertilityController(
            IFileStorageProvider fileStorageProvider,
            MasofaDictionariesDbContext dbContext,
            ILogger<LandResourceFertilityController> logger,
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
