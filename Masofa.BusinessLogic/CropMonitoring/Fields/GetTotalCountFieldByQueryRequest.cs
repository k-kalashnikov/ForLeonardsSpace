using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;

namespace Masofa.BusinessLogic.CropMonitoring.Fields
{
    public class GetTotalCountFieldByQueryRequest : IRequest<int>
    {
        public BaseGetQuery<Field> Query { get; init; } = default!;
    }

    public class GetTotalCountFieldByQueryRequestHandler : IRequestHandler<GetTotalCountFieldByQueryRequest, int>
    {
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }

        public GetTotalCountFieldByQueryRequestHandler(MasofaCropMonitoringDbContext masofaCropMonitoringDbContext, MasofaDictionariesDbContext masofaDictionariesDbContext)
        {
            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
        }

        public async Task<int> Handle(GetTotalCountFieldByQueryRequest request, CancellationToken cancellationToken)
        {
            var seasonsQuery = MasofaCropMonitoringDbContext.Seasons.AsNoTracking();
            var fieldsQuery = MasofaCropMonitoringDbContext.Fields.AsNoTracking();

            bool isBySeasons = false;
            foreach (var item in request.Query.Filters)
            {
                if (item.FilterField == "CropId")
                {
                    seasonsQuery = seasonsQuery.ApplyFiltering(item);
                    isBySeasons = true;
                    continue;
                }
                if (item.FilterField == "VarietyId")
                {
                    seasonsQuery = seasonsQuery.ApplyFiltering(item);
                    isBySeasons = true;
                    continue;
                }
                if (item.FilterField == "PlantingDate")
                {
                    seasonsQuery = seasonsQuery.ApplyFiltering(item);
                    isBySeasons = true;
                    continue;
                }
                if (item.FilterField == "HarvestingDate")
                {
                    seasonsQuery = seasonsQuery.ApplyFiltering(item);
                    isBySeasons = true;
                    continue;
                }
                fieldsQuery = fieldsQuery.ApplyFiltering(item);
            }

            if (!string.IsNullOrEmpty(request.Query.SortBy))
            {

                fieldsQuery = fieldsQuery.ApplyOrdering(request.Query.SortBy, request.Query.Sort);
            }


            var seasons = new List<Season>();
            var fields = new List<Field>();
            if (!isBySeasons)
            {
                if (request.Query.Take.HasValue)
                {
                    fieldsQuery = fieldsQuery.Take(request.Query.Take.Value);
                }
                fields = await fieldsQuery.ToListAsync();
                seasons = await seasonsQuery
                    .Where(s => s.FieldId != null)
                    .Where(s => fields.Select(f => f.Id).Contains(s.FieldId.Value))
                    .ToListAsync();

            }
            else
            {
                seasons = await seasonsQuery.ToListAsync();
                var fieldIdsBySeasons = seasons.Select(s => s.FieldId).ToList();
                fieldsQuery = fieldsQuery.Where(f => fieldIdsBySeasons.Contains(f.Id));
                if (request.Query.Take.HasValue)
                {
                    fieldsQuery = fieldsQuery.Take(request.Query.Take.Value);
                }
                fields = await fieldsQuery.ToListAsync();
            }



            return await fieldsQuery.CountAsync();
        }
    }
}
