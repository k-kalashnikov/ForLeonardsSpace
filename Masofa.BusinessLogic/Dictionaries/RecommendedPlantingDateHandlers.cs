using Masofa.Common.Models;
using Masofa.Common.Models.Dictionaries;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Masofa.BusinessLogic.Features.RecommendedPlantingDates.Handlers
{
    public class CreateRecommendedPlantingDateHandler : IRequestHandler<CreateRecommendedPlantingDateCommand, Guid>
    {
        private readonly MasofaDictionariesDbContext _dbContext;

        public CreateRecommendedPlantingDateHandler(MasofaDictionariesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> Handle(CreateRecommendedPlantingDateCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            if (model == null) throw new ArgumentNullException(nameof(model));

            var exists = await _dbContext.Set<Masofa.Common.Models.Dictionaries.RecommendedPlantingDates>()
                .AnyAsync(x => x.CropId == model.CropId
                            && x.RegionId == model.RegionId
                            && x.Status == StatusType.Active, cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException($"Запись для данной культуры и региона уже существует.");
            }

            if (model.Periods != null && model.Periods.Any())
            {
                model.PeriodsJson = JsonConvert.SerializeObject(model.Periods);
            }
            else
            {
                model.PeriodsJson = null;
            }

            if (model.Id == Guid.Empty)
                model.Id = Guid.NewGuid();

            model.CreateAt = DateTime.UtcNow;
            model.Status = StatusType.Active;

            await _dbContext.Set<Masofa.Common.Models.Dictionaries.RecommendedPlantingDates>().AddAsync(model, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return model.Id;
        }
    }

    public class EditRecommendedPlantingDateHandler : IRequestHandler<EditRecommendedPlantingDateCommand, Guid>
    {
        private readonly MasofaDictionariesDbContext _dbContext;

        public EditRecommendedPlantingDateHandler(MasofaDictionariesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> Handle(EditRecommendedPlantingDateCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            if (model == null || model.Id == Guid.Empty)
                throw new ArgumentException("Model or ID is invalid");

            var entity = await _dbContext.Set<Masofa.Common.Models.Dictionaries.RecommendedPlantingDates>()
                .FirstOrDefaultAsync(x => x.Id == model.Id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"Entity with Id {model.Id} not found");

            var isDuplicate = await _dbContext.Set<Masofa.Common.Models.Dictionaries.RecommendedPlantingDates>()
                .AnyAsync(x => x.CropId == model.CropId
                            && x.RegionId == model.RegionId
                            && x.Status != StatusType.Deleted
                            && x.Id != model.Id, cancellationToken);

            if (isDuplicate)
            {
                throw new InvalidOperationException($"Запись для данной культуры и региона уже существует.");
            }

            entity.CropId = model.CropId;
            entity.RegionId = model.RegionId;

            if (model.Periods != null && model.Periods.Any())
            {
                entity.PeriodsJson = JsonConvert.SerializeObject(model.Periods);
            }
            else
            {
                entity.PeriodsJson = null;
            }

            entity.LastUpdateAt = DateTime.UtcNow;

            _dbContext.Set<Masofa.Common.Models.Dictionaries.RecommendedPlantingDates>().Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }

    public class GetRecommendedPlantingDatesCustomHandler : IRequestHandler<GetRecommendedPlantingDatesCustomQuery, List<Masofa.Common.Models.Dictionaries.RecommendedPlantingDates>>
    {
        private readonly MasofaDictionariesDbContext _dbContext;

        public GetRecommendedPlantingDatesCustomHandler(MasofaDictionariesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Masofa.Common.Models.Dictionaries.RecommendedPlantingDates>> Handle(GetRecommendedPlantingDatesCustomQuery request, CancellationToken cancellationToken)
        {
            var list = await _dbContext.Set<Masofa.Common.Models.Dictionaries.RecommendedPlantingDates>()
                                            .AsNoTracking()
                                            .Where(x => x.Status == StatusType.Active && x.CropId == request.CropId)
                                            .ToListAsync(cancellationToken);

            foreach (var item in list)
            {
                if (!string.IsNullOrEmpty(item.PeriodsJson))
                {
                    try
                    {
                        item.Periods = JsonConvert.DeserializeObject<List<PlantingPeriod>>(item.PeriodsJson)
                                       ?? new List<PlantingPeriod>();
                    }
                    catch
                    {
                        item.Periods = new List<PlantingPeriod>();
                    }
                }
            }

            return list;
        }
    }
}