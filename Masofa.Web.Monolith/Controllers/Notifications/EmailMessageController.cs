using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Notifications;
using Masofa.Common.Services.EmailSender;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quartz.Util;

namespace Masofa.Web.Monolith.Controllers.Notifications
{
    /// <summary>
    /// Контроллер для работы с письмами
    /// </summary>
    [Route("notifications/[controller]")]
    [ApiExplorerSettings(GroupName = "Notifications")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
    public class EmailMessageController : BaseCrudController<EmailMessage, MasofaCommonDbContext>
    {
        private IEmailSender EmailSender {  get; set; }
        public EmailMessageController(
            IFileStorageProvider fileStorageProvider,
            MasofaCommonDbContext dbContext,
            ILogger<EmailMessageController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            IHttpContextAccessor httpContextAccessor,
            IEmailSender emailSender) : base(
                fileStorageProvider,
                dbContext,
                logger,
                configuration,
                mediator,
                businessLogicLogger,
                httpContextAccessor)
        {
            EmailSender = emailSender;
        }

        [HttpPost]
        public async Task SendEmailMethod()
        {
            var to = new List<string>();
            var email = "emin.mov24@gmail.com";
            to.Add(email);
            var subject = "test google smtp";
            var body = "test google smtp";

            var success = await EmailSender.SendEmailAsync(to, subject, body, null);
        }
    }
}
