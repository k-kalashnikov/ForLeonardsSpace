using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Reports;
using Masofa.Common.Resources;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RazorLight;
using System.Text;

namespace Masofa.BusinessLogic.Reports
{
    public class BaseReportPrintCommand<TModel, TReportModel, TDbContext> : IRequest<byte[]>
        where TModel : BaseReportEntity<TReportModel>, new()
        where TReportModel : class, new()
        where TDbContext : DbContext
    {
        public required BaseReportPrintQuery<TModel> Query { get; set; }
    }

    public class BaseReportPrintQuery<TModel>
    {
        public Guid Id { get; set; }
        public string Locale { get; set; } = "ru-RU";
    }

    public class BaseReportPrintCommandHandler<TModel, TReportModel, TDbContext> : IRequestHandler<BaseReportPrintCommand<TModel, TReportModel, TDbContext>, byte[]>
        where TModel : BaseReportEntity<TReportModel>, new()
        where TReportModel : class, new()
        where TDbContext : DbContext
    {
        private TDbContext DbContext { get; set; }
        private ILogger Logger { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private RazorLightEngine RazorLightEngine { get; set; }

        public BaseReportPrintCommandHandler(TDbContext dbContext, ILogger<BaseReportPrintCommandHandler<TModel, TReportModel, TDbContext>> logger, IBusinessLogicLogger businessLogicLogger, RazorLightEngine razorLightEngine)
        {
            DbContext = dbContext;
            Logger = logger;
            BusinessLogicLogger = businessLogicLogger;
            RazorLightEngine = razorLightEngine;
        }

        public async Task<byte[]> Handle(BaseReportPrintCommand<TModel, TReportModel, TDbContext> request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var report = await DbContext.Set<TModel>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == request.Query.Id, cancellationToken);

                if (report == null)
                {
                    var errorMsg = LogMessageResource.EntityNotFound(typeof(TModel).FullName, request.Query.Id.ToString());
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    throw new Exception(errorMsg);
                }

                Console.WriteLine($"typeof(TModel).Name: {typeof(TModel).Name}");

                var localeForTemplate = request.Query.Locale.Split('-')[0];
                var templateName = $"{typeof(TModel).Name}.{localeForTemplate}.cshtml";

                string html = string.Empty;
                try
                {
                    html = await RazorLightEngine.CompileRenderAsync(templateName, report.ReportBody);
                    await BusinessLogicLogger.LogInformationAsync(LogMessageResource.ReportGenerated(request.Query.Locale, templateName), requestPath);
                }
                catch (Exception templateEx)
                {
                    var msg = $"Failed to compile or render template '{templateName}' for locale '{request.Query.Locale}'. Error: {templateEx.Message}";
                    await BusinessLogicLogger.LogWarningAsync(msg, requestPath);
                }

                return Encoding.UTF8.GetBytes(html);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                throw ex;
            }
        }
    }
}
