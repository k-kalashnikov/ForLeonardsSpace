using Masofa.BusinessLogic;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Identity;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.EmailSender;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using System.Linq;

namespace Masofa.Web.Monolith.Jobs.System
{
    public class HealthCheckJob : IJob
    {
        private IServiceProvider ServiceProvider { get; set; }
        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
        private IEmailSender EmailSender { get; set; }
        private IOptions<SmtpOptions> SmtpOptions { get; set; }
        private ILogger<HealthCheckJob> Logger { get; set; }
        private IHealthCheckLocalizationProvider LocalizationProvider { get; }

        private UserManager<User> UserManager { get; set; }

        public HealthCheckJob(
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            ILogger<HealthCheckJob> logger,
            IServiceProvider serviceProvider,
            MasofaCommonDbContext masofaCommonDbContext,
            IEmailSender emailSender,
            IOptions<SmtpOptions> smtpOptions,
            UserManager<User> userManager,
            IHealthCheckLocalizationProvider localizationProvider)
        {
            ServiceProvider = serviceProvider;
            MasofaCommonDbContext = masofaCommonDbContext;
            EmailSender = emailSender;
            SmtpOptions = smtpOptions;
            Logger = logger;
            UserManager = userManager;
            LocalizationProvider = localizationProvider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var defaultLanguage = LocalizationProvider.ResolveLanguageKey(SmtpOptions.Value.HealthCheckLanguage ?? "ru");

                var dbContexts = typeof(MasofaCommonDbContext).Assembly.GetTypes()
                    .Where(m => m.IsSubclassOf(typeof(DbContext)));

                var modules = new List<ModuleCheckItem>();
                foreach (var item in dbContexts)
                {
                    var tempModule = new ModuleCheckItem()
                    {
                        ModuleName = item.Name.Replace("Masofa", "").Replace("DbContext", ""),
                        ModuleDbContextType = item,
                        ModuleNames = LocalizationProvider.GetModuleNames(item)
                    };
                    modules.Add(tempModule);
                }
                using (var scope = ServiceProvider.CreateScope())
                {
                    var result = new HealthCheckResult();
                    foreach (var item in modules)
                    {
                        result.Modules.Add(GetModuleResult(item, scope.ServiceProvider, defaultLanguage));
                    }
                    MasofaCommonDbContext.HealthCheckResults.Add(result);
                    MasofaCommonDbContext.SaveChanges();

                    // Отправляем отчет на email
                    await SendHealthCheckReportAsync(result);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error executing HealthCheckJob");
            }
        }

        private Masofa.Common.Models.SystemCrical.HealthCheckModuleResult GetModuleResult(ModuleCheckItem module, IServiceProvider serviceProvider, string defaultLanguage)
        {
            var result = new HealthCheckModuleResult();
            result.ModuleName = module.ModuleName;
            result.ModuleNames = module.ModuleNames;

            var dbContext = (DbContext)serviceProvider.GetRequiredService(module.ModuleDbContextType);
            var models = dbContext.Model.GetEntityTypes();
            foreach (var modelItem in models)
            {
                var tempModelResult = GetModelResult(modelItem.ClrType, dbContext, serviceProvider, defaultLanguage);
                result.Mоdels.Add(tempModelResult);
            }
            dbContext.Dispose();
            return result;
        }

