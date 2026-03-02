using Masofa.Common.Resources;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Common
{
    /// <summary>
    /// Запрос на получение информации о файле
    /// </summary>
    public class GetFileInfoRequest : IRequest<FileStorageItem>
    {
        /// <summary>
        /// Идентификатор файла
        /// </summary>
        public Guid Id { get; set; }
    }

    /// <summary>
    /// Обработчик запроса на получение информации о файле
    /// </summary>
    public class GetFileInfoRequestHandler : IRequestHandler<GetFileInfoRequest, FileStorageItem>
    {
        private readonly MasofaCommonDbContext _commonDbContext;
        private readonly ILogger<GetFileInfoRequestHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public GetFileInfoRequestHandler(
            MasofaCommonDbContext commonDbContext,
            ILogger<GetFileInfoRequestHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _commonDbContext = commonDbContext;
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<FileStorageItem> Handle(GetFileInfoRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync($"Getting file info for: {request.Id}", requestPath);

                var fileStorageItem = await _commonDbContext.FileStorageItems
                    .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

                if (fileStorageItem == null)
                {
                    throw new KeyNotFoundException($"File with id {request.Id} not found");
                }

                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, fileStorageItem.OwnerTypeFullName.ToString()), requestPath);
                return fileStorageItem;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                _logger.LogCritical(ex, msg);
                throw;
            }
        }
    }
}

