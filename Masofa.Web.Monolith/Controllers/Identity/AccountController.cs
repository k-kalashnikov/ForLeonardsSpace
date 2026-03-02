using Masofa.BusinessLogic.Identity.Users;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Identity;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Resources;
using Masofa.Common.Services.EmailSender;
using Masofa.Common.ViewModels.Account;
using Masofa.Web.Monolith.ViewModels.User;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Masofa.Web.Monolith.Controllers.Identity
{
    /// <summary>
    /// Контроллер для авторизации
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("identity/[controller]")]
    [ApiExplorerSettings(GroupName = "Identity")]
    public class AccountController : BaseController
    {
        private UserManager<User> UserManager { get; set; }
        private SignInManager<User> SignInManager { get; set; }
        private AuthOptions AuthOptions { get; set; }
        private IEmailSender EmailSender { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private IMemoryCache Cache { get; set; }
        private SmtpOptions Opt { get; set; }

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            IBusinessLogicLogger businessLogicLogger,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            IMediator mediator,
            IOptions<SmtpOptions> options,
            IMemoryCache cache) : base(logger, configuration, mediator)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            AuthOptions = configuration.GetSection("AuthOptions").Get<AuthOptions>();
            EmailSender = emailSender;
            BusinessLogicLogger = businessLogicLogger;
            Opt = options.Value;
            Cache = cache;
        }

        /// <summary>
        /// LogIn method. Use username with password and 2FA
        /// </summary>
        /// <param name="viewModel">Object contained username and password</param>
        /// <response code="200">Login checked. 2FA security code sent</response>
        /// <response code="400">Model not valid</response>
        /// <response code="401">Invalid login or password</response>
        /// <response code="500">Internal Server Error. Unhandled.</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("[action]")]
        public async Task<ActionResult<bool>> LoginBy2FA([FromBody] LoginAndPasswordViewModel viewModel)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(LoginByLoginPassword)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, viewModel.ToString()), requestPath);
                    return BadRequest(ModelState);
                }

                var user = await UserManager.FindByNameAsync(viewModel.UserName);

                if (user == null)
                {
                    var errorMsg = LogMessageResource.InvalidLogin();
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return Unauthorized(errorMsg);
                }

                if (user.IsInScheduledLockout)
                {
                    if (user.LockoutEnd != user.ScheduledLockoutEnd)
                    {
                        await UserManager.SetLockoutEndDateAsync(user, user.ScheduledLockoutEnd);
                    }

                    var errorMsg = "User is locked";
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return Forbid(errorMsg);
                }

                if (!user.Approved)
                {
                    var errorMsg = "User is not approved";
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return Forbid(errorMsg);
                }

                var signInResult = await SignInManager.CheckPasswordSignInAsync(user, viewModel.Password, false);
                if (!signInResult.Succeeded)
                {
                    var errorMsg = "Invalid login or password";
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return Unauthorized(errorMsg);
                }

                var code = new Random().Next(100000, 999999).ToString();
                Cache.Set($"2fa_{user.Id}", code, TimeSpan.FromMinutes(10));

                string[] sourceArray = ["en", "ru", "uz"];
                var culture = Request.Headers.AcceptLanguage.ToString().Split(',').First().Split('-').First();
                culture = sourceArray.Contains(culture) ? culture : "en";

                var subject = "Two-factor authentication security code";
                if (culture == "ru") subject = "Код безопасности двухфакторной аутентификации";
                if (culture == "uz") subject = "Ikki faktorli autentifikatsiya xavfsizlik kodi";

                var template = await EmailSender.LoadTemplateAsync("Code2FA", culture);
                template = template.Replace("{{firstName}}", user.FirstName);
                template = template.Replace("{{lastName}}", user.LastName);
                template = template.Replace("{{2faCode}}", code);

                var emailSent = await EmailSender.SendEmailAsync([user.Email], subject, template, null);
                if (!emailSent)
                {
                    await BusinessLogicLogger.LogCriticalAsync(LogMessageResource.TwoFACodeSendError(user.Email), requestPath);
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Error sending email. Please try again");
                }

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.TwoFACodeSent(user.Email), requestPath);

                return Ok(true);
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
        /// 2FA Confirmation
        /// </summary>
        /// <param name="userName">Идентификатор пользователя</param>
        /// <param name="code">Код безопасности</param>
        /// <response code="200">Login complete. Returned JWT token</response>
        /// <response code="400">Model not valid</response>
        /// <response code="401">Invalid login or password</response>
        /// <response code="500">Internal Server Error. Unhandled.</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("[action]")]
        public async Task<ActionResult<string>> ConfirmTwoFactor(string userName, string code)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ConfirmTwoFactor)}";
            try
            {
                var user = await UserManager.FindByNameAsync(userName);
                if (user == null)
                {
                    var errorMsg = LogMessageResource.InvalidLogin();
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return Unauthorized(errorMsg);
                }

                if (!Cache.TryGetValue($"2fa_{user.Id}", out string? storedCode) || storedCode != code)
                {
                    var errorMsg = "Invalid or expired code";
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return BadRequest(errorMsg);
                }

                Cache.Remove($"2fa_{user.Id}");

                var roles = (await UserManager.GetRolesAsync(user))?.ToList() ?? new List<string>();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName)
                };

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    IssuedAt = DateTime.Now,
                    Expires = DateTime.Now.AddHours(24),
                    Issuer = AuthOptions.ISSUER,
                    Audience = AuthOptions.AUDIENCE,
                    SigningCredentials = new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                return Ok(tokenString);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// LogIn method. Use username with password for response JWT token
        /// </summary>
        /// <param name="viewModel">Object contained username and password</param>
        /// <response code="200">Login complete. Returned JWT token</response>
        /// <response code="400">Model not valid</response>
        /// <response code="401">Invalid login or password</response>
        /// <response code="500">Internal Server Error. Unhandled.</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("[action]")]
        public async Task<ActionResult<string>> LoginByLoginPassword([FromBody] LoginAndPasswordViewModel viewModel)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(LoginByLoginPassword)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, viewModel.ToString()), requestPath);
                    return BadRequest(ModelState);
                }

                var user = await UserManager.FindByNameAsync(viewModel.UserName);

                if (user == null)
                {
                    var errorMsg = LogMessageResource.InvalidLogin();
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return Unauthorized(errorMsg);
                }

                if (user.IsInScheduledLockout)
                {
                    if (user.LockoutEnd != user.ScheduledLockoutEnd)
                    {
                        await UserManager.SetLockoutEndDateAsync(user, user.ScheduledLockoutEnd);
                    }

                    var errorMsg = "User is locked";
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return Forbid(errorMsg);
                }

                if (!user.Approved)
                {
                    var errorMsg = "User is not approved";
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return Forbid(errorMsg);
                }

                var signInResult = await SignInManager.PasswordSignInAsync(user, viewModel.Password, false, false);
                if (!signInResult.Succeeded)
                {
                    var errorMsg = "Invalid login or password";
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return Unauthorized(errorMsg);
                }

                var roles = (await UserManager.GetRolesAsync(user))?.ToList() ?? new List<string>();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName)
                };

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    IssuedAt = DateTime.Now,
                    Expires = DateTime.Now.AddHours(24),
                    Issuer = AuthOptions.ISSUER,
                    Audience = AuthOptions.AUDIENCE,
                    SigningCredentials = new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                return Ok(tokenString);
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
        /// Изменяет пароль текущего авторизованного пользователя
        /// </summary>
        /// <param name="viewModel">Модель с данными для смены пароля (старый и новый пароль)</param>
        /// <returns>Результат операции смены пароля</returns>
        /// <response code="200">Пароль успешно изменен</response>
        /// <response code="400">Некорректные данные модели</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">Пользователь не найден</response>
        /// <response code="409">Ошибка при смене пароля (неверный старый пароль или несоответствие требованиям)</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<bool>> ChangePassword([FromBody] ChangePasswordViewModel viewModel)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ChangePassword)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, viewModel.ToString()), requestPath);
                    return BadRequest(ModelState);
                }

                var currentUser = await UserManager.FindByNameAsync(User.Identity.Name);
                if (currentUser == null)
                {
                    var errorMsg = LogMessageResource.UserNotFound(User.Identity.Name);
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }

                var result = await UserManager.ChangePasswordAsync(currentUser, viewModel.OldPassword, viewModel.NewPassword);

                if (!result.Succeeded)
                {
                    var errorMsg = result.Errors.ToString();
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return StatusCode(StatusCodes.Status409Conflict, errorMsg);
                }

                return true;
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
        /// Получает информацию о профиле текущего авторизованного пользователя
        /// </summary>
        /// <returns>Информация о профиле пользователя</returns>
        /// <response code="200">Информация о профиле успешно получена</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<ProfileInfoViewModel>> GetProfileInfo()
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetProfileInfo)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var user = await UserManager.FindByNameAsync(User.Identity.Name);
                var roles = (await UserManager.GetRolesAsync(user))?.ToList() ?? new List<string>();

                return new ProfileInfoViewModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = roles,
                    UserName = user.UserName,
                    LastName = user.LastName,
                    FirstName = user.FirstName,
                };
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
        /// Отправляет письмо для восстановления пароля на указанный email
        /// </summary>
        /// <param name="model">Модель с email адресом для восстановления пароля</param>
        /// <returns>Результат операции отправки письма</returns>
        /// <response code="200">Письмо для восстановления пароля отправлено</response>
        /// <response code="400">Некорректные данные модели</response>
        /// <response code="404">Пользователь с указанным email не найден или email не подтвержден</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("[action]")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ForgotPassword)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(model)), requestPath);
                    return BadRequest(ModelState);
                }

                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !await UserManager.IsEmailConfirmedAsync(user))
                {
                    var errorMsg = LogMessageResource.UserEmailNotFound();
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }

                var token = await UserManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                string[] sourceArray = ["en", "ru", "uz"];
                var culture = Request.Headers.AcceptLanguage.ToString().Split(',').First().Split('-').First();
                culture = sourceArray.Contains(culture) ? culture : "en";

                var callbackUrl = $"{Opt.CallbackUrl}/identity/account/reset-password?userId={user.Id}&token={token}";

                var template = await EmailSender.LoadTemplateAsync("ResetPassword", culture);
                template = template.Replace("{{callbackUrl}}", callbackUrl);

                await EmailSender.SendEmailAsync([model.Email], "Reset password", template, null);

                return Ok("You'll receive email");
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
        /// Получает данные для сброса пароля по токену восстановления
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="token">Токен восстановления пароля</param>
        /// <returns>Данные для сброса пароля</returns>
        /// <response code="200">Данные для сброса пароля получены</response>
        /// <response code="400">Некорректные параметры userId или token</response>
        /// <response code="404">Пользователь не найден</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [AllowAnonymous]
        [Route("[action]")]
        public async Task<IActionResult> ResetPassword(string userId, string token)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ResetPassword)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                {
                    var errorMsg = LogMessageResource.ModelValidationFailed(requestPath, $"UserId {userId}, Token {token}");
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return BadRequest(errorMsg);
                }

                var user = await UserManager.FindByIdAsync(userId);
                if (user == null)
                {
                    var errorMsg = LogMessageResource.UserNotFound(userId.ToString());
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }

                var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

                return Ok(new
                {
                    UserId = userId,
                    Token = decodedToken,
                    RequiresNewPassword = true
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
        /// Сбрасывает пароль пользователя по токену восстановления
        /// </summary>
        /// <param name="model">Модель с данными для сброса пароля (userId, token, новый пароль)</param>
        /// <returns>Результат операции сброса пароля</returns>
        /// <response code="200">Пароль успешно сброшен</response>
        /// <response code="400">Некорректные данные модели или ошибки валидации пароля</response>
        /// <response code="404">Пользователь не найден</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("[action]")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ResetPassword)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(model)), requestPath);
                    return BadRequest(ModelState);
                }

                var user = await UserManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    var errorMsg = LogMessageResource.UserNotFound(model.UserId);
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }

                var result = await UserManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                if (result.Succeeded)
                {
                    var msg = LogMessageResource.GenericError(requestPath, string.Join(";\n", result.Errors.Select(m => $"{m.Code} - {m.Description}")));
                    await BusinessLogicLogger.LogInformationAsync(msg, requestPath);
                    return Ok(msg);
                }

                return BadRequest(result.Errors);
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
        /// Обновляет данные пользователя (email, пароль, роли, статус блокировки)
        /// </summary>
        /// <param name="viewModel">Модель с данными для обновления пользователя</param>
        /// <returns>Обновленные данные пользователя</returns>
        /// <response code="200">Данные пользователя успешно обновлены</response>
        /// <response code="400">Некорректные данные модели</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPut]
        [Route("[action]")]
        public virtual async Task<ActionResult<User>> Update([FromBody] UpdateViewModel viewModel)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Update)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, viewModel.ToString()), requestPath);
                    return BadRequest(ModelState);
                }

                var updateRequest = new UserUpdateCommand()
                {
                    Author = User.Identity?.Name ?? string.Empty,
                    Email = viewModel.Email ?? string.Empty,
                    OldPassword = viewModel.OldPassword,
                    NewPassword = viewModel.NewPassword,
                    Roles = viewModel.Roles,
                    UserName = viewModel.UserName,
                    FirstName = viewModel.FirstName,
                    SecondName = viewModel.SecondName,
                    LastName = viewModel.LastName,
                    Approved = viewModel.Approved,
                    EmailConfirmed = viewModel.EmailConfirmed,
                    LockUser = viewModel.LockUser,
                    LockoutStart = viewModel.LockoutStart,
                    LockoutEnd = viewModel.LockoutEnd,
                    Comment = viewModel.Comment,
                    DeputyId = viewModel.DeputyId,
                    ConnectionBasis = viewModel.ConnectionBasis,
                    ParentId = viewModel.ParentId
                };
                var result = await Mediator.Send(updateRequest);
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(result)), requestPath);
                return result;
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {GetType().FullName}=>{nameof(Update)}";
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
