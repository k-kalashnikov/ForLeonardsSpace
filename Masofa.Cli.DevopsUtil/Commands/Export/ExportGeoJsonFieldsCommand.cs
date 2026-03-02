//using Masofa.Common.Models;
//using Masofa.Common.Models.System;
//using Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;
//using SharpKml.Dom;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

//namespace Masofa.Cli.DevopsUtil.Commands.Export
//{
//    public class ExportGeoJsonFieldsCommandParameters
//    {
//        [TaskParameter("Список ID полей через запятую или путь к файлу (начинается с @)", true, "")]
//        public string FieldIds { get; set; } = string.Empty;

//        [TaskParameter("Путь для сохранения архива", false, "")]
//        public string OutputPath { get; set; } = string.Empty;

//        public static ExportGeoJsonFieldsCommandParameters Parse(string[] args)
//        {
//            if (args.Length == 0)
//                throw new ArgumentException("Необходимо указать список ID полей");

//            var parameters = new ExportGeoJsonFieldsCommandParameters { FieldIds = args[0] };
            
//            if (args.Length > 1)
//                parameters.OutputPath = args[1];

//            return parameters;
//        }

//        public static ExportGeoJsonFieldsCommandParameters GetFromUser()
//        {
//            Console.WriteLine("Enter a comma-separated list of field IDs or a file path (start with @):");
//            var fieldIds = (Console.ReadLine() ?? string.Empty).Trim();

//            Console.WriteLine($"Where to save the archive with GeoJSON fields? " +
//                              $"(Enter — current: {Environment.CurrentDirectory})");
//            var outputPath = (Console.ReadLine() ?? string.Empty).Trim().Trim('"');

//            return new ExportGeoJsonFieldsCommandParameters 
//            { 
//                FieldIds = fieldIds, 
//                OutputPath = outputPath 
//            };
//        }
//    }

//    [BaseCommand("GeoJson: Export Fields", "Экспорт полей в GeoJSON", typeof(ExportGeoJsonFieldsCommandParameters))]
//    public class ExportGeoJsonFieldsCommand : IBaseCommand
//    {
//        private IGeoJsonExportService GeoJsonExport {  get; set; }

//        public ExportGeoJsonFieldsCommand(IGeoJsonExportService export)
//        {
//            GeoJsonExport = export;
//        }

//        public void Dispose()
//        {
            
//        }

//        public async Task Execute()
//        {
//            var parameters = ExportGeoJsonFieldsCommandParameters.GetFromUser();
//            await ExecuteCore(parameters);
//        }

//        public async Task Execute(string[] args)
//        {
//            var parameters = ExportGeoJsonFieldsCommandParameters.Parse(args);
//            await ExecuteCore(parameters);
//        }

//        private async Task ExecuteCore(ExportGeoJsonFieldsCommandParameters parameters)
//        {
//            var ids = ParseIdsOrFile(parameters.FieldIds).ToArray();

//            try
//            {
//                var zipBytes = await GeoJsonExport.ExportFieldsGeoJsonZipAsync(ids);

//                var dir = string.IsNullOrWhiteSpace(parameters.OutputPath) ? Environment.CurrentDirectory : parameters.OutputPath;
//                dir = Path.GetFullPath(dir);
//                Directory.CreateDirectory(dir);

//                var fileName = "fields_geojson.zip";
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
//                      StringSplitOptions.RemoveEmptyEntries)
//               .Select(s => s.Trim())
//               .Select(Guid.Parse);
//        }
//    }
//}
