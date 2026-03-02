using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.BusinessLogic.Index
{
    /// <summary>
    /// Базовая команда для получения экземпляра отчёта по Id, которая наследуются от <see cref="../Masofa.Common/Models/Satellite/Indices/BaseIndexReport.cs">Masofa.Common.Models.Satellite.Indices.BaseIndexReport</see>
    /// </summary>
    /// <typeparam name="TModel">Тип модели, которую удалили</typeparam>
    [RequestPermission(ActionType = "Read")]
    public class BaseGetIndexByIdRequest<TModel> : IRequest<TModel>
        where TModel : BaseIndexReport
    {
        /// <summary>
        /// Id сущности
        /// </summary>
        public Guid Id { get; set; }
        public double PosibleMinValue { get; set; }
        public double PosibleMaxValue { get; set; }
    }

    public class BaseGetIndexByIdRequestHandler<TModel> : IRequestHandler<BaseGetIndexByIdRequest<TModel>, TModel>
    where TModel : BaseIndexReport
    {
        private MasofaIndicesDbContext MasofaIndicesDbContext { get; set; }
        private ILogger Logger { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public BaseGetIndexByIdRequestHandler(MasofaIndicesDbContext masofaIndicesDbContext, ILogger<BaseGetIndexByIdRequestHandler<TModel>> logger, IBusinessLogicLogger businessLogicLogger)
        {
            MasofaIndicesDbContext = masofaIndicesDbContext;
            Logger = logger;
            BusinessLogicLogger = businessLogicLogger;
        }

        public async Task<TModel> Handle(BaseGetIndexByIdRequest<TModel> request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var report =  await MasofaIndicesDbContext.Set<TModel>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id.Equals(request.Id));

                report.PosibleMaxValue = request.PosibleMaxValue;
                report.PosibleMinValue = request.PosibleMinValue;

                return report;
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
