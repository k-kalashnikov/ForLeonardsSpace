using Masofa.Common.Resources;
using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Identity;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Masofa.BusinessLogic.Identity.Users
{
    /// <summary>
    /// Базовая команда для обновления пользователей
    /// </summary>
    [RequestPermission(ActionType = "Update")]
    public class UserUpdateCommand : IRequest<User>
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Required]
        public required string UserName { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Второе имя пользователя
        /// </summary>
        public string? SecondName { get; set; }

        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Старый пароль пользователя
        /// </summary>
        public string? OldPassword { get; set; }

        /// <summary>
        /// Новый пароль пользователя
        /// </summary>
        public string? NewPassword { get; set; }

        /// <summary>
        /// Электронная почта пользователя
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Одобрен
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// Email подтвержден
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Заблокировать пользователя
        /// </summary>
        public bool LockUser { get; set; }

        /// <summary>
        /// Дата и время начала блокировки пользователя
        /// </summary>
        public DateTime? LockoutStart { get; set; }

        /// <summary>
        /// Дата и время окончания блокировки пользователя
        /// </summary>
        public DateTime? LockoutEnd { get; set; }

        /// <summary>
        /// Роли в которые добавить пользователя при обновлении
        /// </summary>
        public List<string> Roles { get; set; } = [];

        /// <summary>
        /// Пользователь-автор изменений
        /// </summary>
        [Required]
        public required string Author { get; set; }

        /// <summary>
        /// Комментарий к пользователю
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// Идентификатор заместителя пользователя
        /// </summary>
        public Guid? DeputyId { get; set; }

        /// <summary>
        /// Основание подключения
        /// </summary>
        public string? ConnectionBasis { get; set; }

        /// <summary>
        /// Идентификатор родительского пользователя
        /// </summary>
        public Guid ParentId { get; set; }
    }

    /// <summary>
    /// Событие которое срабатывает при обновлении пользователя
    /// </summary>
    public class UserUpdateEvent : INotification
    {
        public User CurrentModel { get; set; }
        public User OldModel { get; set; }
        public Guid UserId { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
    }

    public class UserUpdateCommandHandler : IRequestHandler<UserUpdateCommand, User>
    {

        public MasofaIdentityDbContext IdentityDbContext { get; set; }
        private ILogger Logger { get; set; }
        private IMediator Mediator { get; set; }
        private UserManager<User> UserManager { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public UserUpdateCommandHandler(
            MasofaIdentityDbContext identityDbContext,
            ILogger<UserUpdateCommandHandler> logger,
            IMediator mediator,
            UserManager<User> userManager,
            IBusinessLogicLogger businessLogicLogger)
        {
            IdentityDbContext = identityDbContext;
            Logger = logger;
            Mediator = mediator;
            UserManager = userManager;
            BusinessLogicLogger = businessLogicLogger;
        }

        public async Task<User> Handle(UserUpdateCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var user = await UserManager.FindByNameAsync(request.UserName);
                var oldUser = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(Newtonsoft.Json.JsonConvert.SerializeObject(user));
                if (user == null)
                {
                    var msg = $"User \"{request.UserName}\" not found";
                    await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                    throw new Exception(msg);
                }

                if (request.OldPassword != null && request.NewPassword != null && request.OldPassword != request.NewPassword)
                {
                    await UserManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
                }

                var currentRoles = await UserManager.GetRolesAsync(user);

                var removeResult = await UserManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    var msg = $"Removing roles failed. {requestPath}";
                    await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                    throw new Exception(msg);
                }

                if (request.Roles.Any())
                {
                    var addResult = await UserManager.AddToRolesAsync(user, request.Roles);
                    if (!addResult.Succeeded)
                    {
                        var msg = $"Adding roles failed. {requestPath}";
                        await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                        throw new Exception(msg);
                    }
                }

                if (request.LockUser && request.LockoutStart != null && request.LockoutEnd != null)
                {
                    var startDateTime = request.LockoutStart.Value;
                    var endDateTime = request.LockoutEnd.Value;

                    var startOffset = new DateTimeOffset(startDateTime, TimeSpan.Zero);
                    var endOffset = new DateTimeOffset(endDateTime, TimeSpan.Zero);

                    if (startOffset > endOffset)
                    {
                        var msg = $"StartDate cannot be later than EndDate. {requestPath}";
                        await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                        throw new Exception(msg);
                    }

                    user.ScheduledLockoutStart = startOffset;
                    user.ScheduledLockoutEnd = endOffset;

                    if (request.LockoutStart.HasValue && DateTime.UtcNow >= startDateTime)
                    {
                        await UserManager.SetLockoutEndDateAsync(user, endOffset);
                    }

                    var result = await UserManager.UpdateAsync(user);
                    if (!result.Succeeded)
                        throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
                }
                else if (!request.LockUser)
                {
                    // Разблокировка пользователя - очищаем все даты блокировки
                    user.ScheduledLockoutStart = null;
                    user.ScheduledLockoutEnd = null;
                    
                    // Сбрасываем текущую блокировку через UserManager
                    await UserManager.SetLockoutEndDateAsync(user, null);
                    
                    var result = await UserManager.UpdateAsync(user);
                    if (!result.Succeeded)
                        throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                if (request.Email != null && user.Email != request.Email)
                {
                    user.Email = request.Email;
                }

                var lastUpdateUser = IdentityDbContext.Set<User>().First(m => m.UserName.ToLower().Equals(request.Author.ToLower()));
                user.LastUpdateAt = DateTime.UtcNow;
                user.LastUpdateUser = lastUpdateUser.Id;
                user.EmailConfirmed = request.EmailConfirmed;
                user.Approved = request.Approved;
                
                // Обновляем поля имени
                if (request.FirstName != null)
                    user.FirstName = request.FirstName;
                if (request.SecondName != null)
                    user.SecondName = request.SecondName;
                if (request.LastName != null)
                    user.LastName = request.LastName;
                
                // Обновляем новые поля
                if (request.Comment != null)
                    user.Comment = request.Comment;
                if (request.DeputyId.HasValue)
                    user.DeputyId = request.DeputyId;
                if (request.ConnectionBasis != null)
                    user.ConnectionBasis = request.ConnectionBasis;
                    user.ParentId = request.ParentId;

                await UserManager.UpdateAsync(user);

                var authorUser = await UserManager.FindByNameAsync(request.Author);
                await Mediator.Publish(new UserUpdateEvent
                {
                    CurrentModel = user,
                    OldModel = oldUser,
                    UserId = authorUser.Id,
                    DateTime = DateTime.Now
                }, cancellationToken);

                return user;
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
}
