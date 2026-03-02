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
//    public class ExportGeoJsonSeasonsCommandParameters
//    {
//        [TaskParameter("Список ID полей через запятую или путь к файлу (начинается с @)", true, "")]
//        public string FieldIds { get; set; } = string.Empty;

//        [TaskParameter("Путь для сохранения архива", false, "")]
//        public string OutputPath { get; set; } = string.Empty;

//        public static ExportGeoJsonSeasonsCommandParameters Parse(string[] args)
//        {
//            if (args.Length == 0)
//                throw new ArgumentException("Необходимо указать список ID полей");

//            var parameters = new ExportGeoJsonSeasonsCommandParameters { FieldIds = args[0] };
            
//            if (args.Length > 1)
//                parameters.OutputPath = args[1];

//            return parameters;
//        }

//        public static ExportGeoJsonSeasonsCommandParameters GetFromUser()
//        {
//            Console.WriteLine("Enter a list of FIELD IDs (for unloading seasons) separated by commas " +
//                              "or file path (start with @):");
//            var fieldIds = (Console.ReadLine() ?? string.Empty).Trim();

//            Console.WriteLine($"Where to save the archive with GeoJSON seasons? " +
//                              $"(Enter — current: {Environment.CurrentDirectory})");
//            var outputPath = (Console.ReadLine() ?? string.Empty).Trim().Trim('"');

//            return new ExportGeoJsonSeasonsCommandParameters 
//            { 
//                FieldIds = fieldIds, 
//                OutputPath = outputPath 
//            };
//        }
//    }

//    [BaseCommand("GeoJson: Export Season", "Экспорт сезонов в GeoJSON", typeof(ExportGeoJsonSeasonsCommandParameters))]
//    public class ExportGeoJsonSeasonsCommand : IBaseCommand
//    {
//        private IGeoJsonExportService GeoJsonExportService { get; set; }
        
//        public ExportGeoJsonSeasonsCommand(IGeoJsonExportService geoJsonExportService) 
//        { 
//            GeoJsonExportService = geoJsonExportService;
//        }
        
//        public void Dispose()
//        {
            
//        }

//        public async Task Execute()
//        {
//            var parameters = ExportGeoJsonSeasonsCommandParameters.GetFromUser();
//            await ExecuteCore(parameters);
//        }

//        public async Task Execute(string[] args)
//        {
//            var parameters = ExportGeoJsonSeasonsCommandParameters.Parse(args);
//            await ExecuteCore(parameters);
//        }

//        private async Task ExecuteCore(ExportGeoJsonSeasonsCommandParameters parameters)
//        {
//            var ids = ParseIdsOrFile(parameters.FieldIds).ToArray();

//            try
//            {
//                var zipBytes = await GeoJsonExportService.ExportSeasonsGeoJsonZipAsync(ids);

//                var dir = string.IsNullOrWhiteSpace(parameters.OutputPath) ? Environment.CurrentDirectory : parameters.OutputPath;
//                dir = Path.GetFullPath(dir);
//                Directory.CreateDirectory(dir);

//                var fileName = "seasons_geojson.zip";
//                var outPath = Path.Combine(dir, fileName);

//                if (File.Exists(outPath))
//                {
//                    var name = Path.GetFileNameWithoutExtension(outPath);
//                    var ext = Path.GetExtension(outPath);
//                    var i = 1;
//                    while (File.Exists(outPath))
//                    {
//                        outPath = Path.Combine(dir, $"{name} ({i++}){ext}");
//                    }
//                }

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

//        private static IEnumerable<Guid> ParseIdsOrFile(string? idsArg)
//        {
//            if (string.IsNullOrWhiteSpace(idsArg))
//                return Enumerable.Empty<Guid>();

//            if (idsArg.StartsWith("@"))
//            {
//                var path = idsArg.TrimStart('@').Trim('"');
//                var text = File.ReadAllText(path, Encoding.UTF8);
//                return SplitGuids(text);
//            }

//            return SplitGuids(idsArg);
//        }

//        private static IEnumerable<Guid> SplitGuids(string s)
//        {
//            return s.Split(new[] { ',', ';', '\n', '\r', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
//         .Select(x => Guid.TryParse(x.Trim(), out var g) ? g : (Guid?)null)
//         .Where(g => g.HasValue)
//         .Select(g => g!.Value);
//        }
//    }
//}
