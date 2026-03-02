using Masofa.BusinessLogic.Services;
using Masofa.BusinessLogic.WeatherReport;
using Masofa.Common.Models.SystemCrical;
using MediatR;

namespace Masofa.Cli.DevopsUtil.Commands.Weather
{
    [BaseCommand("Republish Old Weather Layers", "Republish Old Weather Layers")]
    public class RepublishOldWeatherLayers : IBaseCommand
    {
        private GeoServerService GeoServerService { get; set; }
        private IMediator Mediator { get; set; }

        public RepublishOldWeatherLayers(IMediator mediator, GeoServerService geoServerService)
        {
            Mediator = mediator;
            GeoServerService = geoServerService;
        }

        public void Dispose()
        {
            Console.WriteLine("\nRepublishOldWeatherLayers END");
        }

        public async Task Execute()
        {
            Console.WriteLine("RepublishOldWeatherLayers START");

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
            var sourceIbm = "Ibm";
            var typeWeather = "Weather";
            var frostDanger = "FrostDanger";

            var pathStart = "/deploy/prod/data-geoserver-prod";

            List<string> indicators = ["Fallout", "Humidity", "SolarRadiationInfluence", "TemperatureAverage", "TemperatureMax", "TemperatureMaxTotal", "TemperatureMin", "TemperatureMinTotal"];

            foreach (var indicator in indicators)
            {
                for (var sd = eraStartDate; sd <= eraEndDate; sd = sd.AddDays(1))
                {
                    var geoServerName = $"{sourceEra}{typeWeather}{indicator}{sd:yyyyMMdd}";
                    var path = $"{indicator}/{sd:yyyyMMdd}";

                    await Process(indicator, geoServerName, path);
                }
            }

            foreach (var indicator in indicators)
            {
                for (var sd = ibmStartDate; sd <= ibmEndDate; sd = sd.AddDays(1))
                {
                    var geoServerName = $"{sourceIbm}{typeWeather}{indicator}{sd:yyyyMMdd}";
                    var path = $"{indicator}/{sd:yyyyMMdd}";

                    await Process(indicator, geoServerName, path);
                }
            }

            for (var sd = frostDangerStartDate; sd <= eraEndDate; sd = sd.AddDays(1))
            {
                var geoServerName = $"{sourceEra}{typeWeather}{frostDanger}{sd:yyyyMMdd}";
                var path = $"{frostDanger}/{sd:yyyyMMdd}";

                await Process(frostDanger, geoServerName, path);
            }

            for (var sd = ugmStartDate; sd <= ugmEndDate; sd = sd.AddDays(1))
            {
                var geoServerName = $"Ugm{typeWeather}TemperatureAverage{sd:yyyyMMdd}";
                var path = $"UgmTemperatureAverage/{sd:yyyyMMdd}";

                await Process("TemperatureAverage", geoServerName, path);
            }

            List<string> normalizedIndicators = ["NormalizedFallout", "NormalizedHumidity", "NormalizedSolarRadiationInfluence", "NormalizedTemperatureAverage", "NormalizedTemperatureMax", "NormalizedTemperatureMaxTotal", "NormalizedTemperatureMin", "NormalizedTemperatureMinTotal"];
            foreach (var indicator in normalizedIndicators)
            {
                for (var i = 1013; i < 1028; i++)
                {
                    var geoServerName = $"{sourceEra}{indicator}_{i}";
                    var path = $"{indicator}/{i}";

                    await Process(indicator, geoServerName, path);
                }

                for (var i = 1109; i < 1123; i++)
                {
                    var geoServerName = $"{sourceEra}{indicator}_{i}";
                    var path = $"{indicator}/{i}";

                    await Process(indicator, geoServerName, path);
                }
            }

            List<string> deviationIndicators = ["DeviationTemperatureAverage", "DeviationTemperatureMax", "DeviationTemperatureMaxTotal", "DeviationTemperatureMin", "DeviationTemperatureMinTotal"];
            foreach (var indicator in deviationIndicators)
            {
                for (var i = 1013; i < 1028; i++)
                {
                    var geoServerName = $"{sourceEra}{indicator}_{i}";
                    var path = $"{indicator}/{i}";

                    await Process(indicator, geoServerName, path);
                }

                for (var i = 1109; i < 1123; i++)
                {
                    var geoServerName = $"{sourceEra}{indicator}_{i}";
                    var path = $"{indicator}/{i}";

                    await Process(indicator, geoServerName, path);
                }
            }
        }

        private async Task Process(string indicator, string geoServerName, string path)
        {
            var storeRes = await GeoServerService.RecreateImageMosaicStoreAsync(geoServerName, path);
            var layerRes = false;
            if (storeRes)
            {
                layerRes = await GeoServerService.PublishCoverageAsync(geoServerName, geoServerName);
            }

            if (layerRes)
            {
                var request = new TileLayerCreateCommand()
                {
                    Indicator = indicator,
                    LayerName = geoServerName,
                    RelativePath = path
                };

                await Mediator.Send(request);
            }
            Console.WriteLine($"{geoServerName} - storeRes: {storeRes}, layerRes: {layerRes}");
        }

        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