        private Masofa.Common.Models.SystemCrical.HealthCheckModuleModelResult GetModelResult(Type modelType, DbContext dbContext, IServiceProvider serviceProvider, string defaultLanguage)
        {
            var result = new HealthCheckModuleModelResult();
            result.ModelName = modelType.FullName;
            result.ModelNames = LocalizationProvider.GetModelNames(modelType);

            var tableName = dbContext.Model.FindEntityType(modelType).GetTableName();
            try
            {
                if (dbContext.IsTableExists(tableName))
                {
                    result.DbLayerCheck = true;
                }
                else
                {
                    result.DbLayerCheck = false;
                    AddLocalizedError(result, LocalizationProvider.GetMissingTableError(tableName), defaultLanguage);
                }
            }
            catch (Exception)
            {
                result.DbLayerCheck = false;
                AddLocalizedError(result, LocalizationProvider.GetDatabaseMissingError(dbContext.GetType().Name), defaultLanguage);
            }


            if (modelType.IsSubclassOf(typeof(BaseEntity)))
            {
                var getType = typeof(IRequestHandler<,>).MakeGenericType(typeof(BaseGetRequest<,>).MakeGenericType(modelType, dbContext.GetType()), typeof(List<>).MakeGenericType(modelType));
                var getByIdType = typeof(IRequestHandler<,>).MakeGenericType(typeof(BaseGetByIdRequest<,>).MakeGenericType(modelType, dbContext.GetType()), modelType);
                var updateType = typeof(IRequestHandler<,>).MakeGenericType(typeof(BaseUpdateCommand<,>).MakeGenericType(modelType, dbContext.GetType()), modelType);
                var createType = typeof(IRequestHandler<,>).MakeGenericType(typeof(BaseCreateCommand<,>).MakeGenericType(modelType, dbContext.GetType()), typeof(Guid));
                var deleteType = typeof(IRequestHandler<,>).MakeGenericType(typeof(BaseDeleteCommand<,>).MakeGenericType(modelType, dbContext.GetType()), typeof(bool));

                var getTypeService = serviceProvider.GetService(getType);
                var getByIdTypeService = serviceProvider.GetService(getByIdType);
                var updateTypeService = serviceProvider.GetService(updateType);
                var createTypeService = serviceProvider.GetService(createType);
                var deleteTypeService = serviceProvider.GetService(deleteType);

                var flag = true;

                if (getTypeService == null)
                {
                    flag = false;
                    AddLocalizedError(result, LocalizationProvider.GetRequestNotRegisteredError("get", modelType.Name), defaultLanguage);
                }

                if (getByIdTypeService == null)
                {
                    flag = false;
                    AddLocalizedError(result, LocalizationProvider.GetRequestNotRegisteredError("getbyid", modelType.Name), defaultLanguage);
                }

                if (updateTypeService == null)
                {
                    flag = false;
                    AddLocalizedError(result, LocalizationProvider.GetRequestNotRegisteredError("update", modelType.Name), defaultLanguage);
                }

                if (createTypeService == null)
                {
                    flag = false;
                    AddLocalizedError(result, LocalizationProvider.GetRequestNotRegisteredError("create", modelType.Name), defaultLanguage);
                }

                if (deleteTypeService == null)
                {
                    flag = false;
                    AddLocalizedError(result, LocalizationProvider.GetRequestNotRegisteredError("delete", modelType.Name), defaultLanguage);
                }

                result.LogicLayerCheck = flag;
            }
            else
            {
                //TODO - раширить для работы с не BaseEntity
                result.LogicLayerCheck = true;
            }
            //TODO - раширить для работы с WebApi
            result.WebApi = true;
            return result;
        }

