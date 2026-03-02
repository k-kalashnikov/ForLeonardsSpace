using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Notifications;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.EmailSender;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Masofa.BusinessLogic.Common.UserTickets
{
    public class CreateMessageCommand : IRequest<Guid>
    {
        public UserTicketMessage userTicketMessages { get; set; }
        public bool SendEmail { get; set; } = false;
        public string? ToEmail { get; set; } = null;
    }

    public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, Guid>
    {
        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
        private MasofaIdentityDbContext MasofaIdentityDbContext { get; set; }
        private ILogger Logger { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private IEmailSender EmailSender { get; set; }
        private IHttpContextAccessor _http { get; set; }
        private IMediator Mediator { get; set; }


        public CreateMessageCommandHandler(MasofaCommonDbContext masofaCommonDbContext, MasofaIdentityDbContext masofaIdentityDbContext, IMediator mediator,IHttpContextAccessor http, IEmailSender emailSender, ILogger<CreateMessageCommandHandler> logger, IBusinessLogicLogger businessLogicLogger)
        {
            MasofaCommonDbContext = masofaCommonDbContext;
            Logger = logger;
            BusinessLogicLogger = businessLogicLogger;
            EmailSender = emailSender;
            MasofaIdentityDbContext = masofaIdentityDbContext;
            Mediator = mediator;
            _http = http;
        }

        public async Task<Guid> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            var ticketMessage = request.userTicketMessages;
            var id = Guid.Empty;

            try
            {
                if (ticketMessage.UserTicketId == Guid.Empty)
                {
                    throw new ValidationException("UserTicketId is required.");
                }
                if (string.IsNullOrWhiteSpace(ticketMessage.Message))
                {
                    throw new ValidationException("Message is required.");
                }

                var userTicket = await MasofaCommonDbContext.UserTickets
                    .FirstOrDefaultAsync(t => t.Id == ticketMessage.UserTicketId, cancellationToken);

                if (userTicket == null)
                {
                    throw new ValidationException($"UserTicket with Id {ticketMessage.UserTicketId} not found.");
                }

                Guid? updaterId = null;
                Guid? emailId = ticketMessage.EmailId;

                var userName = _http.HttpContext?.User?.Identity?.Name;
                if (!string.IsNullOrWhiteSpace(userName))
                {
                    var identityUser = await MasofaIdentityDbContext.Users
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);

                    if (identityUser != null)
                    {
                        updaterId = identityUser.Id;
                        if (!emailId.HasValue)
                        {
                            emailId = identityUser.Id;
                        }
                    }
                }

                if (request.SendEmail)
                {
                    var to = string.IsNullOrWhiteSpace(request.ToEmail) ? userTicket.CreateUserEmail : request.ToEmail;
                    if (string.IsNullOrWhiteSpace(to))
                    {
                        throw new ValidationException("No recipient email. Provide ToEmail or set CreateUserEmail in ticket.");
                    }

                    var subject = $"[Support] Ticket {userTicket.Id}";
                    var ok = await EmailSender.SendEmailAsync([to], subject, ticketMessage.Message, null);
                }

                var attachments = ticketMessage.AttachmentIds ?? new();

                var msg = new UserTicketMessage
                {
                    UserTicketId = userTicket.Id,
                    EmailId = emailId,
                    Message = ticketMessage.Message,
                    Created = DateTime.UtcNow,
                    AttachmentIds = attachments,
                    AttachmentIdsJson = JsonConvert.SerializeObject(attachments)
                };

                var saved = MasofaCommonDbContext.UserTicketMessages.Add(msg);

                if (updaterId.HasValue)
                {
                    userTicket.LastUpdateUser = updaterId.Value;
                }
                else if (emailId.HasValue)
                {
                    userTicket.LastUpdateUser = emailId.Value;
                }

                userTicket.LastUpdateAt = DateTime.UtcNow;

                // If ticket is New and someone other than the creator responds, change status to InProgress
                var currentUserId = updaterId ?? emailId;
                if (userTicket.TicketStatus == UserTicketStatus.New && 
                    currentUserId.HasValue && 
                    currentUserId.Value != userTicket.CreateUser)
                {
                    userTicket.TicketStatus = UserTicketStatus.InProgress;
                }

                await MasofaCommonDbContext.SaveChangesAsync(cancellationToken);

                id = saved.Entity.Id;

                if (attachments.Any())
                {
                    var fileItems = await MasofaCommonDbContext.FileStorageItems
                        .Where(f => attachments.Contains(f.Id))
                        .ToListAsync(cancellationToken);

                    var ownerId = saved.Entity.Id;
                    var ownerType = typeof(UserTicketMessage).FullName ?? nameof(UserTicketMessage);
                    var updater = updaterId ?? emailId ?? Guid.Empty;
                    var now = DateTime.UtcNow;

                    foreach (var file in fileItems)
                    {
                        file.OwnerId = ownerId;
                        file.OwnerTypeFullName = ownerType;
                        file.LastUpdateAt = now;
                        file.LastUpdateUser = updater;
                    }

                    if (fileItems.Count > 0)
                    {
                        await MasofaCommonDbContext.SaveChangesAsync(cancellationToken);
                    }
                }

                return id;
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {requestPath}. {ex.Message}";
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                throw ex;
            }
        }
    }

    public class MessageCreateEvent : INotification
    {
        public UserTicketMessage Model { get; set; }
        public Guid TicketId { get; set; }
        public Guid UserId { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
    }
}
