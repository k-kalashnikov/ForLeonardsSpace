using Masofa.BusinessLogic.Common.SystemMetadata;
using Masofa.Common.Models.SystemMetadata;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace Masofa.Web.Monolith.Controllers.Common
{
    /// <summary>
    /// Контроллер для получения метаданных системы
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("common/[controller]")]
    [ApiExplorerSettings(GroupName = "Common")]
    public class SystemMetadataController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SystemMetadataController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получить все сущности и действия системы
        /// </summary>
        [HttpGet("entities-and-actions")]
        public async Task<ActionResult<SystemMetadataDto>> GetEntitiesAndActions()
        {
            var request = new GetSystemMetadataRequest();
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}