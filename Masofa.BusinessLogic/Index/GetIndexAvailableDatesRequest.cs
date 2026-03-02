using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Masofa.BusinessLogic.Index
{
    [RequestPermission(ActionType = "Read")]
    public class GetIndexAvailableDatesRequest<TIndexSharedReport> : IRequest<List<DateOnly>>
        where TIndexSharedReport : IndexReportShared
    {
        [Required]
        public Guid CropId { get; set; }

        public Guid? RegionId { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }
    }

    public class GetIndexAvailableDatesRequestHandler<TIndexSharedReport> : IRequestHandler<GetIndexAvailableDatesRequest<TIndexSharedReport>, List<DateOnly>>
        where TIndexSharedReport : IndexReportShared
    {
        private readonly MasofaIndicesDbContext _context;
        private readonly ILogger<GetIndexAvailableDatesRequestHandler<TIndexSharedReport>> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public GetIndexAvailableDatesRequestHandler(
            MasofaIndicesDbContext context,
            ILogger<GetIndexAvailableDatesRequestHandler<TIndexSharedReport>> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _context = context;
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<List<DateOnly>> Handle(GetIndexAvailableDatesRequest<TIndexSharedReport> request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";

            try
            {
                await _businessLogicLogger.LogInformationAsync(
                    $"Start {requestPath}: CropId={request.CropId}, RegionId={request.RegionId}, StartDate={request.StartDate}, EndDate={request.EndDate}",
                    requestPath);

                var query = _context.Set<TIndexSharedReport>().AsNoTracking().AsQueryable();

                query = query.Where(report => report.CropId == request.CropId);

                if (request.RegionId.HasValue)
                {
                    query = query.Where(report => report.RegionId == request.RegionId.Value);
                }

                if (request.StartDate.HasValue)
                {
                    query = query.Where(report => report.DateOnly >= request.StartDate.Value);
                }

                if (request.EndDate.HasValue)
                {
                    query = query.Where(report => report.DateOnly <= request.EndDate.Value);
                }

                var dates = await query
                    .Select(report => report.DateOnly)
                    .Distinct()
                    .OrderByDescending(date => date)
                    .ToListAsync(cancellationToken);

                await _businessLogicLogger.LogInformationAsync(
                    $"Finished {requestPath}: Found {dates.Count} available dates",
                    requestPath);

                return dates;
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {requestPath}. {ex.Message}";
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                _logger.LogCritical(ex, msg);
                throw;
            }
        }
    }
}

