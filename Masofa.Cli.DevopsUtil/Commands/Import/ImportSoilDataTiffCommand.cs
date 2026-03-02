using Masofa.Common.Attributes;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MaxRev.Gdal.Core;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using OSGeo.GDAL;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;

namespace Masofa.Cli.DevopsUtil.Commands.Import
{
    public class ImportSoilDataTiffCommandParameters
    {
        [TaskParameter("Путь к Тиф файлу для почвы", true, "C:\\data\\seasons.zip")]
        public string FilePath { get; set; } = string.Empty;

        [TaskParameter("Путь к ZIP-файлу в MinIO (почва)", true, "soil-data/uzbekistan_soil.zip")]
        public string RemoteZipPath { get; set; } = string.Empty;

        [TaskParameter("Имя бакета", false, "soildatas")]
        public string Bucket { get; set; } = "soildatas";

        public static ImportSoilDataTiffCommandParameters Parse(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
                throw new ArgumentException("Необходимо указать путь к ZIP в MinIO");

            var bucket = args.Length > 1 ? args[1] : "develop";
            return new ImportSoilDataTiffCommandParameters
            {
                RemoteZipPath = args[0],
                Bucket = bucket
            };
            //if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
            //    throw new ArgumentException("Необходимо указать путь к архиву");

            //return new ImportSoilDataTiffCommandParameters { FilePath = args[0] };
        }

        public static ImportSoilDataTiffCommandParameters GetFromUser()
        {
            Console.WriteLine("Укажите путь к ZIP-файлу в MinIO (например: soil-data/uzbekistan.zip):");
            var remotePath = (Console.ReadLine() ?? string.Empty).Trim().Trim('"');
            Console.WriteLine("Укажите имя бакета (по умолчанию 'develop'):");
            var bucket = (Console.ReadLine() ?? "develop").Trim() ?? "develop";
            return new ImportSoilDataTiffCommandParameters
            {
                RemoteZipPath = remotePath,
                Bucket = bucket
            };
            //Console.WriteLine("Укажите путь к файлу c Tiff (почва):");
            //var filePath = (Console.ReadLine() ?? string.Empty).Trim().Trim('"');
            //return new ImportSoilDataTiffCommandParameters { FilePath = filePath };
        }
    }

    [BaseCommand("Tiff: Import SoilData", "Импорт данных почвы из Tiff", typeof(ImportSoilDataTiffCommandParameters))]
    public class ImportSoilDataTiffCommand : IBaseCommand
    {
        private IFileStorageProvider FileStorageProvider { get; set; }
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext {  get; set; }
        public ImportSoilDataTiffCommand(MasofaCropMonitoringDbContext masofaCropMonitoringDbContext, IFileStorageProvider fileStorageProvider)
        {
            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
            FileStorageProvider = fileStorageProvider;
        }
        public void Dispose()
        {
            
        }

        public async Task Execute()
        {
            var parameters = ImportSoilDataTiffCommandParameters.GetFromUser();
            await ExecuteCore(parameters);
        }

        public async Task Execute(string[] args)
        {
            var parameters = ImportSoilDataTiffCommandParameters.Parse(args);
            await ExecuteCore(parameters);
        }

        public async Task ExecuteCore(ImportSoilDataTiffCommandParameters parameters)
        {
            Console.WriteLine($"Начало импорта из MinIO: {parameters.RemoteZipPath} (бакет: {parameters.Bucket})");
            Console.WriteLine($"Start time: [{DateTime.Now:HH:mm}]");

            var tempDir = await DownloadAndExtractZipAsync(parameters.RemoteZipPath, parameters.Bucket);

            try
            {
                var tifFiles = Directory.GetFiles(tempDir, "*.tif", SearchOption.AllDirectories);
                Console.WriteLine($"Найдено TIF-файлов: {tifFiles.Length}");

                foreach (var tifFile in tifFiles)
                {
                    await ProcessTiffFileAsync(tifFile);
                }

                Console.WriteLine($"Импорт завершён. Всего файлов: {tifFiles.Length}");
            }
            finally
            {
                // Удаляем временную папку
                try { Directory.Delete(tempDir, true); } catch { }
            }

            Console.WriteLine($"End time: [{DateTime.Now:HH:mm}]");
        }

        private static string GetUnitForParameter(string param)
        {
            return param switch
            {
                "sand" => "g/kg",
                "silt" => "g/kg",
                "clay" => "g/kg",
                "phh2o" => "pH",
                "cec" => "cmol(+)/kg",
                "soc" => "g/kg",
                "bdod" => "kg/m³",
                "cfvo" => "% v/v",
                "nitrogen" => "g/kg",
                "humus" => "g/kg",
                "phosphorus" => "mg/kg",
                "salinity" => "dS/m",
                _ => "unknown"
            };
        }

        private static (string parameter, string depth)? ParseFilename(string filename)
        {
            var match = Regex.Match(filename, @"^([a-z0-9_]+)_(\d+-\d+cm)_[^_]+_uzbekistan_wgs84\.tif$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return (match.Groups[1].Value, match.Groups[2].Value);
            }
            return null;
        }

