using Masofa.Common.Resources;
using Masofa.BusinessLogic.Identity.OneId;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Client.OneId;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.ViewModels.Account;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Masofa.Web.Monolith.Controllers.Identity
{
    /// <summary>
    /// Контроллер для авторизации чере OneId
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("identity/[controller]")]
    [ApiExplorerSettings(GroupName = "Identity")]
    public class OneIdController : BaseController
    {
        private IBusinessLogicLogger BusinessLogicLogger { get; }
        private AuthOptions AuthOptions { get; }
        private MasofaDictionariesDbContext DictionariesDbContext { get; }
        private MasofaIdentityDbContext IdentityDbContext { get; }
        private OneIdUnitOfWork OneIdUnitOfWork { get; }
        private UserManager<Masofa.Common.Models.Identity.User> UserManager { get; }

        public OneIdController(
            ILogger<OneIdController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            OneIdUnitOfWork oneIdUnitOfWork,
            UserManager<Masofa.Common.Models.Identity.User> userManager,
            MasofaIdentityDbContext identityDbContext,
            MasofaDictionariesDbContext dictionariesDbContext) : base(logger, configuration, mediator)
        {
            BusinessLogicLogger = businessLogicLogger;
            OneIdUnitOfWork = oneIdUnitOfWork;
            UserManager = userManager;
            IdentityDbContext = identityDbContext;
            AuthOptions = configuration.GetSection("AuthOptions").Get<AuthOptions>();
            DictionariesDbContext = dictionariesDbContext;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("[action]")]
        public async Task<ActionResult<string>> GetRedirectUrl()
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetRedirectUrl)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var redirectUrlResponse = await OneIdUnitOfWork.OneIdRepository.GetRedirectUrlAsync();
                var result = redirectUrlResponse.Url;

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath,result), requestPath);
                return result;
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
        /// LogIn method by OneId token
        /// </summary>
        /// <param name="code">Coede form OneId tunnel</param>
        /// <response code="200">Login complite. Returned JWT token</response>
        /// <response code="400">Model not valid</response>
        /// <response code="401">Invalid login or password</response>
        /// <response code="500">Internal Server Error. Unhandled.</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("[action]")]
        public async Task<ActionResult<string>> GetUserToken([FromBody] LoginByOneIdCode request)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetUserToken)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var oneIdToken = await OneIdUnitOfWork.OneIdRepository.GetUserTokenAsync(request.Code);

                var oneIdUserData = await OneIdUnitOfWork.OneIdRepository.GetUserDataAsync(oneIdToken.Access);

                await Mediator.Send(new UpsertOneIdUserCommand() { OneIdUserData = oneIdUserData });

                var currentPerson = await DictionariesDbContext.Persons.FirstOrDefaultAsync(p => p.Pinfl == oneIdUserData.Pinfl);
                if (currentPerson == null)
                {
                    //var errorMsg = $"User with Pinfl = {oneIdUserData.Pinfl} not found";
                    var errorMsg = LogMessageResource.UserNotFoundByPinfl(oneIdUserData.Pinfl.ToString());
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }

                var currentUser = await IdentityDbContext.Users.FirstOrDefaultAsync(u => u.UserBusinessId == currentPerson.Id);
                if (currentUser == null)
                {
                    //var errorMsg = $"User with Email = {oneIdUserData.Email} not found";
                    var errorMsg = $"User with Email = 30902942740042 not found";
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }

                var roles = (await UserManager.GetRolesAsync(currentUser))?.ToList() ?? new List<string>();

                var claims = new List<Claim> { new Claim(ClaimTypes.Name, currentUser.UserName) };

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

                await BusinessLogicLogger.LogInformationAsync($"Finish request in {requestPath} with result: {Newtonsoft.Json.JsonConvert.SerializeObject(tokenString)}", requestPath);

                return Ok(tokenString);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
