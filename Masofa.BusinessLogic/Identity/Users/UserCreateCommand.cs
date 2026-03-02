using DocumentFormat.OpenXml.Office2010.Excel;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Identity;
using Masofa.Common.Resources;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Masofa.BusinessLogic.Identity.Users
{
    /// <summary>
    /// Базовая команда для создания пользователей
    /// </summary>
    public class UserCreateCommand : IRequest<UserCreateCommandResult>
    {
        /// <summary>
        /// Имя НОВОГО пользователя
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Имя нового пользователя
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Второе имя нового пользователя
        /// </summary>
        public string? SecondName { get; set; }

        /// <summary>
        /// Фамилия нового пользователя
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        [Required]
        public string Password { get; set; }

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
        /// Роли в которые добавить пользователя при создании
        /// </summary>
        public List<string> Roles { get; set; }

        /// <summary>
        /// Тип пользователя
        /// </summary>
        public UserBusinessType UserBusinessType { get; set; }

        /// <summary>
        /// Ссылка на физ или юр лицо
        /// </summary>
        public Guid UserBusinessId { get; set; }

        /// <summary>
        /// Пользователь-автор изменений
        /// </summary>
        [Required]
        public string Author { get; set; }

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
    /// Базовое событие для создания пользователей
    /// </summary>
    /// <typeparam name="TModel">Тип модели, которую добавили</typeparam>
    /// <typeparam name="TDbContext">Контекст Базы данных, в которой эта модель храниться</typeparam>>
    public class UserCreateEvent : INotification
    {
        public User User { get; set; }
        public Guid AuthorUserId { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
    }

    public class UserCreateCommandHandler : IRequestHandler<UserCreateCommand, UserCreateCommandResult>
    {
        private MasofaIdentityDbContext IdentityDbContext { get; set; }
        private ILogger Logger { get; set; }
        private IMediator Mediator { get; set; }
        private UserManager<User> UserManager { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public UserCreateCommandHandler(
            MasofaIdentityDbContext identityDbContext,
            ILogger<UserCreateCommandHandler> logger,
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

        public async Task<UserCreateCommandResult> Handle(UserCreateCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var lastUpdateUser = await IdentityDbContext.Set<User>().FirstAsync(m => m.UserName.ToLower().Equals(request.Author.ToLower()));
                var password = string.IsNullOrEmpty(request.Password) ? RandomGenerator.GenerateStrongPassword(12) : request.Password;

                var user = new User
                {
                    UserName = request.UserName,
                    FirstName = request.FirstName ?? string.Empty,
                    SecondName = request.SecondName ?? string.Empty,
                    LastName = request.LastName ?? string.Empty,
                    Email = request.Email,
                    CreateAt = DateTime.UtcNow,
                    CreateUser = lastUpdateUser.Id,
                    EmailConfirmed = request.EmailConfirmed,
                    Approved = request.Approved,
                    UserBusinessType = request.UserBusinessType,
                    UserBusinessId = request.UserBusinessId,
                    Comment = request.Comment,
                    DeputyId = request.DeputyId,
                    ConnectionBasis = request.ConnectionBasis,
                    ParentId = request.ParentId
                };

                var result = await UserManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    return new UserCreateCommandResult()
                    {
                        Errors = [.. result.Errors]
                    };
                }
                user = await UserManager.FindByNameAsync(request.UserName);

                if (user != null && request.LockUser)
                {
                    var startDateTime = request.LockoutStart.Value;
                    var endDateTime = request.LockoutEnd.Value;

                    var startOffset = new DateTimeOffset(startDateTime, TimeSpan.Zero);
                    var endOffset = new DateTimeOffset(endDateTime, TimeSpan.Zero);

                    if (startOffset > endOffset)
                    {
                        var errorMessage = "StartDate cannot be later than EndDate.";
                        var msg = LogMessageResource.GenericError(requestPath, errorMessage);
                        await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                        throw new Exception(msg);
                    }

                    user.ScheduledLockoutStart = startOffset;
                    user.ScheduledLockoutEnd = endOffset;

                    if (request.LockoutStart.HasValue && DateTime.UtcNow >= startDateTime)
                    {
                        await UserManager.SetLockoutEndDateAsync(user, endOffset);
                    }
                }

                foreach (var role in request.Roles)
                {
                    await UserManager.AddToRoleAsync(user, role);
                }

                var authorUser = await UserManager.FindByNameAsync(request.Author);
                await Mediator.Publish(new UserCreateEvent
                {
                    User = user,
                    AuthorUserId = authorUser.Id,
                    DateTime = DateTime.Now
                }, cancellationToken);

                return new UserCreateCommandResult()
                {
                    Id = user?.Id ?? Guid.Empty,
                    Password = password,
                    UserName = user?.UserName ?? string.Empty,
                };
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

    public class UserCreateCommandResult
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<IdentityError> Errors { get; set; } = new List<IdentityError>();
    }
}
