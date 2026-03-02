using Masofa.BusinessLogic.Identity.Users;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Identity;
using Masofa.Common.Resources;
using Masofa.Common.Services.EmailSender;
using Masofa.Common.ViewModels.Account;
using Masofa.DataAccess;
using Masofa.Web.Monolith.ViewModels.User;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection;
using System.Text.Encodings.Web;

namespace Masofa.Web.Monolith.Controllers.Identity
{
    /// <summary>
    /// Provides endpoints for managing users in the system, including user creation, email confirmation, user
    /// retrieval, and bulk import operations.
    /// </summary>
    /// <remarks>This controller is part of the Identity API group and requires JWT-based authentication for
    /// most operations. It supports administrative actions such as creating users, confirming email addresses, and
    /// importing users in bulk. The controller also provides endpoints for retrieving user details and managing
    /// user-related data.  The controller is designed to handle both individual user operations and bulk operations,
    /// such as importing users from a file. It ensures proper logging and error handling for all operations.</remarks>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("identity/[controller]")]
    [ApiExplorerSettings(GroupName = "Identity")]
    public class UserController : BaseController
    {
        private MasofaIdentityDbContext MasofaIdentityDbContext { get; set; }
        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
        private UserManager<User> UserManager { get; set; }
        private UrlEncoder UrlEncoder { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private IEmailSender EmailSender { get; set; }
        private const string NewAccountSubject = "Account created";

        public UserController(
            MasofaIdentityDbContext masofaIdentityDbContext,
            MasofaCommonDbContext masofaCommonDbContext,
            ILogger<UserController> logger,
            IConfiguration configuration,
            IMediator mediator,
            UserManager<User> userManager,
            IBusinessLogicLogger businessLogicLogger,
            IEmailSender emailSender,
            UrlEncoder urlEncoder) : base(
                logger,
                configuration,
                mediator)
        {
            MasofaIdentityDbContext = masofaIdentityDbContext;
            MasofaCommonDbContext = masofaCommonDbContext;
            UserManager = userManager;
            BusinessLogicLogger = businessLogicLogger;
            EmailSender = emailSender;
            UrlEncoder = urlEncoder;
        }

        /// <summary>
        /// Создает нового пользователя в системе
        /// </summary>
        /// <param name="viewModel">Модель с данными для создания пользователя</param>
        /// <returns>Результат создания пользователя с ID и статусом</returns>
        /// <response code="200">Пользователь успешно создан</response>
        /// <response code="400">Некорректные данные модели</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin")]
        public async Task<ActionResult<UserCreateCommandResult>> Create([FromBody] CreateViewModel viewModel)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Create)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, viewModel.ToString()), requestPath);
                    return BadRequest(ModelState);
                }

                var user = await UserManager.FindByNameAsync(viewModel.UserName);
                if (user != null)
                {
                    return BadRequest("User is already exist");
                }

                var createRequest = new UserCreateCommand()
                {
                    Author = User?.Identity?.Name ?? string.Empty,
                    Email = viewModel.Email,
                    Password = viewModel.Password,
                    Roles = viewModel.Roles,
                    UserName = viewModel.UserName,
                    FirstName = viewModel.FirstName,
                    SecondName = viewModel.SecondName,
                    LastName = viewModel.LastName,
                    UserBusinessType = viewModel.UserBusinessType,
                    UserBusinessId = viewModel.UserBusinessId,
                    Comment = viewModel.Comment,
                    DeputyId = viewModel.DeputyId,
                    ConnectionBasis = viewModel.ConnectionBasis,
                    ParentId = viewModel.ParentId,
                    Approved = viewModel.Approved,
                    EmailConfirmed = viewModel.EmailConfirmed,
                };
                var result = await Mediator.Send(createRequest);

                if (result.Errors.Count > 0)
                {
                    var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
                    await BusinessLogicLogger.LogErrorAsync($"User creation failed: {errorMessages}", requestPath);
                    return BadRequest(result);
                }

                string[] sourceArray = ["en", "ru", "uz"];
                var culture = Request.Headers.AcceptLanguage.ToString().Split(',').First().Split('-').First();
                culture = sourceArray.Contains(culture) ? culture : "en";

                var newUser = await UserManager.FindByIdAsync(result.Id.ToString());
                if (newUser == null)
                {
                    return BadRequest("Unable to find newly created user.");
                }

                var token = await UserManager.GenerateEmailConfirmationTokenAsync(newUser);
                var encodedToken = WebUtility.UrlEncode(token);

                var confirmUrl = GenerateConfirmUrl(newUser.Id, encodedToken);

                var template = await EmailSender.LoadTemplateAsync("NewAccount", culture);
                template = template.Replace("{{firstName}}", viewModel.FirstName);
                template = template.Replace("{{lastName}}", viewModel.LastName);
                template = template.Replace("{{userName}}", viewModel.UserName);
                template = template.Replace("{{password}}", viewModel.Password);
                template = template.Replace("{{confirmUrl}}", confirmUrl);

                await EmailSender.SendEmailAsync([viewModel.Email], NewAccountSubject, template, null);

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(result)), requestPath);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private string GenerateConfirmUrl(Guid userId, string? token)
        {
            var controllerType = typeof(UserController);
            var classRoute = controllerType.GetCustomAttribute<RouteAttribute>()?.Template.Replace("[controller]", "User") ?? "";
            var methodName = nameof(ConfirmEmail);
            var methodInfo = controllerType.GetMethod(methodName);
            var methodRoute = methodInfo?.GetCustomAttribute<HttpGetAttribute>()?.Template ?? "";
            var fullRoute = $"{classRoute}/{methodRoute}{methodName}";
            var confirmUrl = $"{Request.Scheme}://{Request.Host}/{classRoute}/{methodRoute}{methodName}?userId={userId}&token={token}";
            return confirmUrl;
        }

        /// <summary>
        /// Подтверждение электронной почты пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="token">Токен</param>
        /// <returns>Результат создания пользователя с ID и статусом</returns>
        /// <response code="200">Пользователь успешно создан</response>
        /// <response code="400">Некорректные данные модели</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [AllowAnonymous]
        [Route("[action]")]
        public async Task<IActionResult> ConfirmEmail(Guid userId, string token)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ConfirmEmail)}";
            try
            {
                var user = await UserManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    var errorMsg = LogMessageResource.UserNotFoundById(userId.ToString());
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }

                var result = await UserManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    string[] sourceArray = ["en", "ru", "uz"];
                    var culture = Request.Headers.AcceptLanguage.ToString().Split(',').First().Split('-').First();
                    culture = sourceArray.Contains(culture) ? culture : "en";

                    var msg = LogMessageResource.EmailConfirmed();
                    if (culture == "ru") msg["ru-RU"] = "Email подтверждён!";
                    if (culture == "uz") msg["uz-Latn-UZ"] = "Elektron pochta tasdiqlandi!";

                    return Ok(msg);
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {requestPath}. {ex.Message}";
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Повторно отправляет письмо с данными нового аккаунта пользователю
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns>Результат операции отправки письма</returns>
        /// <response code="200">Письмо успешно отправлено</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">Пользователь не найден или письмо с данными аккаунта не найдено</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("[action]/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin")]
        public virtual async Task<ActionResult> ResendNewAccountLetter(Guid id)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ResendNewAccountLetter)}";
            try
            {
                var user = await UserManager.FindByIdAsync(id.ToString());

                if (user == null)
                {
                    var errorMsg = LogMessageResource.EntityNotFound(typeof(User).FullName, id.ToString());
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }

                var emailMessage = await MasofaCommonDbContext.EmailMessages
                    .Where(m => user.Email != null && m.Recipients.Contains(user.Email) && m.Subject != null && m.Subject.Equals(NewAccountSubject))
                    .OrderByDescending(m => m.CreateAt)
                    .FirstOrDefaultAsync();

                if (emailMessage == null)
                {
                    var errorMsg = $"'{NewAccountSubject}' email not found for User {id.ToString()}";
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }

                await EmailSender.SendEmailAsync([user.Email], NewAccountSubject, emailMessage.Body, null);

                return Ok();
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Получает список пользователей по заданному запросу с фильтрацией, сортировкой и пагинацией
        /// </summary>
        /// <param name="query">Объект запроса с параметрами фильтрации, сортировки и пагинации</param>
        /// <returns>Список найденных пользователей</returns>
        /// <response code="200">Список пользователей успешно получен</response>
        /// <response code="400">Некорректные параметры запроса</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("[action]")]
        public virtual async Task<ActionResult<List<ProfileInfoViewModel>>> GetByQuery([FromBody] UserGetQuery query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetByQuery)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(query)), requestPath);
                    return BadRequest(ModelState);
                }
                var getRequest = new UserGetRequest()
                {
                    Query = query
                };
                return await Mediator.Send(getRequest);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Получает информацию о пользователе по уникальному идентификатору
        /// </summary>
        /// <param name="id">Уникальный идентификатор пользователя</param>
        /// <returns>Информация о пользователе</returns>
        /// <response code="200">Информация о пользователе успешно получена</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">Пользователь с указанным ID не найден</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("[action]/{id}")]
        public virtual async Task<ActionResult<ProfileInfoViewModel>> GetById(Guid id)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetById)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var user = await MasofaIdentityDbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    var errorMsg = LogMessageResource.EntityNotFound(typeof(User).FullName, id.ToString());
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }

                return Ok(new ProfileInfoViewModel()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    SecondName = user.SecondName,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    Roles = (await UserManager.GetRolesAsync(user)).ToList(),
                    IsActive = user.IsActive,
                    Approved = user.Approved,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutStart = user.ScheduledLockoutStart?.DateTime,
                    LockoutEnd = user.ScheduledLockoutEnd?.DateTime,
                    UserBusinessType = user.UserBusinessType,
                    CreateAt = user.CreateAt,
                    LastUpdateAt = user.LastUpdateAt,
                    CreateUser = user.CreateUser,
                    LastUpdateUser = user.LastUpdateUser,
                    ParentId = user.ParentId,
                    DeputyId = user.DeputyId,
                    UserBusinessId = user.UserBusinessId,
                    Comment = user.Comment,
                    ConnectionBasis = user.ConnectionBasis,
                    ScheduledLockoutStart = user.ScheduledLockoutStart?.DateTime,
                    ScheduledLockoutEnd = user.ScheduledLockoutEnd?.DateTime
                });
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Получает список дочерних пользователей текущего авторизованного пользователя
        /// </summary>
        /// <returns>Список дочерних пользователей</returns>
        /// <response code="200">Список дочерних пользователей успешно получен</response>
        /// <response code="400">Текущий пользователь не определен</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<ProfileInfoViewModel>>> GetChildUsers()
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetChildUsers)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var currentUser = MasofaIdentityDbContext.Users.FirstOrDefault(m => m.UserName.Equals(User.Identity.Name));
                if (currentUser == null)
                {
                    var errorMsg = LogMessageResource.UserIsNotDefined();
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return BadRequest(errorMsg);
                }

                var users = MasofaIdentityDbContext.Users.Where(m => m.ParentId.Equals(currentUser.Id))?.ToList();
                var result = new List<ProfileInfoViewModel>();
                foreach (var item in users)
                {
                    var temp = new ProfileInfoViewModel()
                    {
                        Id = item.Id,
                        Email = item.Email,
                        Roles = (await UserManager.GetRolesAsync(item)).ToList(),
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        UserName = item.UserName
                    };
                    result.Add(temp);
                }
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath,result.Count.ToString()), requestPath);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Импортирует пользователей - физических лиц из ZIP файла
        /// </summary>
        /// <param name="file">ZIP файл с данными пользователей</param>
        /// <returns>Результат импорта пользователей</returns>
        /// <response code="200">Импорт пользователей выполнен</response>
        /// <response code="400">Файл пустой или некорректный</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin")]
        public async Task<ActionResult<BulkUserCreateResult>> ImportUserPerson(IFormFile file) =>
            await ImportUser(file, UserBusinessType.Person);

        /// <summary>
        /// Импортирует пользователей - юридических лиц из ZIP файла
        /// </summary>
        /// <param name="file">ZIP файл с данными пользователей</param>
        /// <returns>Результат импорта пользователей</returns>
        /// <response code="200">Импорт пользователей выполнен</response>
        /// <response code="400">Файл пустой или некорректный</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin")]
        public async Task<ActionResult<BulkUserCreateResult>> ImportUserFirm(IFormFile file) =>
            await ImportUser(file, UserBusinessType.Firm);

        private async Task<ActionResult<BulkUserCreateResult>> ImportUser(IFormFile file, UserBusinessType userBusinessType)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetChildUsers)}";
            try
            {
                if (file is null || file.Length == 0)
                {
                    var errorMsg = LogMessageResource.ZipFileIsEmpty();
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return BadRequest(errorMsg);
                }

                var importCommand = new UserImportCommand
                {
                    UserBusinessType = userBusinessType,
                    File = file,
                    Author = User?.Identity?.Name ?? string.Empty
                };

                return await Mediator.Send(importCommand);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Получает список пользователей в формате фильтра (только ID и полное имя)
        /// </summary>
        /// <returns>Список пользователей для фильтров</returns>
        /// <response code="200">Список пользователей успешно получен</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<UserFilter>>> GetUserFilters()
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetUserFilters)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                
                var result = await Mediator.Send(new UserFilterGetRequest());

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath,result.Count.ToString()), requestPath);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
