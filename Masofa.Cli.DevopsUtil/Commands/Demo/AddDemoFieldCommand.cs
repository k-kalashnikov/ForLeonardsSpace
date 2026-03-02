using Masofa.Cli.DevopsUtil.Commands.SatelliteDownloader.Sentinel2;
using Masofa.Client.Copernicus;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Masofa.Cli.DevopsUtil.Commands.Demo
{
    [BaseCommand("Add DemoField Command", "Add DemoField Command\"")]
    public class AddDemoFieldCommand : IBaseCommand
    {
        ILogger<AddDemoFieldCommand> Logger { get; }
        private SentinelServiceOptions SentinelServiceOptions { get; }
        private CopernicusApiUnitOfWork CopernicusApiUnitOfWork { get; }
        private MasofaSentinelDbContext SentinelDbContext { get; }
        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
        private MasofaIndicesDbContext MasofaIndicesDbContext { get; set; }
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
        private IFileStorageProvider FileStorageProvider { get; set; }
        public AddDemoFieldCommand(ILogger<AddDemoFieldCommand> logger,
                    CopernicusApiUnitOfWork copernicusApiUnitOfWork,
                    MasofaSentinelDbContext sentinelDbContext,
                    IOptions<SentinelServiceOptions> options,
                    MasofaCommonDbContext masofaCommonDbContext,
                    IFileStorageProvider fileStorageProvider,
                    MasofaIndicesDbContext masofaIndicesDbContext,
                    MasofaCropMonitoringDbContext masofaCropMonitoringDbContext,
                    MasofaDictionariesDbContext masofaDictionariesDbContext)
        {
            Logger = logger;
            CopernicusApiUnitOfWork = copernicusApiUnitOfWork;
            SentinelDbContext = sentinelDbContext;
            SentinelServiceOptions = options.Value;
            SentinelServiceOptions.SatelliteSearchConfig = new SatelliteSearchConfig();
            MasofaCommonDbContext = masofaCommonDbContext;
            FileStorageProvider = fileStorageProvider;
            MasofaIndicesDbContext = masofaIndicesDbContext;
            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
        }
        public async Task Execute()
        {
            //await CalculateIndices();
        }

        public async Task AddBidResult()
        {
            var bid = MasofaCropMonitoringDbContext.Bids.AsNoTracking().First(b => b.Id == Guid.Parse("45d58025-58fd-4c8e-bcca-2bbb8e80c68d"));
        }

        public async Task AddField()
        {
            try
            {
                var primeField = MasofaCropMonitoringDbContext.Fields.AsNoTracking().First(f => f.Id == Guid.Parse("0199bd3a-1ed5-7e91-8d4f-26821092cbd1"));
                var demoFieldId = Guid.NewGuid();
                var demoField = new Masofa.Common.Models.CropMonitoring.Field()
                {
                    Id = demoFieldId,
                    FieldArea = primeField.FieldArea,
                    AgricultureProducerId = primeField.AgricultureProducerId,
                    AgroclimaticZoneId = primeField.AgroclimaticZoneId,
                    BonitetScore = primeField.BonitetScore,
                    Classifier = primeField.Classifier,
                    Comment = primeField.Comment,
                    Control = true,
                    IrrigationSourceId = primeField.IrrigationSourceId,
                    IrrigationTypeId = primeField.IrrigationTypeId,
                    Name = "Demo For 2025-11-12",
                    Polygon = primeField.Polygon,
                    SoilIndex = primeField.SoilIndex,
                    SoilTypeId = primeField.SoilTypeId,
                    WaterSaving = true,
                    Status = Common.Models.StatusType.Active,
                    CreateAt = DateTime.UtcNow,
                    LastUpdateAt = DateTime.UtcNow,
                    CreateUser = Guid.Parse("01993e2c-0880-716b-9f41-27686a6a5e0a"),
                    LastUpdateUser = Guid.Parse("01993e2c-0880-716b-9f41-27686a6a5e0a"),
                    RegionId = primeField.RegionId
                };
                demoField = MasofaCropMonitoringDbContext.Fields.Add(demoField).Entity;
                MasofaCropMonitoringDbContext.SaveChanges();
                var primeSeasons = MasofaCropMonitoringDbContext.Seasons.AsNoTracking().Where(s => s.FieldId == primeField.Id).ToList();
                foreach (var season in primeSeasons)
                {
                    var tempSeason = new Masofa.Common.Models.CropMonitoring.Season()
                    {
                        Id = Guid.NewGuid(),
                        FieldArea = season.FieldArea,
                        FieldId = demoField.Id,
                        CropId = season.CropId,
                        PlantingDate = season.PlantingDate,
                        PlantingDatePlan = season.PlantingDatePlan,
                        HarvestingDate = season.HarvestingDate,
                        HarvestingDatePlan = season.HarvestingDatePlan,
                        Latitude = season.Latitude,
                        Longitude = season.Longitude,
                        Polygon = season.Polygon,
                        Status = Common.Models.StatusType.Active,
                        Title = season.Title,
                        YieldFact = season.YieldFact,
                        YieldHaFact = season.YieldHaFact,
                        CreateAt = DateTime.UtcNow,
                        LastUpdateAt = DateTime.UtcNow,
                        CreateUser = Guid.Parse("01993e2c-0880-716b-9f41-27686a6a5e0a"),
                        LastUpdateUser = Guid.Parse("01993e2c-0880-716b-9f41-27686a6a5e0a")
                    };
                    tempSeason = MasofaCropMonitoringDbContext.Seasons.Add(tempSeason).Entity;
                    Console.WriteLine($"Added season {tempSeason.Id} for demo field {demoField.Id}");
                }
                MasofaCropMonitoringDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
            }

        }
        public async Task CalculateIndices()
        {
            var tempArviReports = new List<ArviSeasonReport>();
            var tempEviReports = new List<EviSeasonReport>();
            var tempGndviReports = new List<GndviSeasonReport>();
            var tempMndwiReports = new List<MndwiSeasonReport>();
            var tempNdmiReports = new List<NdmiSeasonReport>();
            var tempNdviReports = new List<NdviSeasonReport>();
            var tempOrviReports = new List<OrviSeasonReport>();
            var tempOsaviReports = new List<OsaviSeasonReport>();

            var season = MasofaCropMonitoringDbContext.Seasons.AsNoTracking().First(s => s.Id == Guid.Parse("a2ad391d-d59b-4fc2-aa08-70c80d8817df"));

            var existingArviReports = MasofaIndicesDbContext.ArviSeasonReports.Where(r => r.SeasonId == season.Id).ToList();
            if (existingArviReports.Any())
            {
                MasofaIndicesDbContext.ArviSeasonReports.RemoveRange(existingArviReports);
                await MasofaIndicesDbContext.SaveChangesAsync();
            }
            var existingEviReports = MasofaIndicesDbContext.EviSeasonReports.Where(r => r.SeasonId == season.Id).ToList();
            if (existingEviReports.Any())
            {
                MasofaIndicesDbContext.EviSeasonReports.RemoveRange(existingEviReports);
                await MasofaIndicesDbContext.SaveChangesAsync();
            }
            var existingGndviReports = MasofaIndicesDbContext.GndviSeasonReports.Where(r => r.SeasonId == season.Id).ToList();
            if (existingGndviReports.Any())
            {
                MasofaIndicesDbContext.GndviSeasonReports.RemoveRange(existingGndviReports);
                await MasofaIndicesDbContext.SaveChangesAsync();
            }
            var existingMndwiReports = MasofaIndicesDbContext.MndwiSeasonReports.Where(r => r.SeasonId == season.Id).ToList();
            if (existingMndwiReports.Any())
            {
                MasofaIndicesDbContext.MndwiSeasonReports.RemoveRange(existingMndwiReports);
                await MasofaIndicesDbContext.SaveChangesAsync();
            }
            var existingNdmiReports = MasofaIndicesDbContext.NdmiSeasonReports.Where(r => r.SeasonId == season.Id).ToList();
            if (existingNdmiReports.Any())
            {
                MasofaIndicesDbContext.NdmiSeasonReports.RemoveRange(existingNdmiReports);
                await MasofaIndicesDbContext.SaveChangesAsync();
            }
            var existingNdviReports = MasofaIndicesDbContext.NdviSeasonReports.Where(r => r.SeasonId == season.Id).ToList();
            if (existingNdviReports.Any())
            {
                MasofaIndicesDbContext.NdviSeasonReports.RemoveRange(existingNdviReports);
                await MasofaIndicesDbContext.SaveChangesAsync();
            }
            var existingOrviReports = MasofaIndicesDbContext.OrviSeasonReports.Where(r => r.SeasonId == season.Id).ToList();
            if (existingOrviReports.Any())
            {
                MasofaIndicesDbContext.OrviSeasonReports.RemoveRange(existingOrviReports);
                await MasofaIndicesDbContext.SaveChangesAsync();
            }
            var existingOsaviReports = MasofaIndicesDbContext.OsaviSeasonReports.Where(r => r.SeasonId == season.Id).ToList();
            if (existingOsaviReports.Any())
            {
                MasofaIndicesDbContext.OsaviSeasonReports.RemoveRange(existingOsaviReports);
                await MasofaIndicesDbContext.SaveChangesAsync();
            }

            // Вегетационные индексы для картофеля
            for (DateOnly currentDay = season.PlantingDate.Value; currentDay <= season.HarvestingDate; currentDay = currentDay.AddDays(6))
            {
                // Примерное количество дней от начала вегетации для текущего дня
                var daysFromPlanting = currentDay.Day - season.PlantingDate.Value.Day;

                // Определяем фазу роста картофеля для текущего дня
                // 0-15: посадка/всходы, 16-35: вегетация, 36-65: бутонизация/цветение, 66-90: формирование клубней, 90+: вызревание
                string growthPhase = "maturity";
                if (daysFromPlanting < 7) growthPhase = "emergence";
                else if (daysFromPlanting < 21) growthPhase = "early_vegetation";
                else if (daysFromPlanting < 42) growthPhase = "vegetation";
                else if (daysFromPlanting < 63) growthPhase = "tuber_initiation";
                else if (daysFromPlanting < 84) growthPhase = "tuber_development";
                else if (daysFromPlanting < 98) growthPhase = "maturity";

                // Вспомогательная функция для генерации среднего значения индекса в зависимости от фазы
                double GetAverageForPhase(double peakValue, double declineAfterPeak, string phase)
                {
                    switch (phase)
                    {
                        case "emergence":
                            return Random.Shared.NextDouble() * 0.15; // низкие значения
                        case "early_vegetation":
                            return 0.15 + Random.Shared.NextDouble() * 0.2; // 0.15 - 0.35
                        case "vegetation":
                            return 0.35 + Random.Shared.NextDouble() * 0.25; // 0.35 - 0.6
                        case "tuber_initiation":
                            return 0.6 + Random.Shared.NextDouble() * 0.15; // 0.6 - 0.75
                        case "tuber_development":
                            return 0.65 + Random.Shared.NextDouble() * 0.15; // 0.65 - 0.8
                        case "maturity":
                            return 0.5 + Random.Shared.NextDouble() * 0.2; // 0.5 - 0.7, может снижаться
                        default:
                            return 0.15 + Random.Shared.NextDouble() * 0.2;
                    }
                }

                // ARVI: коррелирует с хлорофиллом и пигментами, пик в период активной вегетации
                var tempArviReport = new ArviSeasonReport
                {
                    SeasonId = season.Id,
                    PosibleMaxValue = 1.0,
                    PosibleMinValue = -1.0,
                    TotalMax = 0.7, // Максимум для картофеля
                    TotalMin = -0.1,
                    DateOnly = currentDay
                };
                tempArviReport.Average = GetAverageForPhase(0.6, 0.4, growthPhase) * 0.7; // Умеренный пик

                // EVI: чувствителен к биомассе, высокие значения в пик вегетации
                var tempEviReport = new EviSeasonReport
                {
                    SeasonId = season.Id,
                    PosibleMaxValue = 1.0,
                    PosibleMinValue = -1.0,
                    TotalMax = 0.9, // EVI может быть высоким у густых культур
                    TotalMin = -0.1,
                    DateOnly = currentDay
                };
                tempEviReport.Average = GetAverageForPhase(0.8, 0.5, growthPhase);

                // GNDVI: чувствителен к хлорофиллу, повышается в активную вегетацию
                var tempGndviReport = new GndviSeasonReport
                {
                    SeasonId = season.Id,
                    PosibleMaxValue = 1.0,
                    PosibleMinValue = -1.0,
                    TotalMax = 0.8,
                    TotalMin = -0.1,
                    DateOnly = currentDay
                };
                tempGndviReport.Average = GetAverageForPhase(0.7, 0.45, growthPhase);

                // MNDWI: чувствителен к влажности, может быть отрицательным или близким к 0
                var tempMndwiReport = new MndwiSeasonReport
                {
                    SeasonId = season.Id,
                    PosibleMaxValue = 1.0,
                    PosibleMinValue = -1.0,
                    TotalMax = 0.5,
                    TotalMin = -0.5,
                    DateOnly = currentDay
                };
                // Влажность может колебаться, но в целом положительна в активный рост
                double moisturePeak = 0.2;
                if (growthPhase == "maturity") moisturePeak = Random.Shared.NextDouble() * 0.3 - 0.1; // Может снижаться
                tempMndwiReport.Average = moisturePeak + (Random.Shared.NextDouble() * 0.15 - 0.075); // Небольшие колебания

                // NDMI: также чувствителен к влажности, может быть от -1 до 1
                var tempNdmiReport = new NdmiSeasonReport
                {
                    SeasonId = season.Id,
                    PosibleMaxValue = 1.0,
                    PosibleMinValue = -1.0,
                    TotalMax = 0.6,
                    TotalMin = -0.6,
                    DateOnly = currentDay
                };
                tempNdmiReport.Average = moisturePeak + (Random.Shared.NextDouble() * 0.2 - 0.1); // Схоже с MNDWI

                // NDVI: основной индекс, отражает зелёную биомассу
                var tempNdviReport = new NdviSeasonReport
                {
                    SeasonId = season.Id,
                    PosibleMaxValue = 1.0,
                    PosibleMinValue = -1.0,
                    TotalMax = 0.9,
                    TotalMin = -0.1,
                    DateOnly = currentDay
                };
                tempNdviReport.Average = GetAverageForPhase(0.85, 0.5, growthPhase);

                // ORVI: оптимизированный индекс, аналогичен NDVI, но с весами
                var tempOrviReport = new OrviSeasonReport
                {
                    SeasonId = season.Id,
                    PosibleMaxValue = 1.0,
                    PosibleMinValue = -1.0,
                    TotalMax = 0.85,
                    TotalMin = -0.1,
                    DateOnly = currentDay
                };
                tempOrviReport.Average = GetAverageForPhase(0.8, 0.5, growthPhase);

                // OSAVI: улучшенный NDVI для высокой биомассы
                var tempOsaviReport = new OsaviSeasonReport
                {
                    SeasonId = season.Id,
                    PosibleMaxValue = 1.0,
                    PosibleMinValue = -1.0,
                    TotalMax = 0.9,
                    TotalMin = -0.1,
                    DateOnly = currentDay
                };
                tempOsaviReport.Average = GetAverageForPhase(0.82, 0.5, growthPhase);

                tempArviReports.Add(tempArviReport);
                tempEviReports.Add(tempEviReport);
                tempGndviReports.Add(tempGndviReport);
                tempMndwiReports.Add(tempMndwiReport);
                tempNdmiReports.Add(tempNdmiReport);
                tempNdviReports.Add(tempNdviReport);
                tempOrviReports.Add(tempOrviReport);
                tempOsaviReports.Add(tempOsaviReport);
            }

            MasofaIndicesDbContext.ArviSeasonReports.AddRange(tempArviReports);
            MasofaIndicesDbContext.EviSeasonReports.AddRange(tempEviReports);
            MasofaIndicesDbContext.GndviSeasonReports.AddRange(tempGndviReports);
            MasofaIndicesDbContext.MndwiSeasonReports.AddRange(tempMndwiReports);
            MasofaIndicesDbContext.NdmiSeasonReports.AddRange(tempNdmiReports);
            MasofaIndicesDbContext.NdviSeasonReports.AddRange(tempNdviReports);
            MasofaIndicesDbContext.OrviSeasonReports.AddRange(tempOrviReports);
            MasofaIndicesDbContext.OsaviSeasonReports.AddRange(tempOsaviReports);
            await MasofaIndicesDbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }



        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
