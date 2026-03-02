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
//    public class ExportKmlFieldCommandParameters
//    {
//        [TaskParameter("Список ID полей через запятую или путь к файлу (начинается с @)", true, "")]
//        public string FieldIds { get; set; } = string.Empty;

//        [TaskParameter("Путь для сохранения архива", false, "")]
//        public string OutputPath { get; set; } = string.Empty;

//        public static ExportKmlFieldCommandParameters Parse(string[] args)
//        {
//            if (args.Length == 0)
//                throw new ArgumentException("Необходимо указать список ID полей");

//            var parameters = new ExportKmlFieldCommandParameters { FieldIds = args[0] };
            
//            if (args.Length > 1)
//                parameters.OutputPath = args[1];

//            return parameters;
//        }

//        public static ExportKmlFieldCommandParameters GetFromUser()
//        {
//            Console.WriteLine("Enter a comma-separated list of field IDs or a file path (start with @):");
//            var fieldIds = (Console.ReadLine() ?? string.Empty).Trim();

//            Console.WriteLine($"Where to save the archive with KML fields? (Enter — current: {Environment.CurrentDirectory})");
//            var outputPath = (Console.ReadLine() ?? string.Empty).Trim().Trim('"');

//            return new ExportKmlFieldCommandParameters 
//            { 
//                FieldIds = fieldIds, 
//                OutputPath = outputPath 
//            };
//        }
//    }

//    [BaseCommand("KML: Export Field", "Экспорт полей в KML", typeof(ExportKmlFieldCommandParameters))]
//    public class ExportKmlFieldCommand : IBaseCommand
//    {
//        private IKmlExportService KmlExportService { get; set; }

//        public ExportKmlFieldCommand(IKmlExportService kmlExportService)
//        {
//            KmlExportService = kmlExportService;
//        }
        
//        public void Dispose()
//        {
            
//        }

//        public async Task Execute()
//        {
//            var parameters = ExportKmlFieldCommandParameters.GetFromUser();
//            await ExecuteCore(parameters);
//        }

//        public async Task Execute(string[] args)
//        {
//            var parameters = ExportKmlFieldCommandParameters.Parse(args);
//            await ExecuteCore(parameters);
//        }

//        private async Task ExecuteCore(ExportKmlFieldCommandParameters parameters)
//        {
//            var ids = ParseIdsOrFile(parameters.FieldIds).ToArray();
            
//            try
//            {
//                var zipBytes = await KmlExportService.ExportFieldsKmlZipAsync(ids);

//                var dir = string.IsNullOrWhiteSpace(parameters.OutputPath) ? Environment.CurrentDirectory : parameters.OutputPath;
//                dir = Path.GetFullPath(dir);
//                Directory.CreateDirectory(dir);

//                var outPath = GetUniquePath(dir, "fields_kml.zip");

//                await File.WriteAllBytesAsync(outPath, zipBytes);
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

//        private static IEnumerable<Guid> ParseIdsOrFile(string idsArg)
//        {
//            if (string.IsNullOrWhiteSpace(idsArg))
//                return Enumerable.Empty<Guid>();

//            if (idsArg.StartsWith("@"))
//            {
//                var path = idsArg.Trim().TrimStart('@').Trim('"');
//                var text = File.ReadAllText(path, System.Text.Encoding.UTF8);
//                return SplitGuids(text);
//            }

//            return SplitGuids(idsArg);
//        }

//        private static IEnumerable<Guid> SplitGuids(string text)
//        {
//            return text.Split(new[] { ',', ';', '\n', '\r', '\t', ' ' },
//                       StringSplitOptions.RemoveEmptyEntries)
//                .Select(s => s.Trim())
//                .Select(Guid.Parse);
//        }

//        private static string GetUniquePath(string dir, string fileName)
//        {
//            var path = Path.Combine(dir, fileName);
//            if (!File.Exists(path)) return path;

//            var name = Path.GetFileNameWithoutExtension(fileName);
//            var ext = Path.GetExtension(fileName);
//            var i = 1;
//            string candidate;
//            do
//            {
//                candidate = Path.Combine(dir, $"{name} ({i++}){ext}");
//            } while (File.Exists(candidate));
//            return candidate;
//        }
//    }
//}
