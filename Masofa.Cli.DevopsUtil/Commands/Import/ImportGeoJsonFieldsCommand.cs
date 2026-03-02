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
//    public class ImportGeoJsonFieldsCommandParameters
//    {
//        [TaskParameter("Путь к файлу (.geojson/.json) или архиву (.zip)", true, "C:\\data\\fields.geojson")]
//        public string FilePath { get; set; } = string.Empty;

//        public static ImportGeoJsonFieldsCommandParameters Parse(string[] args)
//        {
//            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
//                throw new ArgumentException("Необходимо указать путь к файлу");

//            return new ImportGeoJsonFieldsCommandParameters { FilePath = args[0] };
//        }

//        public static ImportGeoJsonFieldsCommandParameters GetFromUser()
//        {
//            Console.WriteLine("Write path to file (.geojson/.json) or archive (.zip):");
//            var filePath = (Console.ReadLine() ?? string.Empty).Trim().Trim('"');
//            return new ImportGeoJsonFieldsCommandParameters { FilePath = filePath };
//        }
//    }

//    [BaseCommand("GeoJSON: Import Fields", "Импорт полей из GeoJSON файла", typeof(ImportGeoJsonFieldsCommandParameters))]
//    public class ImportGeoJsonFieldsCommand : IBaseCommand
//    {
//        private IGeoJsonImportService GeoJsonImportService { get; set; }
//        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }

//        public ImportGeoJsonFieldsCommand(IGeoJsonImportService geoJsonImportService, MasofaCropMonitoringDbContext masofaCropMonitoringDbContext)
//        {
//            GeoJsonImportService = geoJsonImportService;
//            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
//        }

//        public void Dispose()
//        {
//            throw new NotImplementedException();
//        }

//        public async Task Execute()
//        {
//            var parameters = ImportGeoJsonFieldsCommandParameters.GetFromUser();
//            await ExecuteCore(parameters);
//        }

//        public async Task Execute(string[] args)
//        {
//            var parameters = ImportGeoJsonFieldsCommandParameters.Parse(args);
//            await ExecuteCore(parameters);
//        }

//        private async Task ExecuteCore(ImportGeoJsonFieldsCommandParameters parameters)
//        {
//            if (!File.Exists(parameters.FilePath))
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine("File not found");
//                Console.ResetColor();
//                return;
//            }

//            IReadOnlyList<Field> fields;
//            var ext = Path.GetExtension(parameters.FilePath).ToLowerInvariant();

//            await using var fs = File.OpenRead(parameters.FilePath);
//            if (ext == ".zip")
//                fields = await GeoJsonImportService.ImportFieldsFromZipAsync(fs);
//            else if (ext == ".geojson" || ext == ".json")
//                fields = await GeoJsonImportService.ImportFieldsAsync(fs);
//            else
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine("Use only: .geojson/.json или .zip");
//                Console.ResetColor();
//                return;
//            }

//            foreach (var f in fields)
//            {
//                f.CreateAt = DateTime.UtcNow;
//                f.LastUpdateAt = DateTime.UtcNow;
//                // при необходимости – рассчитай/заполни FieldArea и пр.
//            }

//            try
//            {
//                await MasofaCropMonitoringDbContext.Fields.AddRangeAsync(fields);
//                var saved = await MasofaCropMonitoringDbContext.SaveChangesAsync();
//                Console.ForegroundColor = ConsoleColor.Green;
//                Console.WriteLine($"SUCCESS saved fields count: {fields.Count}, saved lines: {saved}");
//                Console.ResetColor();
//            }
//            catch (DbUpdateException ex)
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine($"SAVE ERROR: {ex.Message}");
//                Console.ResetColor();
//            }
//        }
//    }
//}
