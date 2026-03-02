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
//    public class ImportKmlSeasonsCommandParameters
//    {
//        [TaskParameter("FieldId (GUID) для сезонов", true, "")]
//        public string FieldId { get; set; } = string.Empty;

//        [TaskParameter("Путь к файлу (.kml/.kmz) или архиву (.zip)", true, "C:\\data\\seasons.kml")]
//        public string FilePath { get; set; } = string.Empty;

//        public static ImportKmlSeasonsCommandParameters Parse(string[] args)
//        {
//            if (args.Length < 2)
//                throw new ArgumentException("Неверные параметры. Используйте: <fieldId> <filePath>");

//            return new ImportKmlSeasonsCommandParameters 
//            { 
//                FieldId = args[0], 
//                FilePath = args[1] 
//            };
//        }

//        public static ImportKmlSeasonsCommandParameters GetFromUser()
//        {
//            Console.WriteLine("Enter FieldId (GUID) for season:");
//            var fieldId = (Console.ReadLine() ?? string.Empty).Trim();

//            Console.WriteLine("Enter path to file (.geojson/.json) or archive (.zip):");
//            var filePath = (Console.ReadLine() ?? string.Empty).Trim().Trim('"');

//            return new ImportKmlSeasonsCommandParameters 
//            { 
//                FieldId = fieldId, 
//                FilePath = filePath 
//            };
//        }
//    }

//    [BaseCommand("KML: Import Seasons (folder)", "Импорт сезонов из KML файла", typeof(ImportKmlSeasonsCommandParameters))]
//    public class ImportKmlSeasonsCommand : IBaseCommand
//    {
//        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
//        private IKmlImportService KmlImportService { get; set; }
        
//        public ImportKmlSeasonsCommand(MasofaCropMonitoringDbContext masofaCropMonitoringDbContext, IKmlImportService kmlImportService)
//        {
//            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
//            KmlImportService = kmlImportService;
//        }

//        public async Task Execute()
//        {
//            var parameters = ImportKmlSeasonsCommandParameters.GetFromUser();
//            await ExecuteCore(parameters);
//        }

//        public async Task Execute(string[] args)
//        {
//            var parameters = ImportKmlSeasonsCommandParameters.Parse(args);
//            await ExecuteCore(parameters);
//        }

//        private async Task ExecuteCore(ImportKmlSeasonsCommandParameters parameters)
//        {
//            if (!Guid.TryParse(parameters.FieldId, out var fieldId))
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine("Uncorrect GUID.");
//                Console.ResetColor();
//                return;
//            }

//            if (!File.Exists(parameters.FilePath))
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine("File not found");
//                Console.ResetColor();
//                return;
//            }

//            IReadOnlyList<Season> seasons;
//            var ext = Path.GetExtension(parameters.FilePath).ToLowerInvariant();

//            await using var fs = File.OpenRead(parameters.FilePath);
//            if (ext == ".zip")
//            {
//                seasons = await KmlImportService.ImportSeasonsAsyncFromZip(fs);
//            }
//            else if (ext == ".kml" || ext == ".kmz")
//            {
//                seasons = await KmlImportService.ImportSeasonsAsync(fieldId, fs);
//            }
//            else
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine("Supported: .kml/.kmz or .zip");
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
//                Console.WriteLine($"SUCCESS Imported seasons: {seasons.Count}, saved lines: {saved}");
//                Console.ResetColor();
//            }
//            catch (DbUpdateException ex)
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine($"ERROR in SAVE: {ex.Message}");
//                Console.ResetColor();
//            }
//        }

//        public void Dispose()
//        {
            
//        }
//    }
//}
