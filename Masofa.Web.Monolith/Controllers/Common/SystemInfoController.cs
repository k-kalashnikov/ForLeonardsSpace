using Masofa.Common.Resources;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using MediatR;
using Masofa.Web.Monolith.Controllers;
using Microsoft.Extensions.Logging;
using Masofa.Common.Services;
using Masofa.Common.Models.SystemMetadata;

namespace Masofa.Web.Monolith.Controllers.Common
{
    [Route("common/[controller]")]
    [ApiExplorerSettings(GroupName = "Common")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Application, Subscriber, Operator")]
    public class SystemInfoController : BaseController
    {
        private readonly MasofaCommonDbContext _dbContext;
        private readonly IBusinessLogicLogger _businessLogicLogger;
        private readonly IModuleLocalizationService _moduleLocalizationService;

        public SystemInfoController(
            ILogger<SystemInfoController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            MasofaCommonDbContext dbContext,
            IModuleLocalizationService moduleLocalizationService) : base(logger, configuration, mediator)
        {
            _dbContext = dbContext;
            _businessLogicLogger = businessLogicLogger;
            _moduleLocalizationService = moduleLocalizationService;
        }

        /// <summary>
        /// Получает список доступных модулей системы
        /// </summary>
        /// <returns>Список модулей</returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<List<string>> GetAvailableModules()
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetAvailableModules)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var modules = new List<string>();
                
                // Получаем модули из DbContext'ов через рефлексию
                var dbContexts = typeof(MasofaCommonDbContext).Assembly.GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(DbContext)) && t != typeof(DbContext))
                    .ToList();

                foreach (var dbContextType in dbContexts)
                {
                    var moduleName = dbContextType.Name
                        .Replace("Masofa", "")
                        .Replace("DbContext", "")
                        .Replace("Common", "Common")
                        .Replace("Identity", "Identity")
                        .Replace("Satellite", "Satellite")
                        .Replace("WeatherReport", "Weather Report")
                        .Replace("Dictionaries", "Dictionaries")
                        .Replace("Notifications", "Notifications");

                    if (!string.IsNullOrEmpty(moduleName) && !modules.Contains(moduleName))
                    {
                        modules.Add(moduleName);
                    }
                }
                modules.Sort();

                await _businessLogicLogger.LogInformationAsync($"Finish request in {requestPath} with result: {string.Join(", ", modules)}", requestPath);
                return modules;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return new List<string>();
            }
        }

        /// <summary>
        /// Получает список доступных модулей системы с локализованными названиями
        /// </summary>
        /// <param name="language">Язык для локализации (ru-RU, en-US, uz-Latn-UZ, uz-Cyrl-UZ, ar-LB). Если не указан, возвращаются все локализации</param>
        /// <returns>Список модулей с локализованными названиями</returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<List<LocalizedModuleInfo>> GetAvailableModulesLocalized([FromQuery] string? language = null)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetAvailableModulesLocalized)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync($"Start request in {requestPath} with language: {language}", requestPath);

                var modules = new List<string>();
                
                // Получаем модули из DbContext'ов через рефлексию
                var dbContexts = typeof(MasofaCommonDbContext).Assembly.GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(DbContext)) && t != typeof(DbContext))
                    .ToList();

                foreach (var dbContextType in dbContexts)
                {
                    var moduleName = dbContextType.Name
                        .Replace("Masofa", "")
                        .Replace("DbContext", "")
                        .Replace("Common", "Common")
                        .Replace("Identity", "Identity")
                        .Replace("CropMonitoring", "Crop Monitoring")
                        .Replace("Satellite", "Satellite")
                        .Replace("WeatherReport", "Weather Report")
                        .Replace("Dictionaries", "Dictionaries")
                        .Replace("Notifications", "Notifications");

                    if (!string.IsNullOrEmpty(moduleName) && !modules.Contains(moduleName))
                    {
                        modules.Add(moduleName);
                    }
                }
                modules.Sort();

                // Преобразуем в локализованные модули
                var localizedModules = modules.Select(moduleName => new LocalizedModuleInfo
                {
                    ModuleName = moduleName,
                    LocalizedNames = _moduleLocalizationService.GetModuleLocalization(moduleName)
                }).ToList();

                await _businessLogicLogger.LogInformationAsync($"Finish request in {requestPath} with {localizedModules.Count} modules", requestPath);
                return localizedModules;
            }
            catch (Exception ex)
            {
                var msg = $"Error in {requestPath}: {ex.Message}";
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return new List<LocalizedModuleInfo>();
            }
        }
    }
}