        private async Task SendHealthCheckReportAsync(HealthCheckResult healthCheckResult)
        {
            try
            {
                // Получаем email получателя из настроек
                var recipientEmail = SmtpOptions.Value.HealthCheckRecipient ?? SmtpOptions.Value.From;
                var recipients = recipientEmail.Split(",").ToList();

                var adminEmails = (await UserManager.GetUsersInRoleAsync("SystemAdmin")).Select(u => u.Email).ToList();
                foreach (var adminEmail in adminEmails)
                {
                    if (adminEmail != null) recipients.Add(adminEmail);
                }

                if (recipients.Count == 0)
                {
                    Logger.LogCritical("There is no recipients for Health Check report");
                    return;
                }

                // Загружаем шаблон с языком из конфигурации
                var language = SmtpOptions.Value.HealthCheckLanguage ?? "ru";
                var languageKey = LocalizationProvider.ResolveLanguageKey(language);
                var template = await EmailSender.LoadTemplateAsync("HealthCheckReport", language);

                // Генерируем HTML отчет
                var localizedErrorMessages = CollectLocalizedErrors(healthCheckResult, languageKey);
                var htmlReport = GenerateHealthCheckHtml(template, healthCheckResult, languageKey, localizedErrorMessages);

                // Формируем тему письма
                var errorCount = localizedErrorMessages.Count;
                var subject = LocalizationProvider.GetEmailSubject(errorCount, languageKey);

                // Отправляем email
                var success = await EmailSender.SendEmailAsync(
                    recipients,
                    subject,
                    htmlReport,
                    null
                );

                if (success)
                {
                    Logger.LogInformation("Health check report sent successfully to {Email}", recipientEmail);
                }
                else
                {
                    Logger.LogWarning("Failed to send health check report to {Email}", recipientEmail);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error sending health check report");
            }
        }

        private string GenerateHealthCheckHtml(string template, HealthCheckResult result, string languageKey, List<string> localizedErrors)
        {
            var emailTexts = LocalizationProvider.GetEmailTexts(languageKey);

            var totalModules = result.Modules.Count;
            var healthyModules = result.Modules.Count(m => m.Errors.Count == 0);
            var totalModels = result.Modules.Sum(m => m.Mоdels.Count);
            var errorCount = localizedErrors.Count;

            var moduleRows = new List<string>();
            foreach (var module in result.Modules)
            {
                foreach (var model in module.Mоdels)
                {
                    var dbStatus = model.DbLayerCheck ? $"✅ {emailTexts.OkText}" : $"❌ {emailTexts.ErrorText}";
                    var logicStatus = model.LogicLayerCheck ? $"✅ {emailTexts.OkText}" : $"❌ {emailTexts.ErrorText}";
                    var apiStatus = model.WebApi ? $"✅ {emailTexts.OkText}" : $"❌ {emailTexts.ErrorText}";

                    var overallStatus = model.DbLayerCheck && model.LogicLayerCheck && model.WebApi
                        ? $"<span class='status-badge status-success'>{emailTexts.HealthyStatusText}</span>"
                        : $"<span class='status-badge status-error'>{emailTexts.IssuesStatusText}</span>";

                    var localizedModuleName = LocalizationProvider.GetLocalizedValue(module.ModuleNames, languageKey, module.ModuleName);
                    var localizedModelName = LocalizationProvider.GetLocalizedValue(model.ModelNames, languageKey, model.ModelName);

                    moduleRows.Add($@"
                        <tr>
                            <td>{localizedModuleName}</td>
                            <td>{localizedModelName}</td>
                            <td>{dbStatus}</td>
                            <td>{logicStatus}</td>
                            <td>{apiStatus}</td>
                            <td>{overallStatus}</td>
                        </tr>");
                }
            }

            var errorsSection = "";
            if (errorCount > 0)
            {
                var errorItems = localizedErrors.Select(error =>
                    $"<div class='error-item'>{error}</div>").ToList();

                var errorsTitle = LocalizationProvider.GetErrorsTitle(errorCount, languageKey);

                errorsSection = $@"
                    <div class='errors-section'>
                        <h3>{errorsTitle}</h3>
                        {string.Join("", errorItems)}
                    </div>";
            }

            var html = template
                .Replace("{{TotalModules}}", totalModules.ToString())
                .Replace("{{HealthyModules}}", healthyModules.ToString())
                .Replace("{{TotalModels}}", totalModels.ToString())
                .Replace("{{ErrorCount}}", errorCount.ToString())
                .Replace("{{ModuleRows}}", string.Join("", moduleRows))
                .Replace("{{ErrorsSection}}", errorsSection)
                .Replace("{{Timestamp}}", result.DateTime.ToString("dd.MM.yyyy HH:mm:ss UTC"));

            return html;
        }

        private List<string> CollectLocalizedErrors(HealthCheckResult result, string languageKey)
        {
            return result.Modules
                .SelectMany(m => m.LocalizedErrors)
                .Select(err => ResolveLocalizedMessage(err, languageKey))
                .Where(msg => !string.IsNullOrWhiteSpace(msg))
                .ToList();
        }

        private string ResolveLocalizedMessage(LocalizationString message, string languageKey)
        {
            var fallback = LocalizationProvider.GetLocalizedValue(message, "en-US", string.Empty);
            if (string.IsNullOrWhiteSpace(fallback))
            {
                fallback = LocalizationProvider.GetLocalizedValue(message, "ru-RU", "Unknown");
            }

            return LocalizationProvider.GetLocalizedValue(message, languageKey, fallback);
        }

        private void AddLocalizedError(HealthCheckModuleModelResult result, LocalizationString message, string languageKey)
        {
            result.LocalizedErrors.Add(message);
            var localized = ResolveLocalizedMessage(message, languageKey);
            result.Errors.Add(localized);
        }
    }

    public class ModuleCheckItem
    {
        public string ModuleName { get; set; }
        public Type ModuleDbContextType { get; set; }
        public LocalizationString ModuleNames { get; set; } = new LocalizationString();
    }
}
