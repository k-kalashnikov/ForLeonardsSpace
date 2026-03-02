using Masofa.BusinessLogic.Services;
using Masofa.Common.Models;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Models.Tiles;
using Masofa.DataAccess;

namespace Masofa.Cli.DevopsUtil.Commands.Weather
{
    [BaseCommand("Fill Weather Tile Layers", "Fill Weather Tile Layers")]
    public class FillWeatherTileLayers : IBaseCommand
    {
        private GeoServerService GeoServerService { get; set; }
        private MasofaTileDbContext TileDbContext { get; set; }

        public FillWeatherTileLayers(MasofaTileDbContext tileDbContext, GeoServerService geoServerService)
        {
            TileDbContext = tileDbContext;
            GeoServerService = geoServerService;
        }

        public void Dispose()
        {
            Console.WriteLine("\nFillWeatherTileLayers END");
        }

        public async Task Execute()
        {
            var indicesStartDate0 = DateOnly.Parse("2025-09-01");
            var indicesEndDate0 = DateOnly.Parse("2025-09-02");
            var indicesStartDate1 = DateOnly.Parse("2025-09-22");
            var indicesEndDate1 = DateOnly.Parse("2025-09-30");

            var pathStart = "/deploy/prod/data-geoserver-prod";

            List<string> indices = ["ARVI", "EVI", "GNDVI", "MNDWI", "NDMI", "NDVI", "ORVI", "OSAVI"];

            for (var i = 1; i < 3; i++)
            {
                foreach (var index in indices)
                {
                    for (var sd = indicesStartDate0; sd <= indicesEndDate0; sd = sd.AddDays(1))
                    {
                        var storeName = $"{index}_{sd.AddMonths(i):yyyyMMdd}";
                        var storePath = $"{index}/{index}_{sd:yyyyMMdd}";

                        //var storeResult = await GeoServerService.RecreateImageMosaicStoreAsync(storeName, storePath);
                        //var layerResult = await GeoServerService.PublishCoverageAsync(storeName, storeName);

                        var geoServerRelationPath = $"{pathStart}/{index}/{sd:yyyyMMdd}";

                        var tileLayer = new TileLayer
                        {
                            Names = new LocalizationString
                            {
                                ["en-US"] = index,
                                ["ru-RU"] = index,
                                ["uz-Latn-UZ"] = index
                            },
                            GeoServerName = storeName,
                            GeoServerRelationPath = geoServerRelationPath
                        };
                        await TileDbContext.TileLayers.AddAsync(tileLayer);

                        //Console.WriteLine($"{storeName} store: {storeResult} layer: {layerResult}");
                    }

                    for (var sd = indicesStartDate1; sd <= indicesEndDate1; sd = sd.AddDays(1))
                    {
                        var storeName = $"{index}_{sd.AddMonths(i):yyyyMMdd}";
                        var storePath = $"{index}/{index}_{sd:yyyyMMdd}";

                        //var storeResult = await GeoServerService.RecreateImageMosaicStoreAsync(storeName, storePath);
                        //var layerResult = await GeoServerService.PublishCoverageAsync(storeName, storeName);

                        var geoServerRelationPath = $"{pathStart}/{index}/{sd:yyyyMMdd}";

                        var tileLayer = new TileLayer
                        {
                            Names = new LocalizationString
                            {
                                ["en-US"] = index,
                                ["ru-RU"] = index,
                                ["uz-Latn-UZ"] = index
                            },
                            GeoServerName = storeName,
                            GeoServerRelationPath = geoServerRelationPath
                        };
                        await TileDbContext.TileLayers.AddAsync(tileLayer);

                        //Console.WriteLine($"{storeName} store: {storeResult} layer: {layerResult}");
                    }
                }
            }
            await TileDbContext.SaveChangesAsync();
            //Console.WriteLine($"storeResult: {storeResult}");
        }

