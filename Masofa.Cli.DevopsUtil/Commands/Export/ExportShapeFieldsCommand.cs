//using Masofa.BusinessLogic.GeoIo;
//using Masofa.Common.Models;
//using Masofa.Common.Models.System;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Masofa.Cli.DevopsUtil.Commands.Export
//{
//    public class ExportShapeFieldsCommandParameters
//    {
//        [TaskParameter("Список ID полей через запятую или путь к файлу (начинается с @)", true, "")]
//        public string FieldIds { get; set; } = string.Empty;

//        [TaskParameter("Папка для сохранения (абсолютный путь)", false, "")]
//        public string OutputDirectory { get; set; } = string.Empty;

//        public static ExportShapeFieldsCommandParameters Parse(string[] args)
//        {
//            if (args.Length == 0)
//                throw new ArgumentException("Необходимо указать список ID полей");

//            var parameters = new ExportShapeFieldsCommandParameters { FieldIds = args[0] };
            
//            if (args.Length > 1)
//                parameters.OutputDirectory = args[1];

//            return parameters;
//        }

//        public static ExportShapeFieldsCommandParameters GetFromUser()
//        {
//            Console.Write("Field ids (comma or space separated, or @path.txt): ");
//            var fieldIds = Console.ReadLine() ?? string.Empty;

//            Console.Write("Save folder (absolute): ");
//            var outputDirectory = Console.ReadLine()?.Trim() ?? string.Empty;

//            return new ExportShapeFieldsCommandParameters 
//            { 
//                FieldIds = fieldIds, 
//                OutputDirectory = outputDirectory 
//            };
//        }
//    }

//    [BaseCommand("Shape: Export Fields", "Экспорт полей в ShapeFile", typeof(ExportShapeFieldsCommandParameters))]
//    public class ExportShapeFieldsCommand : IBaseCommand
//    {
//        private IShapeExportService ShapeExportService { get; set; }
        
//        public ExportShapeFieldsCommand(IShapeExportService shapeExportService) 
//        { 
//            ShapeExportService = shapeExportService;
//        }
        
//        public void Dispose()
//        {
           
//        }

//        public async Task Execute()
//        {
//            var parameters = ExportShapeFieldsCommandParameters.GetFromUser();
//            await ExecuteCore(parameters);
//        }

//        public async Task Execute(string[] args)
//        {
//            var parameters = ExportShapeFieldsCommandParameters.Parse(args);
//            await ExecuteCore(parameters);
//        }

//        private async Task ExecuteCore(ExportShapeFieldsCommandParameters parameters)
//        {
//            var ids = ReadIds(parameters.FieldIds);

//            var outDir = string.IsNullOrWhiteSpace(parameters.OutputDirectory) 
//                ? Directory.GetCurrentDirectory() 
//                : parameters.OutputDirectory;

//            Directory.CreateDirectory(outDir);
//            try
//            {
//                var zip = await ShapeExportService.ExportFieldsShapeZipAsync(ids);
//                var outPath = Path.Combine(outDir, $"fields_shapefile_{DateTime.UtcNow:yyyyMMdd_HHmmss}.zip");
//                await File.WriteAllBytesAsync(outPath, zip);

//                Console.ForegroundColor = ConsoleColor.Green;
//                Console.WriteLine($"SUCCESS: {outPath}");
//                Console.ResetColor();
//            }
//            catch (Exception ex)
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine($"ERROR: {ex.Message}");
//                Console.WriteLine($"INNER: {ex?.InnerException?.Message}");
//                Console.ResetColor();
//            }
//        }

//        private static IEnumerable<Guid> ReadIds(string? input)
//        {
//            if (string.IsNullOrWhiteSpace(input)) return Enumerable.Empty<Guid>();
//            input = input.Trim();
//            if (input.StartsWith("@"))
//            {
//                var text = File.ReadAllText(input[1..]);
//                return SplitGuids(text);
//            }
//            return SplitGuids(input);
//        }

//        private static IEnumerable<Guid> SplitGuids(string text)
//        {
//            return text.Split(new[] { ',', ';', '\n', '\r', '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries)
//               .Select(s => Guid.TryParse(s, out var id) ? id : Guid.Empty)
//               .Where(id => id != Guid.Empty)
//               .ToArray();
//        }
//    }
//}
