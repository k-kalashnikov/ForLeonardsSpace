using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Masofa.BusinessLogic.CropMonitoring.Fields
{
    public class GetFieldByQueryRequest : IRequest<List<CustomFieldViewModel>>
    {
        public BaseGetQuery<Field> Query { get; init; } = default!;
    }

    public class GetFieldByQueryRequestHandler : IRequestHandler<GetFieldByQueryRequest, List<CustomFieldViewModel>>
    {
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }

        public GetFieldByQueryRequestHandler(MasofaCropMonitoringDbContext masofaCropMonitoringDbContext, MasofaDictionariesDbContext masofaDictionariesDbContext)
        {
            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
        }

        public async Task<List<CustomFieldViewModel>> Handle(GetFieldByQueryRequest request, CancellationToken cancellationToken)
        {
            var seasonsQuery = MasofaCropMonitoringDbContext.Seasons.AsNoTracking()
                .Where(s => s.Status == StatusType.Active);
            var fieldsQuery = MasofaCropMonitoringDbContext.Fields.AsNoTracking()
                .Where(s => s.Status == StatusType.Active);

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




            var regions = MasofaDictionariesDbContext.Regions.AsNoTracking()
                .Where(r => fields.Select(f => f.RegionId).Contains(r.Id))
                .ToList();
            var crops = MasofaDictionariesDbContext.Crops.AsNoTracking()
                .Where(c => seasons.Select(s => s.CropId).Contains(c.Id))
                .ToList();
            var varieties = MasofaDictionariesDbContext.Varieties.AsNoTracking()
                .Where(v => seasons.Select(s => s.VarietyId).Contains(v.Id))
                .ToList();
            var firms = MasofaDictionariesDbContext.Firms.AsNoTracking()
                .Where(fm => fields.Select(f => f.AgricultureProducerId).Contains(fm.Id))
                .ToList();
            var persons = MasofaDictionariesDbContext.Persons.AsNoTracking()
                .Where(p => fields.Select(f => f.AgricultureProducerId).Contains(p.Id))
                .ToList();

            var bids = MasofaCropMonitoringDbContext.Bids.AsNoTracking()
                .Where(b => b.FieldId != null)
                .Where(b => fields.Select(f => f.Id).Contains(b.FieldId.Value))
                .Where(m => m.EndDate != null)
                .OrderByDescending(m => m.EndDate)
                .ToList();

            var result = new List<CustomFieldViewModel>();


            foreach (var field in fieldsQuery)
            {
                var tempSeasons = seasons.Where(s => s.FieldId == field.Id).ToList();
                var tempBid = bids.Where(b => b.FieldId == field.Id).FirstOrDefault();

                var latestPlannedSeason = tempSeasons
                    .Where(s => s.FieldId == field.Id)
                    .Where(s => s.PlantingDatePlan.HasValue)
                    .OrderByDescending(s => s.PlantingDatePlan)
                    .FirstOrDefault();

                var latestActualSeason = tempSeasons
                    .Where(s => s.FieldId == field.Id)
                    .Where(s => s.PlantingDate.HasValue)
                    .OrderByDescending(s => s.PlantingDate)
                    .FirstOrDefault();

                var fieldCrops = crops.Where(c => tempSeasons.Select(s => s.CropId).Contains(c.Id)).ToList();

                var cropSquare = tempSeasons
                    .Where(s => s.CropId is not null &&
                                s.FieldArea is not null &&
                                fieldCrops.Select(c => c.Id).ToList().Contains(s.CropId.Value))
                    .GroupBy(s => s.CropId.Value)
                    .ToDictionary(g => g.Key, g => g.Sum(s => s.FieldArea.Value));

                string? plantingPeriodPlan = null;
                if (latestPlannedSeason?.PlantingDatePlan.HasValue == true && latestPlannedSeason?.HarvestingDatePlan.HasValue == true)
                {
                    plantingPeriodPlan = $"{latestPlannedSeason.PlantingDatePlan.Value:dd.MM.yyyy} - {latestPlannedSeason.HarvestingDatePlan.Value:dd.MM.yyyy}";
                }

                string? plantingPeriodActual = null;
                if (latestActualSeason?.PlantingDate.HasValue == true && latestActualSeason?.HarvestingDate.HasValue == true)
                {
                    plantingPeriodActual = $"{latestActualSeason.PlantingDate.Value:dd.MM.yyyy} - {latestActualSeason.HarvestingDate.Value:dd.MM.yyyy}";
                }

                var temp = new CustomFieldViewModel()
                {
                    Field = field,
                    AgricultureProducerName = ((firms.FirstOrDefault(m => m.Id == field.AgricultureProducerId) == null) ? persons.FirstOrDefault(m => m.Id == field.AgricultureProducerId)?.FullName : firms.FirstOrDefault(m => m.Id == field.AgricultureProducerId)?.FullName) ?? string.Empty,
                    Crops = fieldCrops,
                    Varieties = varieties.Where(c => tempSeasons.Select(s => s.VarietyId).Contains(c.Id)).ToList(),
                    LastBidDate = ((tempBid == null) || (!tempBid.EndDate.HasValue)) ? null : DateOnly.FromDateTime(tempBid.EndDate.Value),
                    Region = regions.FirstOrDefault(m => m.Id == field.RegionId),
                    PlantingDatePlan = latestPlannedSeason?.PlantingDatePlan,
                    HarvestingDatePlan = latestPlannedSeason?.HarvestingDatePlan,
                    PlantingDate = latestActualSeason?.PlantingDate,
                    HarvestingDate = latestActualSeason?.HarvestingDate,
                    PlantingPeriodPlan = plantingPeriodPlan,
                    PlantingPeriodActual = plantingPeriodActual,
                    CropSquare = cropSquare
                };

                result.Add(temp);
            }
            return result;
        }

    }

    public class CustomFieldViewModel
    {
        public Field Field { get; set; }
        public DateOnly? LastBidDate { get; set; }
        public List<Masofa.Common.Models.Dictionaries.Crop> Crops { get; set; }
        public List<Masofa.Common.Models.Dictionaries.Variety> Varieties { get; set; }
        public string AgricultureProducerName { get; set; }
        public Masofa.Common.Models.Dictionaries.Region Region { get; set; }

        public DateOnly? PlantingDatePlan { get; set; }
        public DateOnly? HarvestingDatePlan { get; set; }
        public DateOnly? PlantingDate { get; set; }
        public DateOnly? HarvestingDate { get; set; }
        public string? PlantingPeriodPlan { get; set; }
        public string? PlantingPeriodActual { get; set; }
        public Dictionary<Guid, double> CropSquare { get; set; } = [];
    }
}
