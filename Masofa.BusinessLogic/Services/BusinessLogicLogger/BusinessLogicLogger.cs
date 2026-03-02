using Masofa.Common.Models.Identity;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.EmailSender;
using Masofa.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RTools_NTS.Util;
using System.Reactive.Subjects;
using System.Text;

namespace Masofa.BusinessLogic.Services.BusinessLogicLogger
{
    public interface IBusinessLogicLogger
    {
        Task LogInformationAsync(string message, string path);
        Task LogCriticalAsync(string message, string path);
        Task LogWarningAsync(string message, string path);
        Task LogErrorAsync(string message, string path);
        Task LogDebugAsync(string message, string path);
        Task LogAsync(LogMessage message);

    }

    public class BusinessLogicLogger : IBusinessLogicLogger
    {
        private MasofaCommonDbContext CommonDbContext { get; set; }
        private ILogger Logger { get; set; }
        private IHttpContextAccessor HttpContextAccessor { get; set; }
        private UserManager<User> UserManager { get; set; }
        private IEmailSender EmailSender { get; set; }
        private SmtpOptions Opt { get; set; }

        private Guid _callStackId;

        public BusinessLogicLogger(MasofaCommonDbContext commonDbContext, 
            ILogger<BusinessLogicLogger> logger, 
            IHttpContextAccessor httpContextAccessor, 
            UserManager<User> userManager, 
            IEmailSender emailSender, IOptions<SmtpOptions> opt)
        {
            CommonDbContext = commonDbContext;
            Logger = logger;
            _callStackId = Guid.NewGuid();
            HttpContextAccessor = httpContextAccessor;
            UserManager = userManager;
            EmailSender = emailSender;
            Opt = opt.Value;
        }

        public async Task LogCriticalAsync(string message, string path)
        {
            var logMessage = new LogMessage
            {
                Message = message,
                Path = path,
                LogMessageType = LogLevelType.Critical,
            };

            var to = new List<string> { Opt.HealthCheckRecipient };
            var subject = "BusinessLogger LogCritical";

            try
            {
                var emailSent = await EmailSender.SendEmailAsync(to, subject, message, null);
                if (!emailSent)
                {
                    await LogAsync(new LogMessage
                    {
                        Message = $"Critical email failed to send to {Opt.HealthCheckRecipient}",
                        Path = path,
                        LogMessageType = LogLevelType.Error
                    });
                }
            }
            catch (Exception ex)
            {
                await LogAsync(new LogMessage
                {
                    Message = $"Exception during critical email send: {ex}",
                    Path = path,
                    LogMessageType = LogLevelType.Error
                });
            }

            await LogAsync(logMessage);
        }

        public async Task LogDebugAsync(string message, string path)
        {
            var logMessage = new LogMessage
            {
                Message = message,
                Path = path,
                LogMessageType = LogLevelType.Debug,
            };

            await LogAsync(logMessage);
        }

        public async Task LogErrorAsync(string message, string path)
        {
            var logMessage = new LogMessage
            {
                Message = message,
                Path = path,
                LogMessageType = LogLevelType.Error,
            };

            await LogAsync(logMessage);
        }

        public async Task LogInformationAsync(string message, string path)
        {
            var logMessage = new LogMessage
            {
                Message = message,
                Path = path,
                LogMessageType = LogLevelType.Information,
            };

            await LogAsync(logMessage);
        }

        public async Task LogWarningAsync(string message, string path)
        {
            var logMessage = new LogMessage
            {
                Message = message,
                Path = path,
                LogMessageType = LogLevelType.Warning,
            };

            await LogAsync(logMessage);
        }

        public async Task LogAsync(LogMessage message)
        {
            try
            {
                User? currentUser = null;
                string? userName = HttpContextAccessor.HttpContext?.User?.Identity?.Name;

                var userId = Guid.Empty;
                string userFullName = string.Empty;

                if (!string.IsNullOrEmpty(userName))
                {
                    currentUser = await UserManager.FindByNameAsync(userName);
                    if (currentUser != null)
                    {
                        userId = currentUser.Id;
                        userFullName = $"{currentUser.FirstName} {currentUser.LastName}".Trim();
                    }
                }

                var trackedCallStack = CommonDbContext.CallStacks.Local
                    .FirstOrDefault(c => c.Id == _callStackId);

                CallStack callStack;

                if (trackedCallStack != null)
                {
                    callStack = trackedCallStack;
                }
                else
                {
                    callStack = await CommonDbContext.CallStacks.FindAsync(_callStackId);

                    if (callStack == null)
                    {
                        callStack = new CallStack
                        {
                            Id = _callStackId,
                            CreateUserId = userId,
                            CreateUserName = userName ?? "system",
                            CreateUserFullName = userFullName
                        };

                        await CommonDbContext.CallStacks.AddAsync(callStack);
                    }
                }

                message.Order = await CommonDbContext.LogMessages
                    .CountAsync(m => m.CallStackId == _callStackId);

                message.CallStackId = _callStackId;

                await CommonDbContext.LogMessages.AddAsync(message);
                await CommonDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex.Message);
            }
        }
    }
}