        public async Task Execute1()
        {
            Console.WriteLine("nFillWeatherTileLayers START\n");

            Dictionary<string, string> namesEn = new Dictionary<string, string>()
            {
                {"FrostDanger", "Frost Danger" },
                {"Fallout", "Precipitation"},
                {"Humidity", "Humidity"},
                {"SolarRadiationInfluence", "Solar Radiation Influence"},
                {"TemperatureAverage", "Temperature Average"},
                {"TemperatureMax", "Temperature Max"},
                {"TemperatureMaxTotal", "Temperature Max Total"},
                {"TemperatureMin", "Temperature Min"},
                {"TemperatureMinTotal", "Temperature Min Total"},
                {"NormalizedFallout", "Norm of Precipitation"},
                {"NormalizedHumidity", "Norm of Humidity"},
                {"NormalizedSolarRadiationInfluence", "Norm of Solar Radiation Influence"},
                {"NormalizedTemperatureAverage", "Norm of Temperature Average"},
                {"NormalizedTemperatureMax", "Norm of Temperature Max"},
                {"NormalizedTemperatureMaxTotal", "Norm of Temperature Max Total"},
                {"NormalizedTemperatureMin", "Norm of Temperature Min"},
                {"NormalizedTemperatureMinTotal", "Norm of Temperature Min Total"},
                {"DeviationTemperatureAverage", "Deviation of Temperature Average from Norm"},
                {"DeviationTemperatureMax", "Deviation of Temperature Max from Norm"},
                {"DeviationTemperatureMaxTotal", "Deviation of Temperature Max Total from Norm"},
                {"DeviationTemperatureMin", "Deviation of Temperature Min from Norm"},
                {"DeviationTemperatureMinTotal", "Deviation of Temperature Min Total from Norm"}
            };

            Dictionary<string, string> namesRu = new Dictionary<string, string>()
            {
                {"FrostDanger", "Морозоопасность" },
                {"Fallout", "Осадки"},
                {"Humidity", "Влажность"},
                {"SolarRadiationInfluence", "Солнечная радиация"},
                {"TemperatureAverage", "Средняя температура"},
                {"TemperatureMax", "Максимальная температура"},
                {"TemperatureMaxTotal", "Максимальная общая температура"},
                {"TemperatureMin", "Минимальная температура"},
                {"TemperatureMinTotal", "Минимальная общая температура"},
                {"NormalizedFallout", "Норма осадков"},
                {"NormalizedHumidity", "Норма влажности"},
                {"NormalizedSolarRadiationInfluence", "Норма влияния солнечной радиации"},
                {"NormalizedTemperatureAverage", "Норма средней температуры"},
                {"NormalizedTemperatureMax", "Норма максимальной температуры"},
                {"NormalizedTemperatureMaxTotal", "Норма максимальной общей температуры"},
                {"NormalizedTemperatureMin", "Норма минимальной температуры"},
                {"NormalizedTemperatureMinTotal", "Норма минимальной общей температуры"},
                {"DeviationTemperatureAverage", "Отклонение средней температуры от нормы"},
                {"DeviationTemperatureMax", "Отклонение максимальной температуры от нормы"},
                {"DeviationTemperatureMaxTotal", "Отклонение максимальной общей температуры от нормы"},
                {"DeviationTemperatureMin", "Отклонение минимальной температуры от нормы"},
                {"DeviationTemperatureMinTotal", "Отклонение минимальной общей температуры от нормы"}
            };

            Dictionary<string, string> namesUz = new Dictionary<string, string>()
            {
                {"FrostDanger", "Sovuq xavfi" },
                {"Fallout", "Yog'ingarchilik"},
                {"Humidity", "Namlik"},
                {"SolarRadiationInfluence", "Quyosh nurlanishining ta'siri"},
                {"TemperatureAverage", "O'rtacha harorat"},
                {"TemperatureMax", "Maksimal harorat"},
                {"TemperatureMaxTotal", "Maksimal harorat jami"},
                {"TemperatureMin", "Harorat minimal"},
                {"TemperatureMinTotal", "Haroratning minimal jami"},
                {"NormalizedFallout", "Yog'ingarchilik normasi"},
                {"NormalizedHumidity", "Namlik normasi"},
                {"NormalizedSolarRadiationInfluence", "Quyosh nurlanishining ta'sir qilish normasi"},
                {"NormalizedTemperatureAverage", "Harorat o'rtacha normasi"},
                {"NormalizedTemperatureMax", "Maksimal harorat normasi"},
                {"NormalizedTemperatureMaxTotal", "Maksimal harorat normasi Jami"},
                {"NormalizedTemperatureMin", "Harorat normasi Min"},
                {"NormalizedTemperatureMinTotal", "Harorat normasi Minimal jami"},
                {"DeviationTemperatureAverage", "Harorat o'rtacha ko'rsatkichining normadan og'ishi"},
                {"DeviationTemperatureMax", "Maksimal haroratning normadan og'ishi"},
                {"DeviationTemperatureMaxTotal", "Haroratning maksimal umumiy qiymatining normadan og'ishi"},
                {"DeviationTemperatureMin", "Haroratning minimal ko'rsatkichining normadan og'ishi"},
                {"DeviationTemperatureMinTotal", "Haroratning minimal umumiy qiymatining normadan og'ishi"}
            };

            var frostDangerStartDate = DateOnly.Parse("2025-10-24");

            var eraStartDate = DateOnly.Parse("2025-09-30");
            var eraEndDate = DateOnly.Parse("2025-11-22");

            var ibmStartDate = DateOnly.Parse("2025-11-19");
            var ibmEndDate = DateOnly.Parse("2025-11-25");

            var ugmStartDate = DateOnly.Parse("2025-10-20");
            var ugmEndDate = DateOnly.Parse("2025-11-19");

            var indicesStartDate0 = DateOnly.Parse("2025-09-01");
            var indicesEndDate0 = DateOnly.Parse("2025-09-02");
            var indicesStartDate1 = DateOnly.Parse("2025-09-22");
            var indicesEndDate1 = DateOnly.Parse("2025-09-30");

            var sourceEra = "Era";
            var typeWeather = "Weather";
            var frostDanger = "FrostDanger";

            var pathStart = "/deploy/prod/data-geoserver-prod";

            //for (var sd = frostDangerStartDate; sd <= eraEndDate; sd = sd.AddDays(1))
            //{
            //    //var geoServerName = $"{sourceEra}{typeWeather}{frostDanger}{sd:yyyyMMdd}";
            //    var geoServerName = $"{sourceEra}{frostDanger}_{sd:yyyyMMdd}";
            //    var geoServerRelationPath = $"{pathStart}/{frostDanger}/{sd:yyyyMMdd}";

            //    var tileLayer = new TileLayer
            //    {
            //        Names = new LocalizationString
            //        {
            //            ["en-US"] = namesEn[frostDanger],
            //            ["ru-RU"] = namesRu[frostDanger],
            //            ["uz-Latn-UZ"] = namesUz[frostDanger]
            //        },
            //        GeoServerName = geoServerName,
            //        GeoServerRelationPath = geoServerRelationPath
            //    };
            //    await TileDbContext.TileLayers.AddAsync(tileLayer);
            //    Console.WriteLine($"{geoServerName}");
            //}

            List<string> indicators = ["Fallout", "Humidity", "SolarRadiationInfluence", "TemperatureAverage", "TemperatureMax", "TemperatureMaxTotal", "TemperatureMin", "TemperatureMinTotal"];

            //foreach (var indicator in indicators)
            //{
            //    for (var sd = eraStartDate; sd <= eraEndDate; sd = sd.AddDays(1))
            //    {
            //        //var geoServerName = $"{sourceEra}{typeWeather}{indicator}{sd:yyyyMMdd}";
            //        var geoServerName = $"{sourceEra}{indicator}_{sd:yyyyMMdd}";
            //        var geoServerRelationPath = $"{pathStart}/{indicator}/{sd:yyyyMMdd}";

            //        var tileLayer = new TileLayer
            //        {
            //            Names = new LocalizationString
            //            {
            //                ["en-US"] = namesEn[indicator],
            //                ["ru-RU"] = namesRu[indicator],
            //                ["uz-Latn-UZ"] = namesUz[indicator]
            //            },
            //            GeoServerName = geoServerName,
            //            GeoServerRelationPath = geoServerRelationPath
            //        };
            //        await TileDbContext.TileLayers.AddAsync(tileLayer);
            //        Console.WriteLine($"{geoServerName}");
            //    }
            //}


            //foreach (var indicator in indicators)
            //{
            //    for (var sd = ibmStartDate; sd <= ibmEndDate; sd = sd.AddDays(1))
            //    {
            //        //var geoServerName = $"{sourceEra}{typeWeather}{indicator}{sd:yyyyMMdd}";
            //        var geoServerName = $"Ibm{indicator}_{sd:yyyyMMdd}";
            //        var geoServerRelationPath = $"{pathStart}/Ibm{indicator}/{sd:yyyyMMdd}";

            //        var tileLayer = new TileLayer
            //        {
            //            Names = new LocalizationString
            //            {
            //                ["en-US"] = namesEn[indicator],
            //                ["ru-RU"] = namesRu[indicator],
            //                ["uz-Latn-UZ"] = namesUz[indicator]
            //            },
            //            GeoServerName = geoServerName,
            //            GeoServerRelationPath = geoServerRelationPath
            //        };
            //        await TileDbContext.TileLayers.AddAsync(tileLayer);
            //        Console.WriteLine($"{geoServerName}");
            //    }
            //}

            //for (var sd = ugmStartDate; sd <= ugmEndDate; sd = sd.AddDays(1))
            //{
            //    //var geoServerName = $"Ugm{typeWeather}TemperatureAverage{sd:yyyyMMdd}";
            //    var geoServerName = $"UgmTemperatureAverage_{sd:yyyyMMdd}";
            //    var geoServerRelationPath = $"{pathStart}/UgmTemperatureAverage/{sd:yyyyMMdd}";

            //    var tileLayer = new TileLayer
            //    {
            //        Names = new LocalizationString
            //        {
            //            ["en-US"] = namesEn["TemperatureAverage"],
            //            ["ru-RU"] = namesRu["TemperatureAverage"],
            //            ["uz-Latn-UZ"] = namesUz["TemperatureAverage"]
            //        },
            //        GeoServerName = geoServerName,
            //        GeoServerRelationPath = geoServerRelationPath
            //    };
            //    await TileDbContext.TileLayers.AddAsync(tileLayer);
            //    Console.WriteLine($"{geoServerName}");
            //}

            //List<string> normalizedIndicators = ["NormalizedFallout", "NormalizedHumidity", "NormalizedSolarRadiationInfluence", "NormalizedTemperatureAverage", "NormalizedTemperatureMax", "NormalizedTemperatureMaxTotal", "NormalizedTemperatureMin", "NormalizedTemperatureMinTotal"];
            //foreach (var indicator in normalizedIndicators)
            //{
            //    for (var i = 1013; i < 1028; i++)
            //    {
            //        var geoServerName = $"{sourceEra}{indicator}_{i}";
            //        var geoServerRelationPath = $"{pathStart}/{indicator}/{i}";

            //        var tileLayer = new TileLayer
            //        {
            //            Names = new LocalizationString
            //            {
            //                ["en-US"] = namesEn[indicator],
            //                ["ru-RU"] = namesRu[indicator],
            //                ["uz-Latn-UZ"] = namesUz[indicator]
            //            },
            //            GeoServerName = geoServerName,
            //            GeoServerRelationPath = geoServerRelationPath
            //        };
            //        await TileDbContext.TileLayers.AddAsync(tileLayer);
            //        Console.WriteLine($"{geoServerName}");
            //    }

            //    for (var i = 1109; i < 1123; i++)
            //    {
            //        var geoServerName = $"{sourceEra}{indicator}_{i}";
            //        var geoServerRelationPath = $"{pathStart}/{indicator}/{i}";

            //        var tileLayer = new TileLayer
            //        {
            //            Names = new LocalizationString
            //            {
            //                ["en-US"] = namesEn[indicator],
            //                ["ru-RU"] = namesRu[indicator],
            //                ["uz-Latn-UZ"] = namesUz[indicator]
            //            },
            //            GeoServerName = geoServerName,
            //            GeoServerRelationPath = geoServerRelationPath
            //        };
            //        await TileDbContext.TileLayers.AddAsync(tileLayer);
            //        Console.WriteLine($"{geoServerName}");
            //    }
            //}

            //List<string> deviationIndicators = ["DeviationTemperatureAverage", "DeviationTemperatureMax", "DeviationTemperatureMaxTotal", "DeviationTemperatureMin", "DeviationTemperatureMinTotal"];
            //foreach (var indicator in deviationIndicators)
            //{
            //    for (var i = 1013; i < 1028; i++)
            //    {
            //        var geoServerName = $"{sourceEra}{indicator}_2025{i}";
            //        var geoServerRelationPath = $"{pathStart}/{sourceEra}{indicator}/2025{i}";

            //        var tileLayer = new TileLayer
            //        {
            //            Names = new LocalizationString
            //            {
            //                ["en-US"] = namesEn[indicator],
            //                ["ru-RU"] = namesRu[indicator],
            //                ["uz-Latn-UZ"] = namesUz[indicator]
            //            },
            //            GeoServerName = geoServerName,
            //            GeoServerRelationPath = geoServerRelationPath
            //        };
            //        await TileDbContext.TileLayers.AddAsync(tileLayer);
            //        Console.WriteLine($"{geoServerName}");
            //    }

            //    for (var i = 1109; i < 1123; i++)
            //    {
            //        var geoServerName = $"{sourceEra}{indicator}_2025{i}";
            //        var geoServerRelationPath = $"{pathStart}/{sourceEra}{indicator}/2025{i}";

            //        var tileLayer = new TileLayer
            //        {
            //            Names = new LocalizationString
            //            {
            //                ["en-US"] = namesEn[indicator],
            //                ["ru-RU"] = namesRu[indicator],
            //                ["uz-Latn-UZ"] = namesUz[indicator]
            //            },
            //            GeoServerName = geoServerName,
            //            GeoServerRelationPath = geoServerRelationPath
            //        };
            //        await TileDbContext.TileLayers.AddAsync(tileLayer);
            //        Console.WriteLine($"{geoServerName}");
            //    }
            //}

            //List<string> indices = ["ARVI", "EVI", "GNDVI", "MNDWI", "NDMI", "NDVI", "ORVI", "OSAVI"];

            //foreach (var index in indices)
            //{
            //    for (var sd = indicesStartDate0; sd <= indicesEndDate0; sd = sd.AddDays(1))
            //    {
            //        //var geoServerName = $"{sourceEra}{typeWeather}{index}{sd:yyyyMMdd}";
            //        var geoServerName = $"{index}_{sd:yyyyMMdd}";
            //        var geoServerRelationPath = $"{pathStart}/{index}/{sd:yyyyMMdd}";

            //        var tileLayer = new TileLayer
            //        {
            //            Names = new LocalizationString
            //            {
            //                ["en-US"] = index,
            //                ["ru-RU"] = index,
            //                ["uz-Latn-UZ"] = index
            //            },
            //            GeoServerName = geoServerName,
            //            GeoServerRelationPath = geoServerRelationPath
            //        };
            //        await TileDbContext.TileLayers.AddAsync(tileLayer);
            //        Console.WriteLine($"{geoServerName}");
            //    }

            //    for (var sd = indicesStartDate1; sd <= indicesEndDate1; sd = sd.AddDays(1))
            //    {
            //        //var geoServerName = $"{sourceEra}{typeWeather}{index}{sd:yyyyMMdd}";
            //        var geoServerName = $"{index}_{sd:yyyyMMdd}";
            //        var geoServerRelationPath = $"{pathStart}/{index}/{sd:yyyyMMdd}";

            //        var tileLayer = new TileLayer
            //        {
            //            Names = new LocalizationString
            //            {
            //                ["en-US"] = index,
            //                ["ru-RU"] = index,
            //                ["uz-Latn-UZ"] = index
            //            },
            //            GeoServerName = geoServerName,
            //            GeoServerRelationPath = geoServerRelationPath
            //        };
            //        await TileDbContext.TileLayers.AddAsync(tileLayer);
            //        Console.WriteLine($"{geoServerName}");
            //    }
            //}

            //await TileDbContext.SaveChangesAsync();
        }

        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
