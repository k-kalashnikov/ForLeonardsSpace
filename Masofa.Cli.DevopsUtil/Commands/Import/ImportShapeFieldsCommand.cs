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
//    public class ImportShapeFieldsCommandParameters
//    {
//        [TaskParameter("Путь к архиву (.zip) с ShapeFile", true, "C:\\data\\fields.zip")]
//        public string FilePath { get; set; } = string.Empty;

//        public static ImportShapeFieldsCommandParameters Parse(string[] args)
//        {
//            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
//                throw new ArgumentException("Необходимо указать путь к архиву");

//            return new ImportShapeFieldsCommandParameters { FilePath = args[0] };
//        }

//        public static ImportShapeFieldsCommandParameters GetFromUser()
//        {
//            Console.Write("Enter the path to archive (.zip) c ShapeFile:");
//            var filePath = (Console.ReadLine() ?? string.Empty).Trim().Trim('"');
//            return new ImportShapeFieldsCommandParameters { FilePath = filePath };
//        }
//    }

//    [BaseCommand("Shape: Import Fields", "Импорт полей из ShapeFile", typeof(ImportShapeFieldsCommandParameters))]
//    public class ImportShapeFieldsCommand : IBaseCommand
//    {
//        private IShapeImportService ShapeImportService {  get; set; }
//        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext {  get; set; }
        
//        public ImportShapeFieldsCommand(IShapeImportService shapeImportService, MasofaCropMonitoringDbContext masofaCropMonitoringDbContext) 
//        { 
//            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
//            ShapeImportService = shapeImportService;
//        }
        
//        public void Dispose()
//        {
            
//        }

//        public async Task Execute()
//        {
//            var parameters = ImportShapeFieldsCommandParameters.GetFromUser();
//            await ExecuteCore(parameters);
//        }

//        public async Task Execute(string[] args)
//        {
//            var parameters = ImportShapeFieldsCommandParameters.Parse(args);
//            await ExecuteCore(parameters);
//        }

//        private async Task ExecuteCore(ImportShapeFieldsCommandParameters parameters)
//        {
//            if (!File.Exists(parameters.FilePath))
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine("ERROR: File not found");
//                Console.ResetColor();
//                return;
//            }

//            var ext = Path.GetExtension(parameters.FilePath).ToLowerInvariant();
//            if (ext != ".zip")
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine("Only .zip with files is supported .shp/.dbf/.shx (etc.).");
//                Console.ResetColor();
//                return;
//            }

//            IReadOnlyList<Field> fields;
//            await using (var fs = File.OpenRead(parameters.FilePath))
//                fields = await ShapeImportService.ImportFieldsFromShapeZipAsync(fs);

//            foreach (var f in fields)
//            {
//                f.CreateAt = DateTime.UtcNow;
//                f.LastUpdateAt = DateTime.UtcNow;
//            }

//            try
//            {
//                await MasofaCropMonitoringDbContext.Fields.AddRangeAsync(fields);
//                var saved = await MasofaCropMonitoringDbContext.SaveChangesAsync();

//                Console.ForegroundColor = ConsoleColor.Green;
//                Console.WriteLine($"SUCCESS imported fields: {fields.Count}, saved lines: {saved}");
//            }
//            catch (DbUpdateException ex)
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine($"SAVE ERROR: {ex.Message}");
//            }
//            finally
//            {
//                Console.ResetColor();
//            }
//        }
//    }
//}
