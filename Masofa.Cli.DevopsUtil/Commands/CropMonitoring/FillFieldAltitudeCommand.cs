using Masofa.Common.Models;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;

namespace Masofa.Cli.DevopsUtil.Commands.CropMonitoring
{
    [BaseCommand("Crop: Fill Field Altitude", "Заполнение высоты над уровнем моря для активных полей")]
    public class FillFieldAltitudeCommand : IBaseCommand
    {
        private readonly MasofaCropMonitoringDbContext _cropMonitoringDbContext;
        private readonly HttpClient _httpClient;

        public FillFieldAltitudeCommand(MasofaCropMonitoringDbContext cropMonitoringDbContext, HttpClient httpClient)
        {
            _cropMonitoringDbContext = cropMonitoringDbContext;
            _httpClient = httpClient;
        }

        public void Dispose()
        {
        }

        public async Task Execute()
        {
            Console.WriteLine("Запуск заполнения высоты над уровнем моря для полей...");

            var fields = await _cropMonitoringDbContext.Fields
                .Where(f => f.Status == StatusType.Active &&
                           (f.AltitudeAboveSeaLevel == null || f.AltitudeAboveSeaLevel == 0) &&
                           f.Polygon != null &&
                           !f.Polygon.IsEmpty)
                .ToListAsync();

            if (fields.Count == 0)
            {
                Console.WriteLine("Подходящих полей не найдено.");
                return;
            }

            const int batchSize = 1000;
            const int delayMinutes = 10;
            var totalBatches = (int)Math.Ceiling((double)fields.Count / batchSize);

            var totalUpdatedCount = 0;
            var totalErrorCount = 0;

            for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
            {
                var batchStart = batchIndex * batchSize;
                var batchEnd = Math.Min(batchStart + batchSize, fields.Count);
                var batch = fields.Skip(batchStart).Take(batchEnd - batchStart).ToList();

                var batchUpdatedCount = 0;
                var batchErrorCount = 0;

                foreach (var field in batch)
                {
                    try
                    {
                        if (field.Polygon == null || field.Polygon.IsEmpty || field.Polygon.Centroid == null)
                        {
                            batchErrorCount++;
                            continue;
                        }

                        var lat = field.Polygon.Centroid.Y;
                        var lon = field.Polygon.Centroid.X;

                        var elevation = await GetElevationAsync(lat, lon);

                        if (elevation.HasValue)
                        {
                            field.AltitudeAboveSeaLevel = elevation.Value;
                            field.LastUpdateAt = DateTime.UtcNow;
                            batchUpdatedCount++;
                        }
                        else
                        {
                            batchErrorCount++;
                        }
                    }
                    catch
                    {
                        batchErrorCount++;
                    }
                }

                if (batchUpdatedCount > 0)
                {
                    await _cropMonitoringDbContext.SaveChangesAsync();
                }

                totalUpdatedCount += batchUpdatedCount;
                totalErrorCount += batchErrorCount;

                if (batchIndex < totalBatches - 1)
                {
                    Console.WriteLine($"Пауза {delayMinutes} минут. Следующий батч начнётся в {DateTime.Now.AddMinutes(delayMinutes):HH:mm:ss}");
                    await Task.Delay(TimeSpan.FromMinutes(delayMinutes));
                }
            }

            Console.WriteLine($"Завершено. Обновлено полей: {totalUpdatedCount}, Ошибок: {totalErrorCount}");
        }

        private async Task<double?> GetElevationAsync(double lat, double lon)
        {
            try
            {
                var url = $"https://www.elevation-api.eu/v1/elevation/{lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}/{lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}";

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var jsonString = await response.Content.ReadAsStringAsync();

                if (double.TryParse(jsonString, NumberStyles.Float, CultureInfo.InvariantCulture, out var elev))
                {
                    return elev;
                }

                var jsonDoc = JsonDocument.Parse(jsonString);

                if (jsonDoc.RootElement.TryGetProperty("elevation", out var elevationElement))
                {
                    if (elevationElement.ValueKind == JsonValueKind.Number)
                    {
                        return elevationElement.GetDouble();
                    }
                }

                if (jsonDoc.RootElement.TryGetProperty("elevations", out var elevationsElement))
                {
                    if (elevationsElement.ValueKind == JsonValueKind.Array && elevationsElement.GetArrayLength() > 0)
                    {
                        return elevationsElement[0].GetDouble();
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public Task Execute(string[] args)
        {
            return Execute();
        }
    }
}

