//using Masofa.BusinessLogic.GeoIo;
//using Masofa.Common.Models;
//using Masofa.Common.Models.CropMonitoring;
//using Masofa.Common.Models.System;
//using Masofa.DataAccess;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Masofa.Cli.DevopsUtil.Commands.Import
//{
//    public class ImportGeoJsonSeasonsCommandParameters
//    {
//        [TaskParameter("FieldId (GUID) для импорта сезонов", true, "")]
//        public string FieldId { get; set; } = string.Empty;

//        [TaskParameter("Путь к файлу (.geojson/.json) или архиву (.zip)", true, "C:\\data\\seasons.geojson")]
//        public string FilePath { get; set; } = string.Empty;

//        public static ImportGeoJsonSeasonsCommandParameters Parse(string[] args)
//        {
//            if (args.Length < 2)
//                throw new ArgumentException("Неверные параметры. Используйте: <fieldId> <filePath>");

//            return new ImportGeoJsonSeasonsCommandParameters 
//            { 
//                FieldId = args[0], 
//                FilePath = args[1] 
//            };
//        }

//        public static ImportGeoJsonSeasonsCommandParameters GetFromUser()
//        {
//            Console.WriteLine("Укажите FieldId (GUID), в который загружаем сезоны:");
//            var fieldId = (Console.ReadLine() ?? string.Empty).Trim();

//            Console.WriteLine("Укажите путь к файлу (.geojson/.json) или архиву (.zip):");
//            var filePath = (Console.ReadLine() ?? string.Empty).Trim().Trim('"');

//            return new ImportGeoJsonSeasonsCommandParameters 
//            { 
//                FieldId = fieldId, 
//                FilePath = filePath 
//            };
//        }
//    }

//    [BaseCommand("GeoJSON: Import Seasons", "Импорт сезонов из GeoJSON файла", typeof(ImportGeoJsonSeasonsCommandParameters))]
//    public class ImportGeoJsonSeasonsCommand : IBaseCommand
//    {
//        private IGeoJsonImportService GeoJsonImportService { get; set; }
//        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }

//        public ImportGeoJsonSeasonsCommand(IGeoJsonImportService geoJsonImportService, MasofaCropMonitoringDbContext masofaCropMonitoringDbContext) 
//        {
//            GeoJsonImportService = geoJsonImportService;
//            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
//        }

//        public void Dispose()
//        {
            
//        }

//        public async Task Execute()
//        {
//            var parameters = ImportGeoJsonSeasonsCommandParameters.GetFromUser();
//            await ExecuteCore(parameters);
//        }

//        public async Task Execute(string[] args)
//        {
//            var parameters = ImportGeoJsonSeasonsCommandParameters.Parse(args);
//            await ExecuteCore(parameters);
//        }

//        private async Task ExecuteCore(ImportGeoJsonSeasonsCommandParameters parameters)
//        {
//            if (!Guid.TryParse(parameters.FieldId, out var fieldId))
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine("Некорректный GUID.");
//                Console.ResetColor();
//                return;
//            }

//            if (!File.Exists(parameters.FilePath))
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine("Файл не найден.");
//                Console.ResetColor();
//                return;
//            }

//            IReadOnlyList<Season> seasons;
//            var ext = Path.GetExtension(parameters.FilePath).ToLowerInvariant();

//            await using var fs = File.OpenRead(parameters.FilePath);
//            if (ext == ".zip")
//            {
//                seasons = await GeoJsonImportService.ImportSeasonsFromZipAsync(fs);
//            }
//            else if (ext == ".geojson" || ext == ".json")
//            {
//                seasons = await GeoJsonImportService.ImportSeasonsAsync(fieldId, fs);
//            }
//            else
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine("Supported: .geojson/.json or .zip");
//                Console.ResetColor();
//                return;
//            }

//            foreach (var s in seasons)
//            {
//                s.FieldId ??= fieldId;
//                s.CreateAt = DateTime.UtcNow;
//                s.LastUpdateAt = DateTime.UtcNow;
//            }

//            try
//            {
//                await MasofaCropMonitoringDbContext.Seasons.AddRangeAsync(seasons);
//                var saved = await MasofaCropMonitoringDbContext.SaveChangesAsync();
//                Console.ForegroundColor = ConsoleColor.Green;
//                Console.WriteLine($"SUCCESS Imported seasons: {seasons.Count}, сохранено записей: {saved}");
//                Console.ResetColor();
//            }
//            catch (DbUpdateException ex)
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine($"ERROR in SAVE: {ex.Message}");
//                Console.ResetColor();
//            }
//        }
//    }
//}
