using Masofa.Common.Resources;
using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Common
{
    /// <summary>
    /// Запрос для получения списка всех бакетов
    /// </summary>
    [RequestPermission(ActionType = "Read")]
    public class GetFileStorageBucketsRequest : IRequest<List<string>>
    {
    }

    public class GetFileStorageBucketsRequestHandler : IRequestHandler<GetFileStorageBucketsRequest, List<string>>
    {
        private readonly MasofaCommonDbContext _dbContext;
        private readonly ILogger<GetFileStorageBucketsRequestHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public GetFileStorageBucketsRequestHandler(
            MasofaCommonDbContext dbContext,
            ILogger<GetFileStorageBucketsRequestHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<List<string>> Handle(GetFileStorageBucketsRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var buckets = await _dbContext.FileStorageItems
                    .AsNoTracking()
                    .Where(x => x.Status == StatusType.Active)
                    .Select(x => x.FileStorageBacket)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToListAsync(cancellationToken);

                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, buckets.ToString()), requestPath);
                return buckets;
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

    /// <summary>
    /// Запрос для получения файлов в бакете
    /// </summary>
    [RequestPermission(ActionType = "Read")]
    public class GetFileStorageFilesByBucketRequest : IRequest<List<FileStorageItem>>
    {
        /// <summary>
        /// Имя бакета
        /// </summary>
        public string Bucket { get; set; }

        /// <summary>
        /// Префикс для фильтрации (путь внутри бакета)
        /// </summary>
        public string? Prefix { get; set; }
    }

    public class GetFileStorageFilesByBucketRequestHandler : IRequestHandler<GetFileStorageFilesByBucketRequest, List<FileStorageItem>>
    {
        private readonly MasofaCommonDbContext _dbContext;
        private readonly ILogger<GetFileStorageFilesByBucketRequestHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public GetFileStorageFilesByBucketRequestHandler(
            MasofaCommonDbContext dbContext,
            ILogger<GetFileStorageFilesByBucketRequestHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<List<FileStorageItem>> Handle(GetFileStorageFilesByBucketRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync($"Request in {requestPath} for bucket: {request.Bucket}, prefix: {request.Prefix}", requestPath);

                var query = _dbContext.FileStorageItems
                    .AsNoTracking()
                    .Where(x => x.Status == StatusType.Active && x.FileStorageBacket == request.Bucket);

                if (!string.IsNullOrEmpty(request.Prefix))
                {
                    query = query.Where(x => x.FileStoragePath.StartsWith(request.Prefix));
                }

                var files = await query
                    .OrderBy(x => x.FileStoragePath)
                    .ToListAsync(cancellationToken);

                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, files.ToString()), requestPath);
                return files;
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

