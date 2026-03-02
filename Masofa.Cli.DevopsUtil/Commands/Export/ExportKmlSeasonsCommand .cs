//using Masofa.BusinessLogic.GeoIo;
//using Masofa.Common.Models;
//using Masofa.Common.Models.System;
//using Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Masofa.Cli.DevopsUtil.Commands.Export
//{
//    public class ExportKmlSeasonsCommandParameters
//    {
//        [TaskParameter("Список ID полей через запятую или путь к файлу", true, "")]
//        public string FieldIds { get; set; } = string.Empty;

//        [TaskParameter("Имя выходного файла", false, "")]
//        public string OutputFileName { get; set; } = string.Empty;

//        public static ExportKmlSeasonsCommandParameters Parse(string[] args)
//        {
//            if (args.Length == 0)
//                throw new ArgumentException("Необходимо указать список ID полей");

//            var parameters = new ExportKmlSeasonsCommandParameters { FieldIds = args[0] };
            
//            if (args.Length > 1)
//                parameters.OutputFileName = args[1];

//            return parameters;
//        }

//        public static ExportKmlSeasonsCommandParameters GetFromUser()
//        {
//            Console.WriteLine("== Export Seasons → KML (zip) ==");
//            Console.WriteLine("Insert GUID's of Fields use comma or path to .txt с GUID's by lines:");
//            var fieldIds = Console.ReadLine() ?? string.Empty;

//            Console.WriteLine("Specify the output file name (default is seasons_kml_{timestamp}.zip):");
//            var outputFileName = Console.ReadLine() ?? string.Empty;

//            return new ExportKmlSeasonsCommandParameters 
//            { 
//                FieldIds = fieldIds, 
//                OutputFileName = outputFileName 
//            };
//        }
//    }

//    [BaseCommand("KML: Export Season", "Экспорт сезонов в KML", typeof(ExportKmlSeasonsCommandParameters))]
//    public class ExportKmlSeasonsCommand : IBaseCommand
//    {
//        private IKmlExportService KmlExportService { get; set; }
        
//        public ExportKmlSeasonsCommand(IKmlExportService kmlExportService)
//        {
//            KmlExportService = kmlExportService;
//        }

//        public void Dispose()
//        {

//        }

//        public async Task Execute()
//        {
//            var parameters = ExportKmlSeasonsCommandParameters.GetFromUser();
//            await ExecuteCore(parameters);
//        }

//        public async Task Execute(string[] args)
//        {
//            var parameters = ExportKmlSeasonsCommandParameters.Parse(args);
//            await ExecuteCore(parameters);
//        }

//        private async Task ExecuteCore(ExportKmlSeasonsCommandParameters parameters)
//        {
//            var fieldIds = ReadIds(parameters.FieldIds);
//            if (fieldIds.Count == 0)
//            {
//                Console.BackgroundColor = ConsoleColor.Yellow;
//                Console.WriteLine("GUID's not recognized");
//                Console.ResetColor();
//                return;
//            }

//            var outName = string.IsNullOrWhiteSpace(parameters.OutputFileName) 
//                ? $"seasons_kml_{DateTime.UtcNow:yyyyMMddHHmmss}.zip" 
//                : parameters.OutputFileName;

//            try
//            {
//                var bytes = await KmlExportService.ExportSeasonsKmlZipAsync(fieldIds);
//                await File.WriteAllBytesAsync(outName, bytes);

//                Console.ForegroundColor = ConsoleColor.Green;
//                Console.WriteLine($"SUCCESS: {outName} ({bytes.Length} bytes).");
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

//        private static List<Guid> ReadIds(string? input)
//        {
//            var list = new List<Guid>();
//            if (string.IsNullOrWhiteSpace(input))
//                return list;

//            if (File.Exists(input))
//            {
//                foreach (var line in File.ReadAllLines(input))
//                {
//                    if (Guid.TryParse(line.Trim(), out var id))
//                    {
//                        list.Add(id);
//                    }
//                }
//                return list;
//            }

//            foreach (var token in input.Split(',', ';', ' ', '\t', '\n', '\r'))
//            {
//                if (Guid.TryParse(token.Trim(), out var id))
//                {
//                    list.Add(id);
//                } 
//            }

//            return list;
//        }
//    }
//}
