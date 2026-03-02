using Masofa.Common.Resources;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Identity;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Identity.OneId
{
    public class UpsertOneIdUserCommand : IRequest<UpsertOneIdUserCommandResult>
    {
        public required OneIdUser OneIdUserData { get; set; }
    }

    public class UpsertOneIdUserCommandHandler : IRequestHandler<UpsertOneIdUserCommand, UpsertOneIdUserCommandResult>
    {
        private IBusinessLogicLogger BusinessLogicLogger { get; }
        private ILogger Logger { get; }
        private MasofaIdentityDbContext IdentityDbContext { get; }

        public UpsertOneIdUserCommandHandler(
            MasofaIdentityDbContext identityDbContext,
            IBusinessLogicLogger businessLogicLogger,
            ILogger<UpsertOneIdUserCommandHandler> logger)
        {
            IdentityDbContext = identityDbContext;
            BusinessLogicLogger = businessLogicLogger;
            Logger = logger;
        }

        public async Task<UpsertOneIdUserCommandResult> Handle(UpsertOneIdUserCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                var currentOneIdUser = IdentityDbContext.OneIdUsers.FirstOrDefault(u => u.Pinfl == request.OneIdUserData.Pinfl);
                if (currentOneIdUser == null)
                {
                    await IdentityDbContext.OneIdUsers.AddAsync(new OneIdUser()
                    {
                        Id = request.OneIdUserData.Id,
                        Username = request.OneIdUserData.Username,
                        Email = request.OneIdUserData.Email,
                        FirstName = request.OneIdUserData.FirstName,
                        LastName = request.OneIdUserData.LastName,
                        Pinfl = request.OneIdUserData.Pinfl,
                        Position = request.OneIdUserData.Position,
                        PhoneNumber = request.OneIdUserData.PhoneNumber,
                        Subdivision = request.OneIdUserData.Subdivision,
                        Description = request.OneIdUserData.Description,
                        Base = request.OneIdUserData.Base,
                        Organization = request.OneIdUserData.Organization,
                        LastLogin = request.OneIdUserData.LastLogin
                    });
                }
                else
                {
                    currentOneIdUser.Id = request.OneIdUserData.Id;
                    currentOneIdUser.Username = request.OneIdUserData.Username;
                    currentOneIdUser.Email = request.OneIdUserData.Email;
                    currentOneIdUser.FirstName = request.OneIdUserData.FirstName;
                    currentOneIdUser.LastName = request.OneIdUserData.LastName;
                    currentOneIdUser.Pinfl = request.OneIdUserData.Pinfl;
                    currentOneIdUser.Position = request.OneIdUserData.Position;
                    currentOneIdUser.PhoneNumber = request.OneIdUserData.PhoneNumber;
                    currentOneIdUser.Subdivision = request.OneIdUserData.Subdivision;
                    currentOneIdUser.Description = request.OneIdUserData.Description;
                    currentOneIdUser.Base = request.OneIdUserData.Base;
                    currentOneIdUser.Organization = request.OneIdUserData.Organization;
                    currentOneIdUser.LastLogin = request.OneIdUserData.LastLogin;
                }

                await IdentityDbContext.SaveChangesAsync(cancellationToken);

                return new UpsertOneIdUserCommandResult()
                {
                    UserName = currentOneIdUser?.Username ?? string.Empty,
                    Email = currentOneIdUser?.Email ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                throw ex;
            }
        }
    }

    public class UpsertOneIdUserCommandResult
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<IdentityError> Errors { get; set; } = new List<IdentityError>();
    }
}
