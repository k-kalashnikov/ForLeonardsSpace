using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Resources;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reactive.Linq;

namespace Masofa.BusinessLogic.Dictionaries
{
    /// <summary>
    /// Пользовательский запрос для Firm
    /// </summary>
    public class FirmCustomQueryRequest : IRequest<List<Masofa.Common.Models.Dictionaries.Firm>>
    {
        /// <summary>
        /// Виды деятельности
        /// </summary>
        public Guid? BusinessTypeId { get; set; }

        /// <summary>
        /// Налоговый номер ЮЛ (ИНН)
        /// </summary>
        public string? Inn { get; set; }

        /// <summary>
        /// Регистрационный номер ЮЛ (ЕГРПО)
        /// </summary>
        public string? Egrpo { get; set; }

        /// <summary>
        /// Регион
        /// </summary>
        public Guid? RegionId { get; set; }
    }

    public class FirmCustomQueryRequestHandler : IRequestHandler<FirmCustomQueryRequest, List<Masofa.Common.Models.Dictionaries.Firm>>
    {
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private ILogger<FirmCustomQueryRequestHandler> Logger { get; set; }

        private MasofaDictionariesDbContext DictionariesDbContext { get; set; }

        public FirmCustomQueryRequestHandler(IBusinessLogicLogger businessLogicLogger, ILogger<FirmCustomQueryRequestHandler> logger, MasofaDictionariesDbContext dictionariesDbContext)
        {
            BusinessLogicLogger = businessLogicLogger;
            Logger = logger;
            DictionariesDbContext = dictionariesDbContext;
        }

        public async Task<List<Masofa.Common.Models.Dictionaries.Firm>> Handle(FirmCustomQueryRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                IQueryable<Masofa.Common.Models.Dictionaries.Firm> firmsQuery = DictionariesDbContext.Firms.AsNoTracking();
                firmsQuery = firmsQuery.Where(f => f.Status == Masofa.Common.Models.StatusType.Active);

                if (request.BusinessTypeId != null)
                {
                    var businessTypesFirms = await DictionariesDbContext.BusinessTypesFirm
                        .Where(btf => btf.BusinessTypeId == request.BusinessTypeId)
                        .Select(btf => btf.FirmId)
                        .ToListAsync(cancellationToken);

                    firmsQuery = firmsQuery.Where(f => businessTypesFirms.Contains(f.Id));
                }

                if (request.Inn != null)
                {
                    firmsQuery = firmsQuery.Where(f => f.Inn != null && f.Inn == request.Inn);
                }

                if (request.Egrpo != null)
                {
                    firmsQuery = firmsQuery.Where(f => f.Egrpo != null && f.Egrpo == request.Egrpo);
                }

                if (request.RegionId != null)
                {
                    firmsQuery = firmsQuery.Where(f => f.MainRegionId != null && f.MainRegionId == request.RegionId);
                }

                var result = await firmsQuery.ToListAsync(cancellationToken);

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.Count.ToString()), requestPath);

                return result;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                throw;
            }
        }
    }
}