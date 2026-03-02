using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Masofa.Cli.DevopsUtil.Commands.CropMonitoring
{
    /// <summary>
    /// CLI-команда для заполнения дат посева и сбора урожая у сезонов.
    /// </summary>
    /// <remarks>
    /// Обновляет только активные сезоны (<see cref="StatusType.Active"/>),
    /// у которых отсутствуют даты посева и/или сбора урожая.
    /// Для посева выбирается случайная дата из первых 7 дней августа 2025 года,
    /// для уборки — случайная дата через 115–125 дней после даты посева.
    /// </remarks>
    [BaseCommand("Crop: Fill Season Planting Dates", "Заполнение дат посева и уборки для активных сезонов без дат")]
    public class FillSeasonPlantingDatesCommand : IBaseCommand
    {
        private readonly MasofaCropMonitoringDbContext _cropMonitoringDbContext;

        public FillSeasonPlantingDatesCommand(MasofaCropMonitoringDbContext cropMonitoringDbContext)
        {
            _cropMonitoringDbContext = cropMonitoringDbContext;
        }

        public void Dispose()
        {
        }

        public async Task Execute()
        {
            Console.WriteLine("Поиск сезонов со статусом Active без дат посева/уборки...");

            var seasons = await _cropMonitoringDbContext.Seasons
                .Where(s => s.Status == StatusType.Active &&
                            (s.PlantingDate == null || s.HarvestingDate == null))
                .ToListAsync();

            if (seasons.Count == 0)
            {
                Console.WriteLine("Подходящих сезонов не найдено.");
                return;
            }

            Console.WriteLine($"Найдено сезонов для обновления: {seasons.Count}");

            var basePlantingDate = new DateOnly(2025, 4, 1);

            foreach (var season in seasons)
            {
                // Если дата посева отсутствует — назначаем случайную дату в первые 7 дней апреля 2025
                if (season.PlantingDate == null)
                {
                    var offsetDays = Random.Shared.Next(0, 7); // 0..6 -> 1–7 августа
                    season.PlantingDate = basePlantingDate.AddDays(offsetDays);
                }

                // Если дата уборки отсутствует — считаем её от даты посева
                if (season.HarvestingDate == null && season.PlantingDate != null)
                {
                    var harvestOffset = Random.Shared.Next(115, 126); // 115..125 дней
                    season.HarvestingDate = season.PlantingDate.Value.AddDays(harvestOffset);
                }

                season.LastUpdateAt = DateTime.UtcNow;
            }

            await _cropMonitoringDbContext.SaveChangesAsync();

            Console.WriteLine("Обновление сезонов завершено успешно.");
        }

        public Task Execute(string[] args)
        {
            return Execute();
        }
    }
}


