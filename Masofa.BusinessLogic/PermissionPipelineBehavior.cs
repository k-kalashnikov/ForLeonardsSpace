using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Exceptions;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace Masofa.BusinessLogic
{
    public class PermissionPipelineBehavior<TRequest, TResponse>
            : IPipelineBehavior<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
    {
        private MasofaIdentityDbContext IdentityDbContext { get; set; }
        private IHttpContextAccessor HttpContextAccessor { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public PermissionPipelineBehavior(MasofaIdentityDbContext identityDbContext, IHttpContextAccessor httpContextAccessor, IBusinessLogicLogger businessLogicLogger)
        {
            IdentityDbContext = identityDbContext;
            HttpContextAccessor = httpContextAccessor;
            BusinessLogicLogger = businessLogicLogger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
            var currentUser = IdentityDbContext.Users.FirstOrDefault(m => m.UserName.Equals(HttpContextAccessor.HttpContext.User.Identity.Name));
            if (currentUser == null)
            {
                var errorMsg = LogMessageResource.UserNotFound(HttpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Undefined");
                await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                throw new LockPermissionException(errorMsg);
            }

            var lockPer = IdentityDbContext.LockPermissions
                .Where(m => m.UserId.Equals(currentUser.Id))
                ?.ToList() ?? new List<Masofa.Common.Models.Identity.LockPermission>();

            var modelType = (typeof(TRequest).IsGenericType && (typeof(TRequest).GetGenericArguments()?.Count() ?? 0) == 2) ? typeof(TRequest).GetGenericArguments()[0] : null;

            if (modelType == null)
            {
                return await next();
            }

            lockPer = lockPer.Where(m => m.EntityTypeName.Equals(modelType.FullName))?.ToList() ?? new List<Masofa.Common.Models.Identity.LockPermission>();

            if (!lockPer.Any())
            {
                return await next();
            }

            if (lockPer.Any(m => m.LockPermissionType == Masofa.Common.Models.Identity.LockPermissionType.Entity))
            {
                var errorMsg = $"Access denied for User:{currentUser.UserName} to Entity: {modelType.Name}";
                await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                throw new LockPermissionException(errorMsg);
            }

            if (lockPer.Any(m => m.LockPermissionType == Masofa.Common.Models.Identity.LockPermissionType.Entity))
            {
                var errorMsg = $"Access denied for User:{currentUser.UserName} to Entity: {modelType.Name}";
                await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                throw new LockPermissionException(errorMsg);
            }

            var perAttributeAction = typeof(TRequest)
                .GetCustomAttribute<RequestPermissionAttribute>()?.ActionType ?? string.Empty;

            if (string.IsNullOrEmpty(perAttributeAction))
            {
                return await next();
            }

            if (lockPer.Any(m => m.EntityAction.Equals(perAttributeAction)))
            {
                var errorMsg = $"Access denied for User:{currentUser.UserName} to Entity: {modelType.Name} at action: {perAttributeAction}";
                await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                throw new LockPermissionException(errorMsg);
            }

            return await next();
        }
    }
}
