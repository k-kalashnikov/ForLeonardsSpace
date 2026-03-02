using Masofa.Common.Resources;
using Masofa.BusinessLogic;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Identity;
using Masofa.Common.Models.Notifications;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Masofa.BusinessLogic.Common.UserTickets
{
    public class UserTicketCreateCommand : BaseCreateCommand<UserTicket, MasofaCommonDbContext>
    {
        /// <summary>
        /// Initial message text from the user (optional)
        /// If provided, this will be used as the initial message content
        /// </summary>
        public string? InitialMessage { get; set; }

        /// <summary>
        /// EmailId of the user creating the ticket
        /// </summary>
        public Guid? EmailId { get; set; }
    }

    /// <summary>
    /// Custom handler for UserTicket creation that automatically creates an initial message
    /// </summary>
    public class UserTicketCreateCommandHandler : IRequestHandler<UserTicketCreateCommand, Guid>
    {
        private MasofaCommonDbContext CommonDbContext { get; set; }
        private MasofaIdentityDbContext IdentityDbContext { get; set; }
        private ILogger Logger { get; set; }
        private IMediator Mediator { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private IHttpContextAccessor HttpContextAccessor { get; set; }

        public UserTicketCreateCommandHandler(
            MasofaCommonDbContext commonDbContext,
            MasofaIdentityDbContext identityDbContext,
            ILogger<UserTicketCreateCommandHandler> logger,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            IHttpContextAccessor httpContextAccessor)
        {
            CommonDbContext = commonDbContext;
            IdentityDbContext = identityDbContext;
            Logger = logger;
            Mediator = mediator;
            BusinessLogicLogger = businessLogicLogger;
            HttpContextAccessor = httpContextAccessor;
        }

        public async Task<Guid> Handle(UserTicketCreateCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                
                var model = request.Model;
                var authorUserName = request.Author;
                var lastUpdateUser = await IdentityDbContext.Set<User>().FirstAsync(m => m.UserName.ToLower().Equals(authorUserName.ToLower()), cancellationToken);

                // Set base entity properties
                model.CreateAt = DateTime.UtcNow;
                model.CreateUser = lastUpdateUser.Id;
                model.LastUpdateUser = lastUpdateUser.Id;
                model.LastUpdateAt = DateTime.UtcNow;
                model.TicketStatus = UserTicketStatus.New;
                model.Status = StatusType.Active;

                // Add the ticket to the database
                var ticketResult = await CommonDbContext
                    .Set<UserTicket>()
                    .AddAsync(model, cancellationToken);

                await CommonDbContext.SaveChangesAsync(cancellationToken);

                var createdTicket = ticketResult.Entity;
                var ticketId = createdTicket.Id;

                // Automatically create an initial message from the user
                var initialMessageText = !string.IsNullOrWhiteSpace(request.InitialMessage) 
                    ? request.InitialMessage 
                    : $"Created a ticket for module: {model.ModuleName}, Exception Type: {model.ExceptionType}";

                var userProfile = await IdentityDbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserName == authorUserName, cancellationToken);

                var emailId = request.EmailId ?? userProfile?.Id;

                var initialMessage = new UserTicketMessage
                {
                    UserTicketId = ticketId,
                    EmailId = emailId,
                    Message = initialMessageText,
                    Created = DateTime.UtcNow,
                    AttachmentIds = new List<Guid>(),
                    AttachmentIdsJson = JsonConvert.SerializeObject(new List<Guid>())
                };

                await CommonDbContext.UserTicketMessages.AddAsync(initialMessage, cancellationToken);
                await CommonDbContext.SaveChangesAsync(cancellationToken);

                // Publish create event
                await Mediator.Publish(new BaseCreateEvent<UserTicket, MasofaCommonDbContext>()
                {
                    Model = createdTicket,
                    DateTime = DateTime.UtcNow,
                    UserId = lastUpdateUser.Id
                }, cancellationToken);

                await BusinessLogicLogger.LogInformationAsync($"Ticket created with ID: {ticketId} and initial message", requestPath);

                return ticketId;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                throw;
            }
        }
    }
}