        private async Task<string> DownloadAndExtractZipAsync(string remoteZipPath, string bucket)
        {
            var tempDir = Path.Combine(Environment.CurrentDirectory, $"soil_import_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}");
            Directory.CreateDirectory(tempDir);
            Console.WriteLine($"Временная папка создана: {tempDir}");

            try
            {
                var zipFilePath = Path.Combine(tempDir, "archive.zip");
                using (var remoteStream = await FileStorageProvider.GetFileStreamAsync(remoteZipPath, bucket))
                using (var fileStream = File.Create(zipFilePath))
                {
                    await remoteStream.CopyToAsync(fileStream);
                }

                Console.WriteLine($"ZIP скачан: {zipFilePath}");

                ZipFile.ExtractToDirectory(zipFilePath, tempDir, overwriteFiles: true);
                Console.WriteLine($"ZIP распакован в: {tempDir}");

                File.Delete(zipFilePath);

                return tempDir;
            }
            catch
            {
                try { Directory.Delete(tempDir, true); } catch { }
                throw;
            }

        }

        private async Task ProcessTiffFileAsync(string tifPath)
        {
            Console.WriteLine($"Обработка файла: {Path.GetFileName(tifPath)}");

            var parsed = ParseFilename(Path.GetFileName(tifPath));
            if (parsed == null)
            {
                Console.WriteLine($"Пропуск: не распознано имя файла {tifPath}");
                return;
            }

            var (parameter, depth) = parsed.Value;
            var unit = GetUnitForParameter(parameter);

            GdalBase.ConfigureAll();

            using var dataset = Gdal.Open(tifPath, Access.GA_ReadOnly);
            if (dataset == null) throw new Exception($"Не удалось открыть файл: {tifPath}");

            var geoTransform = new double[6];
            dataset.GetGeoTransform(geoTransform);

            int width = dataset.RasterXSize;
            int height = dataset.RasterYSize;

            var band = dataset.GetRasterBand(1);
            double noDataValue;
            int hasNoData;
            band.GetNoDataValue(out noDataValue, out hasNoData);
            double? noData = hasNoData != 0 ? noDataValue : null;

            var buffer = new float[width * height];
            band.ReadRaster(0, 0, width, height, buffer, width, height, 0, 0);

            var entities = new List<SoilData>();

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    var value = buffer[row * width + col];
                    if (noData.HasValue && Math.Abs(value - noData.Value) < 1e-6) continue;
                    if (float.IsNaN(value) || float.IsInfinity(value)) continue;

                    double lon = geoTransform[0] + col * geoTransform[1] + row * geoTransform[2];
                    double lat = geoTransform[3] + col * geoTransform[4] + row * geoTransform[5];
                    if (lon < 55 || lon > 75 || lat < 37 || lat > 46) continue;

                    var point = new Point(lon, lat) { SRID = 4326 };

                    var tileLon = FloorToStep(lon, 0.25);
                    var tileLat = FloorToStep(lat, 0.25);
                    var tileKey = $"SoilData_{tileLon:F2}_{tileLat:F2}";

                    if (tileLon < 56.0 || tileLon > 72.75 || tileLat < 37.0 || tileLat > 45.25)
                        continue;

                    if (value == 0)
                    {
                        continue;
                    }

                    double finalValue = value;

                    if (parameter == "phh2o")
                    {
                        finalValue = value / 10.0;
                    }

                    entities.Add(new SoilData
                    {
                        Point = point,
                        Parameter = parameter,
                        DepthRange = depth,
                        Value = finalValue,
                        Unit = unit,
                        Source = Path.GetFileName(tifPath),
                        TileKey = tileKey,
                        CreateUser = Guid.Empty,
                        LastUpdateUser = Guid.Empty
                    });
                }
            }

            //await MasofaCropMonitoringDbContext.Database.ExecuteSqlRawAsync(
            //    @"DELETE FROM ""SoilDatas"" WHERE ""Source"" = {0}", Path.GetFileName(tifPath));

            const int batchSize = 200_000;
            var total = 0;
            foreach (var chunk in entities.Chunk(batchSize))
            {
                try
                {
                    await MasofaCropMonitoringDbContext.SoilDatas.AddRangeAsync(chunk.ToList());
                    await MasofaCropMonitoringDbContext.SaveChangesAsync();
                    MasofaCropMonitoringDbContext.ChangeTracker.Clear();
                    total += chunk.Count();
                    Console.WriteLine($"  → {Path.GetFileName(tifPath)}: {total}/{entities.Count} [{DateTime.Now:HH:mm}]");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            Console.WriteLine($"Завершено: {Path.GetFileName(tifPath)} ({entities.Count} записей)");
        }

        private static double FloorToStep(double value, double step = 0.25)
        {
            return Math.Floor(value / step) * step;
        }
    }
}
