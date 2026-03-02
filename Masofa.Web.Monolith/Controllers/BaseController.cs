using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers
{
    [ApiController]
    public class BaseController : Controller
    {
        public ILogger Logger { get; set; }
        protected IConfiguration Configuration { get; set; }
        protected IMediator Mediator { get; set; }

        public BaseController(ILogger logger, IConfiguration configuration, IMediator mediator)
        {
            Logger = logger;
            Configuration = configuration;
            Mediator = mediator;
        }
    }
}
