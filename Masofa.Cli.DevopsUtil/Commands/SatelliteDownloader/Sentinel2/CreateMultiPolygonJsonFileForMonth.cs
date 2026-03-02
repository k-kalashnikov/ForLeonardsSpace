using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Cli.DevopsUtil.Commands.SatelliteDownloader.Sentinel2
{
    [BaseCommand("Create multipolygon Json File for 1 month tiles", "Create multipolygon Json File for 1 month tiles")]
    public class CreateMultiPolygonJsonFileForMonth : IBaseCommand
    {
        private MasofaIndicesDbContext MasofaIndicesDbContext { get; set; }
        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
        public CreateMultiPolygonJsonFileForMonth(MasofaIndicesDbContext masofaIndicesDbContext, MasofaCommonDbContext masofaCommonDbContext)
        {
            MasofaIndicesDbContext = masofaIndicesDbContext;
            MasofaCommonDbContext = masofaCommonDbContext;
        }
        public void Dispose()
        {
            
        }

        public Task Execute()
        {
            return Execute(Array.Empty<string>());
            
        }

        public async Task Execute(string[] args)
        {
            await CreateExecute();
        }

        public async Task CreateExecute()
        {
            var endDate = DateTime.UtcNow.Date;
            var startDate = endDate.AddDays(-30);
            string exportDirectory = @"D:\MasofaExports";

            var polygons = await MasofaCommonDbContext.SatelliteProducts
                .Where(p => p.OriginDate.HasValue &&
                            p.OriginDate.Value >= startDate &&
                            p.OriginDate.Value < endDate &&
                            p.Polygon != null)
                .Select(p => p.Polygon)
                .ToListAsync();

            if (!polygons.Any())
            {
                Console.WriteLine("Нет снимков за указанный период.");
                return;
            }

            var geometryFactory = MasofaCommonDbContext.Database.GetService<GeometryFactory>()
                                   ?? NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory();

            var multiPolygon = geometryFactory.CreateMultiPolygon(polygons.ToArray());

            var geoJsonWriter = new GeoJsonWriter();
            var geoJson = geoJsonWriter.Write(multiPolygon);

            string fileName = $"satellite_multipolygon_{startDate:yyyy-MM-dd}_to_{endDate:yyyy-MM-dd}.geojson";
            string fullPath = Path.Combine(exportDirectory, fileName);

            try
            {
                await File.WriteAllTextAsync(fullPath, geoJson, Encoding.UTF8);
                Console.WriteLine($"Файл успешно сохранён: {fullPath}");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Ошибка доступа: нет прав на запись в {exportDirectory}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка записи файла: {ex.Message}");
                throw;
            }
        }
    }
}
