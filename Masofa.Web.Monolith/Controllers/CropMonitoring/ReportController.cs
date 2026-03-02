using Masofa.BusinessLogic.AnaliticReport;
using Masofa.BusinessLogic.Index;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.MasofaAnaliticReport;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Common;

namespace Masofa.Web.Monolith.Controllers.CropMonitoring
{
    /// <summary>
    /// Provides endpoints for managing and generating reports related to crop monitoring.
    /// </summary>
    /// <remarks>This controller is part of the Crop Monitoring API group and requires authentication using
    /// the JWT Bearer scheme.  Access is restricted to users with specific roles, including Admin, SystemAdmin,
    /// ModuleAdmin, Operator, Foreman, and FieldWorker.</remarks>
    [Route("cropMonitoring/[controller]")]
    [ApiExplorerSettings(GroupName = "CropMonitoring")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator, Foreman, FieldWorker")]
    public class ReportController : BaseCrudController<FarmerRecomendationReport, MasofaAnaliticReportDbContext>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ReportController> _log;

        public ReportController(IFileStorageProvider fileStorageProvider, MasofaAnaliticReportDbContext dbContext, ILogger<ReportController> logger, IConfiguration configuration, IMediator mediator, IBusinessLogicLogger businessLogicLogger, IHttpContextAccessor httpContextAccessor) : base(fileStorageProvider, dbContext, logger, configuration, mediator, businessLogicLogger, httpContextAccessor)
        {
            _mediator = mediator;
            _log = logger;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> BuildFarmerReport([FromBody] StartQwenAnalysisCommand req, CancellationToken ct)
        {
            if (req.FieldId == Guid.Empty)
                return BadRequest("fieldId is required");

            var cmd = new StartQwenAnalysisCommand
            {
                FieldId = req.FieldId,
                SeasonId = req.SeasonId,
                ReportDate = req.ReportDate == default ? DateOnly.FromDateTime(DateTime.UtcNow) : req.ReportDate,
                Locale = string.IsNullOrWhiteSpace(req.Locale) ? "ru-RU" : req.Locale!,
                AlsoPdf = req.AlsoPdf
            };

            var res = await _mediator.Send(cmd, ct);

            return Ok(new
            {
                fieldId = cmd.FieldId,
                date = cmd.ReportDate
            });
        }

        /// <summary>
        /// Получает файл отчета с учетом локализации
        /// </summary>
        /// <param name="id">Идентификатор отчета (FarmerRecomendationReport.Id)</param>
        /// <param name="language">Язык файла. Если не указан, берется из заголовков или дефолтный.</param>
        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<ActionResult> GetFile(Guid id, [FromQuery] string? language = null)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetFile)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var report = await DbContext.Set<FarmerRecomendationReport>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (report == null)
                    return NotFound("Report not found");

                if (string.IsNullOrWhiteSpace(language))
                {
                    var acceptLanguage = Request.Headers["Accept-Language"].FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(acceptLanguage))
                    {
                        var languages = acceptLanguage.Split(',')
                            .Select(l => l.Split(';')[0].Trim())
                            .ToList();

                        var supportedLanguages = new[] { "ru-RU", "en-US", "uz-Latn-UZ", "uz-Cyrl-UZ", "ar-LB" };

                        foreach (var lang in languages)
                        {
                            var matchedLang = supportedLanguages.FirstOrDefault(sl =>
                                string.Equals(sl, lang, StringComparison.OrdinalIgnoreCase) ||
                                sl.StartsWith(lang, StringComparison.OrdinalIgnoreCase) ||
                                lang.StartsWith(sl, StringComparison.OrdinalIgnoreCase));

                            if (matchedLang != null)
                            {
                                language = matchedLang;
                                break;
                            }
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(language)) language = "ru-RU";

                Guid? fileStorageId = null;

                if (!string.IsNullOrWhiteSpace(report.LocalizationFile))
                {
                    try
                    {
                        var fileStorageIds = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Guid?>>(report.LocalizationFile);

                        if (fileStorageIds != null)
                        {
                            if (!string.IsNullOrWhiteSpace(language) && fileStorageIds.TryGetValue(language, out var fId) && fId.HasValue)
                            {
                                fileStorageId = fId;
                            }

                            if (!fileStorageId.HasValue)
                            {
                                var supportedLanguages = new[] { "uz-Latn-UZ", "en-US", "ru-RU", "uz-Cyrl-UZ", "ar-LB" };
                                foreach (var lang in supportedLanguages)
                                {
                                    if (fileStorageIds.TryGetValue(lang, out var fIdVal) && fIdVal.HasValue)
                                    {
                                        fileStorageId = fIdVal;
                                        language = lang;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWarning(ex, "Failed to deserialize LocalizationFile for report {ReportId}", id);
                    }
                }

                if (!fileStorageId.HasValue)
                {
                    fileStorageId = report.FileStorageItemId;
                }

                if (!fileStorageId.HasValue || fileStorageId == Guid.Empty)
                {
                    return NotFound("Report file not found");
                }

                var request = new GetFileInfoRequest { Id = fileStorageId.Value };
                var fileStorageItem = await Mediator.Send(request);

                if (fileStorageItem == null)
                {
                    return NotFound("File storage item not found");
                }

                var fileBytes = await FileStorageProvider.GetFileBytesAsync(fileStorageItem);

                var contentType = GetContentType(fileStorageItem.FileStoragePath);
                var fileName = ExtractFileName(fileStorageItem.FileStoragePath) ?? $"report_{id}.html";

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, "success"), requestPath);

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private static string? ExtractFileName(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return null;
            var parts = filePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 ? parts[^1] : null;
        }

        private static string GetContentType(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return "application/octet-stream";
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".html" => "text/html",
                ".txt" => "text/plain",
                ".csv" => "text/csv",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };
        }
    }
}