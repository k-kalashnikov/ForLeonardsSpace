using Masofa.Common.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Models.Identity;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.FieldPhotoRequest
{
    public class GetFieldPhotoMetadataRequest : IRequest<FieldPhotoMetadataResponse>
    {
    }

    public class FieldPhotoMetadataResponse
    {
        public IReadOnlyList<RegionTreeNode> Regions { get; init; } = Array.Empty<RegionTreeNode>();
        public IReadOnlyList<TagShortDto> Tags { get; init; } = Array.Empty<TagShortDto>();
        public IReadOnlyList<FieldSummaryDto> Fields { get; init; } = Array.Empty<FieldSummaryDto>();
        public IReadOnlyList<AuthorShortDto> Authors { get; init; } = Array.Empty<AuthorShortDto>();
        public DateTime? MinCaptureDateUtc { get; init; }
        public DateTime? MaxCaptureDateUtc { get; init; }
        public DateTime? MinUploadDateUtc { get; init; }
        public DateTime? MaxUploadDateUtc { get; init; }
    }

    public class RegionTreeNode
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
        public IReadOnlyList<RegionTreeNode> Children { get; set; } = Array.Empty<RegionTreeNode>();
    }

    public class FieldSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? RegionId { get; set; }
    }

    public class AuthorShortDto
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string? Email { get; set; }
    }

    public class GetFieldPhotoMetadataRequestHandler : IRequestHandler<GetFieldPhotoMetadataRequest, FieldPhotoMetadataResponse>
    {
        private readonly MasofaCropMonitoringDbContext _cropMonitoringDbContext;
        private readonly MasofaCommonDbContext _commonDbContext;
        private readonly MasofaDictionariesDbContext _dictionariesDbContext;
        private readonly MasofaIdentityDbContext _identityDbContext;
        private readonly IBusinessLogicLogger _businessLogicLogger;
        private readonly ILogger<GetFieldPhotoMetadataRequestHandler> _logger;

        public GetFieldPhotoMetadataRequestHandler(
            MasofaCropMonitoringDbContext cropMonitoringDbContext,
            MasofaCommonDbContext commonDbContext,
            MasofaDictionariesDbContext dictionariesDbContext,
            MasofaIdentityDbContext identityDbContext,
            IBusinessLogicLogger businessLogicLogger,
            ILogger<GetFieldPhotoMetadataRequestHandler> logger)
        {
            _cropMonitoringDbContext = cropMonitoringDbContext;
            _commonDbContext = commonDbContext;
            _dictionariesDbContext = dictionariesDbContext;
            _identityDbContext = identityDbContext;
            _businessLogicLogger = businessLogicLogger;
            _logger = logger;
        }

        public async Task<FieldPhotoMetadataResponse> Handle(GetFieldPhotoMetadataRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";

            try
            {
                await _businessLogicLogger.LogInformationAsync("LogMessageResource.RequestStarted(requestPath)", requestPath);

                var photosQuery = _cropMonitoringDbContext.FieldPhotos
                    .AsNoTracking()
                    .Where(p => p.Status == StatusType.Active);

                var photos = await photosQuery.ToListAsync(cancellationToken);

                var minCaptureDate = photos.Where(p => p.CaptureDateUtc.HasValue).Select(p => p.CaptureDateUtc!.Value).DefaultIfEmpty().Min();
                var maxCaptureDate = photos.Where(p => p.CaptureDateUtc.HasValue).Select(p => p.CaptureDateUtc!.Value).DefaultIfEmpty().Max();
                var minUploadDate = photos.Select(p => p.CreateAt).DefaultIfEmpty().Min();
                var maxUploadDate = photos.Select(p => p.CreateAt).DefaultIfEmpty().Max();

                var fieldIds = photos.Where(p => p.FieldId.HasValue).Select(p => p.FieldId!.Value).Distinct().ToList();
                var authorIds = photos.Select(p => p.CreateUser).Distinct().Where(id => id != Guid.Empty).ToList();

                var fields = fieldIds.Count == 0
                    ? new List<Field>()
                    : await _cropMonitoringDbContext.Fields
                        .AsNoTracking()
                        .Where(f => fieldIds.Contains(f.Id))
                        .ToListAsync(cancellationToken);

                var fieldSummaries = fields
                    .Select(f => new FieldSummaryDto
                    {
                        Id = f.Id,
                        Name = string.IsNullOrWhiteSpace(f.Name) ? f.Id.ToString() : f.Name,
                        RegionId = f.RegionId
                    })
                    .OrderBy(f => f.Name)
                    .ToList();

                var tags = await _dictionariesDbContext.Tags
                    .AsNoTracking()
                    .Where(t => t.Status == StatusType.Active)
                    .OrderBy(t => t.Name)
                    .Select(t => new TagShortDto
                    {
                        Id = t.Id,
                        Name = string.IsNullOrWhiteSpace(t.Name) ? t.Id.ToString() : t.Name
                    })
                    .ToListAsync(cancellationToken);

                var regionEntities = await _dictionariesDbContext.Regions
                    .AsNoTracking()
                    .Where(r => r.Status == StatusType.Active)
                    .ToListAsync(cancellationToken);

                var regionTreeInputs = regionEntities
                    .Select(r => new RegionNodeInput
                    {
                        Id = r.Id,
                        ParentId = r.ParentId,
                        Name = ResolveRegionName(r)
                    })
                    .ToList();

                var regionTree = BuildRegionTree(regionTreeInputs);

                var authors = authorIds.Count == 0
                    ? new List<AuthorShortDto>()
                    : (await _identityDbContext.Users
                        .AsNoTracking()
                        .Where(u => authorIds.Contains(u.Id))
                        .ToListAsync(cancellationToken))
                        .Select(u => new AuthorShortDto
                        {
                            Id = u.Id,
                            DisplayName = ResolveUserDisplayName(u),
                            Email = u.Email
                        })
                        .OrderBy(u => u.DisplayName)
                        .ToList();

                var response = new FieldPhotoMetadataResponse
                {
                    Regions = regionTree,
                    Tags = tags,
                    Fields = fieldSummaries,
                    Authors = authors,
                    MinCaptureDateUtc = NormalizeDate(minCaptureDate),
                    MaxCaptureDateUtc = NormalizeDate(maxCaptureDate),
                    MinUploadDateUtc = NormalizeDate(minUploadDate),
                    MaxUploadDateUtc = NormalizeDate(maxUploadDate)
                };

                await _businessLogicLogger.LogInformationAsync("LogMessageResource.RequestFinishedWithResult(requestPath,result)", requestPath);

                return response;
            }
            catch (Exception ex)
            {
                var message = $"Error in GetFieldPhotoMetadataRequest: {ex.Message}";
                await _businessLogicLogger.LogCriticalAsync(message, requestPath);
                _logger.LogCritical(ex, message);
                throw;
            }
        }

        private static IReadOnlyList<RegionTreeNode> BuildRegionTree(IEnumerable<RegionNodeInput> regions)
        {
            var nodes = new Dictionary<Guid, RegionTreeNode>();
            foreach (var region in regions)
            {
                nodes[region.Id] = new RegionTreeNode
                {
                    Id = region.Id,
                    Name = region.Name,
                    ParentId = region.ParentId
                };
            }

            foreach (var node in nodes.Values)
            {
                if (node.ParentId.HasValue && nodes.TryGetValue(node.ParentId.Value, out var parent))
                {
                    var children = parent.Children.ToList();
                    children.Add(node);
                    parent.Children = children
                        .OrderBy(child => child.Name)
                        .ToList();
                }
            }

            return nodes.Values
                .Where(n => !n.ParentId.HasValue)
                .OrderBy(n => n.Name)
                .ToList();
        }

        private class RegionNodeInput
        {
            public Guid Id { get; set; }
            public Guid? ParentId { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        private static string ResolveRegionName(Region region)
        {
            var priority = new[] { "ru-RU", "uz-Latn-UZ", "en-US" };
            foreach (var culture in priority)
            {
                var value = region.Names[culture];
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return region.Names.ValuesJson;
        }

        private static string ResolveUserDisplayName(User user)
        {
            var parts = new[]
            {
                user.LastName,
                user.FirstName,
                user.SecondName
            }.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

            if (parts.Length > 0)
            {
                return string.Join(" ", parts);
            }

            if (!string.IsNullOrWhiteSpace(user.UserName))
            {
                return user.UserName;
            }

            return user.Email ?? user.Id.ToString();
        }

        private static DateTime? NormalizeDate(DateTime dateTime)
        {
            if (dateTime == default)
            {
                return null;
            }

            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }
    }
}

