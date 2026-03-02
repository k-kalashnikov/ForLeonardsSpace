using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Satellite
{
    /// <summary>
    /// Запрос для добавления тега к спутниковому продукту
    /// </summary>
    [RequestPermission(ActionType = "Create")]
    public class AddTagToSatelliteProductRequest : IRequest<bool>
    {
        /// <summary>
        /// ID спутникового продукта
        /// </summary>
        public Guid SatelliteProductId { get; set; }

        /// <summary>
        /// Название тега
        /// </summary>
        public string TagName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Запрос для удаления тега у спутникового продукта
    /// </summary>
    [RequestPermission(ActionType = "Delete")]
    public class RemoveTagFromSatelliteProductRequest : IRequest<bool>
    {
        /// <summary>
        /// ID спутникового продукта
        /// </summary>
        public Guid SatelliteProductId { get; set; }

        /// <summary>
        /// ID тега
        /// </summary>
        public Guid TagId { get; set; }
    }

    /// <summary>
    /// Запрос для получения всех тегов из БД (для фильтра)
    /// </summary>
    [RequestPermission(ActionType = "Read")]
    public class GetAllTagsRequest : IRequest<List<TagDto>>
    {
        /// <summary>
        /// Поисковый запрос (опционально)
        /// </summary>
        public string? SearchQuery { get; set; }
    }

    /// <summary>
    /// DTO для тега
    /// </summary>
    public class TagDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    /// <summary>
    /// Handler для AddTagToSatelliteProductRequest
    /// </summary>
    public class AddTagToSatelliteProductRequestHandler : IRequestHandler<AddTagToSatelliteProductRequest, bool>
    {
        private readonly MasofaCommonDbContext _dbCommonContext;
        private readonly MasofaDictionariesDbContext _dbDictContext;
        private readonly ILogger<AddTagToSatelliteProductRequestHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public AddTagToSatelliteProductRequestHandler(
            MasofaCommonDbContext dbCommonContext,
            MasofaDictionariesDbContext dbDictContext,
            ILogger<AddTagToSatelliteProductRequestHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _dbCommonContext = dbCommonContext;
            _dbDictContext = dbDictContext;
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<bool> Handle(AddTagToSatelliteProductRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                // Проверяем существование спутникового продукта
                var product = await _dbCommonContext.Set<SatelliteProduct>()
                    .AsNoTracking()
                    .Where(p => p.Id == request.SatelliteProductId && p.Status == StatusType.Active)
                    .FirstOrDefaultAsync(cancellationToken);

                if (product == null)
                {
                    await _businessLogicLogger.LogErrorAsync(LogMessageResource.SatelliteProductNotFound(request.SatelliteProductId.ToString()), requestPath);
                    return false;
                }

                // Ищем существующий тег или создаем новый
                var tag = await _dbDictContext.Set<Tag>()
                    .Where(t => t.Name == request.TagName && t.Status == StatusType.Active)
                    .FirstOrDefaultAsync(cancellationToken);

                if (tag == null)
                {
                    // Создаем новый тег
                    tag = new Tag
                    {
                        Id = Guid.NewGuid(),
                        Name = request.TagName,
                        Status = StatusType.Active,
                        CreateAt = DateTime.UtcNow
                    };

                    _dbDictContext.Set<Tag>().Add(tag);
                    await _dbDictContext.SaveChangesAsync(cancellationToken);
                }

                // Проверяем, не существует ли уже связь
                var existingRelation = await _dbCommonContext.Set<TagRelation>()
                    .Where(tr => tr.OwnerId == request.SatelliteProductId && 
                                tr.TagId == tag.Id &&
                                tr.OwnerTypeFullName == typeof(SatelliteProduct).FullName)
                    .FirstOrDefaultAsync(cancellationToken);

                if (existingRelation == null)
                {
                    // Создаем связь
                    var tagRelation = new TagRelation
                    {
                        Id = Guid.NewGuid(),
                        OwnerId = request.SatelliteProductId,
                        OwnerTypeFullName = typeof(SatelliteProduct).FullName!,
                        TagId = tag.Id,
                        Status = StatusType.Active,
                        CreateAt = DateTime.UtcNow
                    };

                    _dbCommonContext.Set<TagRelation>().Add(tagRelation);
                    await _dbCommonContext.SaveChangesAsync(cancellationToken);
                }

                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, true.ToString()), requestPath);
                return true;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                _logger.LogCritical(ex, msg);
                return false;
            }
        }
    }

    /// <summary>
    /// Handler для RemoveTagFromSatelliteProductRequest
    /// </summary>
    public class RemoveTagFromSatelliteProductRequestHandler : IRequestHandler<RemoveTagFromSatelliteProductRequest, bool>
    {
        private readonly MasofaCommonDbContext _dbCommonContext;
        private readonly ILogger<RemoveTagFromSatelliteProductRequestHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public RemoveTagFromSatelliteProductRequestHandler(
            MasofaCommonDbContext dbCommonContext,
            ILogger<RemoveTagFromSatelliteProductRequestHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _dbCommonContext = dbCommonContext;
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<bool> Handle(RemoveTagFromSatelliteProductRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                // Находим и удаляем связь
                var tagRelation = await _dbCommonContext.Set<TagRelation>()
                    .Where(tr => tr.OwnerId == request.SatelliteProductId && 
                                tr.TagId == request.TagId &&
                                tr.OwnerTypeFullName == typeof(SatelliteProduct).FullName)
                    .FirstOrDefaultAsync(cancellationToken);

                if (tagRelation != null)
                {
                    _dbCommonContext.Set<TagRelation>().Remove(tagRelation);
                    await _dbCommonContext.SaveChangesAsync(cancellationToken);
                    
                    await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, true.ToString()), requestPath);
                    return true;
                }

                await _businessLogicLogger.LogWarningAsync(LogMessageResource.TagRelationNotFound(request.SatelliteProductId.ToString(), request.TagId.ToString()), requestPath);
                return false;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                _logger.LogCritical(ex, msg);
                return false;
            }
        }
    }

    /// <summary>
    /// Handler для GetAllTagsRequest
    /// </summary>
    public class GetAllTagsRequestHandler : IRequestHandler<GetAllTagsRequest, List<TagDto>>
    {
        private readonly MasofaDictionariesDbContext _dbDictContext;
        private readonly ILogger<GetAllTagsRequestHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public GetAllTagsRequestHandler(
            MasofaDictionariesDbContext dbDictContext,
            ILogger<GetAllTagsRequestHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _dbDictContext = dbDictContext;
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<List<TagDto>> Handle(GetAllTagsRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var query = _dbDictContext.Set<Tag>()
                    .AsNoTracking()
                    .Where(t => t.Status == StatusType.Active);

                // Добавляем поиск по названию если указан поисковый запрос
                if (!string.IsNullOrWhiteSpace(request.SearchQuery))
                {
                    var searchTerm = request.SearchQuery.Trim().ToLower();
                    query = query.Where(t => t.Name.ToLower().Contains(searchTerm));
                }

                var tags = await query
                    .OrderBy(t => t.Name)
                    .Select(t => new TagDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Description = t.Description
                    })
                    .ToListAsync(cancellationToken);

                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, tags.Count.ToString()), requestPath);
                return tags;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                _logger.LogCritical(ex, msg);
                return new List<TagDto>();
            }
        }
    }
}
