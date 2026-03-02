using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.BusinessLogic.Uav.Queries;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Uav.Handlers
{
    public class GetUavImageQueryHandler : IRequestHandler<GetUavImageQuery, UavImageFileDto>
    {
        private readonly MasofaCommonDbContext _commonDbContext;
        private readonly IFileStorageProvider _fileStorageProvider;
        private readonly IBusinessLogicLogger _businessLogicLogger;
        private readonly ILogger<GetUavImageQueryHandler> _logger;

        public GetUavImageQueryHandler(
            MasofaCommonDbContext commonDbContext,
            IFileStorageProvider fileStorageProvider,
            IBusinessLogicLogger businessLogicLogger,
            ILogger<GetUavImageQueryHandler> logger)
        {
            _commonDbContext = commonDbContext;
            _fileStorageProvider = fileStorageProvider;
            _businessLogicLogger = businessLogicLogger;
            _logger = logger;
        }

        public async Task<UavImageFileDto> Handle(GetUavImageQuery request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                var fileStorageItem = await _commonDbContext.FileStorageItems
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Id == request.PhotoId, cancellationToken);
                if (fileStorageItem == null)
                    throw new KeyNotFoundException("FileStorageItem not found.");
                var sourceStream = await _fileStorageProvider.GetFileStreamAsync(fileStorageItem);
                if (sourceStream == null)
                    throw new FileNotFoundException("File stream is null.");
                if (sourceStream.CanSeek) sourceStream.Position = 0;
                var contentType = fileStorageItem.FileContentTypeName ?? "application/octet-stream";
                var fileName = fileStorageItem.FileStoragePath;
                return new UavImageFileDto
                {
                    FileStream = sourceStream,
                    ContentType = contentType,
                    FileName = fileName
                };
            }
            catch (Exception ex)
            {
                var msg = $"Error in GetUavImageQueryHandler: {ex.Message}";
                _logger.LogError(ex, msg);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                throw;
            }
        }
    }
}