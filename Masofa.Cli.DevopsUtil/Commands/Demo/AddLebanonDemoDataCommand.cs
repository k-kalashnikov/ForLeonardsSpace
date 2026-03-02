using Masofa.Common.Attributes;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GeoJsonSerializer = NetTopologySuite.IO.GeoJsonSerializer;
using NtsAttributesTable = NetTopologySuite.Features.AttributesTable;
using NtsFeature = NetTopologySuite.Features.Feature;
using NtsFeatureCollection = NetTopologySuite.Features.FeatureCollection;
using NtsGeometry = NetTopologySuite.Geometries.Geometry;
using Polygon = NetTopologySuite.Geometries.Polygon;

namespace Masofa.Cli.DevopsUtil.Commands.Demo
{
    [BaseCommand("Add Lebanon DemoField Command", "Add Lebanon DemoField Command\"")]
    public class AddLebanonDemoDataCommand : IBaseCommand
    {
        private GeometryFactory GeometryFactory { get; }
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; }
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; }
        private MasofaIndicesDbContext MasofaIndicesDbContext { get; }
        public AddLebanonDemoDataCommand(GeometryFactory geometryFactory, MasofaCropMonitoringDbContext masofaCropMonitoringDbContext, MasofaDictionariesDbContext masofaDictionariesDbContext, MasofaIndicesDbContext masofaIndicesDbContext)
        {
            GeometryFactory = geometryFactory;
            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
            MasofaIndicesDbContext = masofaIndicesDbContext;
        }

        public void Dispose()
        {

        }

        public async Task Execute()
        {
            Console.WriteLine("Adding Lebanon demo data...");
            //await AddCrops();
            //await AddFieldsAndSeasons();
            await AddIndicesReports();
        }

        public async Task AddCrops()
        {
            var crops = new List<Crop>
            {
                new Crop
                {
                    CreateAt = DateTime.UtcNow.AddDays(-5),
                    LastUpdateAt = DateTime.UtcNow.AddDays(-5),
                    CreateUser = Guid.Parse("01993e2c-0880-716b-9f41-27686a6a5e0a"),
                    LastUpdateUser = Guid.Parse("01993e2c-0880-716b-9f41-27686a6a5e0a"),
                    IsPublic = true,
                    Names = new Dictionary<string, string>
                    {
                        { "en-US", "Okra" },
                        { "ar-LB", "حسنا" },
                        { "ru-RU", "Окра" }
                    }
                },
                new Crop
                {
                    CreateAt = DateTime.UtcNow.AddDays(-5),
                    LastUpdateAt = DateTime.UtcNow.AddDays(-5),
                    CreateUser = Guid.Parse("01993e2c-0880-716b-9f41-27686a6a5e0a"),
                    LastUpdateUser = Guid.Parse("01993e2c-0880-716b-9f41-27686a6a5e0a"),
                    IsPublic = true,
                    Names = new Dictionary<string, string>
                    {
                        { "en-US", "Eggplant" },
                        { "ar-LB", "باذنجان" },
                        { "ru-RU", "Баклажан" }
                    },
                },
                new Crop
                {
                    CreateAt = DateTime.UtcNow.AddDays(-5),
                    LastUpdateAt = DateTime.UtcNow.AddDays(-5),
                    CreateUser = Guid.Parse("01993e2c-0880-716b-9f41-27686a6a5e0a"),
                    LastUpdateUser = Guid.Parse("01993e2c-0880-716b-9f41-27686a6a5e0a"),
                    IsPublic = true,
                    Names = new Dictionary<string, string>
                    {
                        { "en-US", "Avocado" },
                        { "ar-LB", "أفوكادو" },
                        { "ru-RU", "Авокадо" }
                    },
                }
            };

            foreach (var crop in crops)
            {
                MasofaDictionariesDbContext.Crops.Add(crop);
            }
            await MasofaDictionariesDbContext.SaveChangesAsync();
        }
        public async Task AddFieldsAndSeasons()
        {
            Console.WriteLine("Enter pls field data GeoJson");
            var filePath = Console.ReadLine();
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                Console.WriteLine("Invalid file path.");
                return;
            }
            var geoJson = await File.ReadAllTextAsync(filePath);
            var features = ReadFeatureCollectionAsync(geoJson, CancellationToken.None);
            var index = 1;
            foreach (var feature in await features)
            {
                if (!(feature.Geometry is Polygon))
                {
                    continue;
                }
                var tempPolygon = (Polygon)feature.Geometry;
                var tempField = new Field
                {
                    Name = $"Demo Lebanon Field {feature.Attributes["name"]} {index.ToString()}",
                    Polygon = tempPolygon,
                    FieldArea = tempPolygon.Area,
                    IsPublic = true,
                    CreateAt = DateTime.UtcNow.AddDays(-2),
                    LastUpdateAt = DateTime.UtcNow.AddDays(-2),
                    CreateUser = Guid.Parse("01993e2c-0880-716b-9f41-27686a6a5e0a"),
                    LastUpdateUser = Guid.Parse("01993e2c-0880-716b-9f41-27686a6a5e0a")
                };
                tempField = MasofaCropMonitoringDbContext.Fields.Add(tempField).Entity;

                var cropId = MasofaDictionariesDbContext.Crops.ToList().FirstOrDefault(c => (c.Names["en-US"] ?? string.Empty) == (feature.Attributes["name"] ?? "Undefined"))?.Id ?? Guid.Empty;

                for (var currentDate = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc); currentDate < DateTime.UtcNow; currentDate = currentDate.AddDays(Random.Shared.Next(15, 30)))
                {
                    var tempSeason = new Season()
                    {
                        FieldArea = tempField.FieldArea,
                        FieldId = tempField.Id,
                        StartDate = DateOnly.FromDateTime(currentDate),
                        EndDate = DateOnly.FromDateTime(currentDate.AddMonths(3)),
                        YieldFact = Random.Shared.Next(100, 150),
                        YieldHaFact = Random.Shared.Next(100, 150),
                        YieldHaPlan = Random.Shared.Next(100, 150),
                        YieldPlan = Random.Shared.Next(100, 150),
                        CreateAt = DateTime.UtcNow.AddDays(-2),
                        LastUpdateAt = DateTime.UtcNow.AddDays(-2),
                        CreateUser = Guid.Parse("01993e2c-0880-716b-9f41-27686a6a5e0a"),
                        LastUpdateUser = Guid.Parse("01993e2c-0880-716b-9f41-27686a6a5e0a"),
                        PlantingDatePlan = DateOnly.FromDateTime(currentDate.AddDays(-1)),
                        HarvestingDatePlan = DateOnly.FromDateTime(currentDate.AddDays(Random.Shared.Next(15, 30))),
                        PlantingDate = DateOnly.FromDateTime(currentDate),
                        HarvestingDate = DateOnly.FromDateTime(currentDate.AddDays(Random.Shared.Next(15, 30))),
                        IsPublic = true,
                        Latitude = tempPolygon.Centroid.Y,
                        Longitude = tempPolygon.Centroid.X,
                        Polygon = tempPolygon,
                        Status = Common.Models.StatusType.Active,
                        Title = $"Demo Lebanon Season {feature.Attributes["name"]} {index.ToString()}",
                        CropId = cropId
                    };

                    MasofaCropMonitoringDbContext.Seasons.Add(tempSeason);
                }
            }
            await MasofaCropMonitoringDbContext.SaveChangesAsync();
        }
        public async Task AddIndicesReports()
        {
            var seasonsIds = new List<Guid>
            {
                Guid.Parse("019a637a-6994-785e-961a-703708004cc7"),
                Guid.Parse("019a637a-69b0-7151-b307-dc5de252080c"),
                Guid.Parse("019a637a-69b0-7196-b405-abb0c920af42"),
                Guid.Parse("019a637a-69b0-71ad-85b7-977b260b1b05"),
                Guid.Parse("019a637a-69b0-7646-a389-8711a4b310f8"),
                Guid.Parse("019a637a-69b0-78ae-ac9a-44ca49b81d5f"),
                Guid.Parse("019a637a-69f1-723f-9984-e1672c09b225"),
                Guid.Parse("019a637a-69f1-75d5-973b-0f1a84ecf36a"),
                Guid.Parse("019a637a-69f1-7744-8760-21e407d6af08"),
                Guid.Parse("019a637a-69f1-7791-b98e-d2b6c390a397"),
                Guid.Parse("019a637a-69f1-7808-8b38-f22444b19cb4"),
                Guid.Parse("019a637a-69f1-7b17-a584-2ab21aa6606a"),
                Guid.Parse("019a637a-69f1-7b41-a7a8-6831308d82d5")
            };

            foreach (var seasonId in seasonsIds)
            {
                var existingArviReports = MasofaIndicesDbContext.ArviSeasonReports.Where(r => r.SeasonId == seasonId).ToList();
                if (existingArviReports.Any())
                {
                    MasofaIndicesDbContext.ArviSeasonReports.RemoveRange(existingArviReports);
                    await MasofaIndicesDbContext.SaveChangesAsync();
                }
                var existingEviReports = MasofaIndicesDbContext.EviSeasonReports.Where(r => r.SeasonId == seasonId).ToList();
                if (existingEviReports.Any())
                {
                    MasofaIndicesDbContext.EviSeasonReports.RemoveRange(existingEviReports);
                    await MasofaIndicesDbContext.SaveChangesAsync();
                }
                var existingGndviReports = MasofaIndicesDbContext.GndviSeasonReports.Where(r => r.SeasonId == seasonId).ToList();
                if (existingGndviReports.Any())
                {
                    MasofaIndicesDbContext.GndviSeasonReports.RemoveRange(existingGndviReports);
                    await MasofaIndicesDbContext.SaveChangesAsync();
                }
                var existingMndwiReports = MasofaIndicesDbContext.MndwiSeasonReports.Where(r => r.SeasonId == seasonId).ToList();
                if (existingMndwiReports.Any())
                {
                    MasofaIndicesDbContext.MndwiSeasonReports.RemoveRange(existingMndwiReports);
                    await MasofaIndicesDbContext.SaveChangesAsync();
                }
                var existingNdmiReports = MasofaIndicesDbContext.NdmiSeasonReports.Where(r => r.SeasonId == seasonId).ToList();
                if (existingNdmiReports.Any())
                {
                    MasofaIndicesDbContext.NdmiSeasonReports.RemoveRange(existingNdmiReports);
                    await MasofaIndicesDbContext.SaveChangesAsync();
                }
                var existingNdviReports = MasofaIndicesDbContext.NdviSeasonReports.Where(r => r.SeasonId == seasonId).ToList();
                if (existingNdviReports.Any())
                {
                    MasofaIndicesDbContext.NdviSeasonReports.RemoveRange(existingNdviReports);
                    await MasofaIndicesDbContext.SaveChangesAsync();
                }
                var existingOrviReports = MasofaIndicesDbContext.OrviSeasonReports.Where(r => r.SeasonId == seasonId).ToList();
                if (existingOrviReports.Any())
                {
                    MasofaIndicesDbContext.OrviSeasonReports.RemoveRange(existingOrviReports);
                    await MasofaIndicesDbContext.SaveChangesAsync();
                }
                var existingOsaviReports = MasofaIndicesDbContext.OsaviSeasonReports.Where(r => r.SeasonId == seasonId).ToList();
                if (existingOsaviReports.Any())
                {
                    MasofaIndicesDbContext.OsaviSeasonReports.RemoveRange(existingOsaviReports);
                    await MasofaIndicesDbContext.SaveChangesAsync();
                }

                var season = await MasofaCropMonitoringDbContext.Seasons.FirstAsync(s => s.Id == seasonId);


                var tempArviReports = new List<ArviSeasonReport>();
                var tempEviReports = new List<EviSeasonReport>();
                var tempGndviReports = new List<GndviSeasonReport>();
                var tempMndwiReports = new List<MndwiSeasonReport>();
                var tempNdmiReports = new List<NdmiSeasonReport>();
                var tempNdviReports = new List<NdviSeasonReport>();
                var tempOrviReports = new List<OrviSeasonReport>();
                var tempOsaviReports = new List<OsaviSeasonReport>();

                for (DateOnly currentDay = season.PlantingDate.Value; currentDay <= season.HarvestingDate; currentDay = currentDay.AddDays(6))
                {
                    var tempArviReport = new ArviSeasonReport
                    {
                        SeasonId = season.Id,
                        PosibleMaxValue = 1.0,
                        PosibleMinValue = -1.0,
                        TotalMax = Random.Shared.Next(1, 6) * 0.1,
                        TotalMin = -0.4,
                        DateOnly = currentDay
                    };
                    tempArviReport.Average = (Random.Shared.Next(1, (int)(tempArviReport.TotalMax * 100)) * 0.01);

                    var tempEviReport = new EviSeasonReport
                    {
                        SeasonId = season.Id,
                        PosibleMaxValue = 1.0,
                        PosibleMinValue = -1.0,
                        TotalMax = Random.Shared.Next(1, 6) * 0.1,
                        TotalMin = -0.4,
                        DateOnly = currentDay
                    };
                    tempEviReport.Average = (Random.Shared.Next(1, (int)(tempEviReport.TotalMax * 100)) * 0.01);

                    var tempGndviReport = new GndviSeasonReport
                    {
                        SeasonId = season.Id,
                        PosibleMaxValue = 1.0,
                        PosibleMinValue = -1.0,
                        TotalMax = Random.Shared.Next(1, 6) * 0.1,
                        TotalMin = -0.4,
                        DateOnly = currentDay
                    };
                    tempGndviReport.Average = (Random.Shared.Next(1, (int)(tempGndviReport.TotalMax * 100)) * 0.01);

                    var tempMndwiReport = new MndwiSeasonReport
                    {
                        SeasonId = season.Id,
                        PosibleMaxValue = 1.0,
                        PosibleMinValue = -1.0,
                        TotalMax = Random.Shared.Next(1, 6) * 0.1,
                        TotalMin = -0.4,
                        DateOnly = currentDay
                    };
                    tempMndwiReport.Average = (Random.Shared.Next(1, (int)(tempMndwiReport.TotalMax * 100)) * 0.01);

                    var tempNdmiReport = new NdmiSeasonReport
                    {
                        SeasonId = season.Id,
                        PosibleMaxValue = 1.0,
                        PosibleMinValue = -1.0,
                        TotalMax = Random.Shared.Next(1, 6) * 0.1,
                        TotalMin = -0.4,
                        DateOnly = currentDay
                    };
                    tempNdmiReport.Average = (Random.Shared.Next(1, (int)(tempNdmiReport.TotalMax * 100)) * 0.01);

                    var tempNdviReport = new NdviSeasonReport
                    {
                        SeasonId = season.Id,
                        PosibleMaxValue = 1.0,
                        PosibleMinValue = -1.0,
                        TotalMax = Random.Shared.Next(1, 6) * 0.1,
                        TotalMin = -0.4,
                        DateOnly = currentDay
                    };
                    tempNdviReport.Average = (Random.Shared.Next(1, (int)(tempNdviReport.TotalMax * 100)) * 0.01);

                    var tempOrviReport = new OrviSeasonReport
                    {
                        SeasonId = season.Id,
                        PosibleMaxValue = 1.0,
                        PosibleMinValue = -1.0,
                        TotalMax = Random.Shared.Next(1, 6) * 0.1,
                        TotalMin = -0.4,
                        DateOnly = currentDay
                    };
                    tempOrviReport.Average = (Random.Shared.Next(1, (int)(tempOrviReport.TotalMax * 100)) * 0.01);

                    var tempOsaviReport = new OsaviSeasonReport
                    {
                        SeasonId = season.Id,
                        PosibleMaxValue = 1.0,
                        PosibleMinValue = -1.0,
                        TotalMax = Random.Shared.Next(1, 6) * 0.1,
                        TotalMin = -0.4,
                        DateOnly = currentDay
                    };
                    tempOsaviReport.Average = (Random.Shared.Next(1, (int)(tempOsaviReport.TotalMax * 100)) * 0.01);

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
        }


        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }


        private async Task<NtsFeatureCollection> ReadFeatureCollectionAsync(string text, CancellationToken ct)
        {
            using var jr = new JsonTextReader(new StringReader(text));

            var serializer = GeoJsonSerializer.Create(this.GeometryFactory);

            var fc = serializer.Deserialize<NtsFeatureCollection>(jr);
            if (fc != null)
                return fc;

            // допустим «голая» геометрия
            jr.Close();
            using var jr2 = new JsonTextReader(new StringReader(text));
            NtsGeometry? geom = serializer.Deserialize<NtsGeometry>(jr2);

            var coll = new NtsFeatureCollection();
            if (geom != null)
                coll.Add(new NtsFeature(geom, new NtsAttributesTable()));
            return coll;
        }
    }

    public class InputViewModel
    {
        public Point Location { get; set; }
        public ReportType ReportType { get; set; }
        public string ReportConfigJson { get; set; }
    }

    public class HourConfig
    {
        public DateTime? DateTime { get; set; }
    }

    public class DayConfig 
    {
        public DateOnly? DateOnly { get; set; }
    }

    public class WeekConfig
    {
        public DateOnly? DateOnlyStart { get; set; }
        public DateOnly? DateOnlyEnd { get; set; }
    }

    public class MonthConfig
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
    }

    public class YearConfig
    {
        public int? Year { get; set; }
    }

    public enum ReportType
    {
        Hour = 0,
        Day = 1,
        Week = 3,
        Month = 4,
        Year = 5
    }

    public class OutputWeatherReport
    {
        /// <summary>
        /// Температура средняя
        /// </summary>
        [NotMapped]
        [ReportValue(ColorTable = "Temperature")]
        public double TemperatureAverage
        {
            get
            {
                return (TemperatureMin + TemperatureMax) / 2.0;
            }
        }

        /// <summary>
        /// Температура максимальная (средняя)
        /// </summary>
        [ReportValue(ColorTable = "Temperature")]
        public double TemperatureMin { get; set; }

        /// <summary>
        /// Температура минимальная (средняя)
        /// </summary>
        [ReportValue(ColorTable = "Temperature")]
        public double TemperatureMax { get; set; }

        /// <summary>
        /// Температура максимальная (максимальная)
        /// </summary>
        [ReportValue(ColorTable = "Temperature")]
        public double TemperatureMinTotal { get; set; }

        /// <summary>
        /// Температура минимальная (минимальная)
        /// </summary>
        [ReportValue(ColorTable = "Temperature")]
        public double TemperatureMaxTotal { get; set; }

        /// <summary>
        /// Солнечное излучение
        /// </summary>
        [ReportValue(ColorTable = "Radiation")]
        public double SolarRadiationInfluence { get; set; }

        /// <summary>
        /// Осадки
        /// </summary>
        [ReportValue(ColorTable = "Fallout")]
        public double Fallout { get; set; }

        /// <summary>
        /// Влажность
        /// </summary>
        [ReportValue(ColorTable = "Humidity")]
        public double Humidity { get; set; }

        /// <summary>
        /// Скорость ветра
        /// </summary>
        public double WindSpeed { get; set; }

        /// <summary>
        /// Направление ветра
        /// </summary>
        public double WindDerection { get; set; }

        /// <summary>
        /// Точка Era5
        /// </summary>
        public Point WeatherStation { get; set; }
    }
}
