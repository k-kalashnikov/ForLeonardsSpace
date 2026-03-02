//using Masofa.BusinessLogic.GeoIo;
//using Masofa.Common.Models;
//using Masofa.Common.Models.System;
//using Masofa.DataAccess;
//using Microsoft.Extensions.Logging;
//using NetTopologySuite.Geometries;
//using NetTopologySuite.IO;
//using SharpKml.Dom;
//using SharpKml.Engine;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Masofa.Cli.DevopsUtil.Commands.Import
//{
//    public class ImportKmlCommandParameters
//    {
//        [TaskParameter("Тип импорта: 1 - Поля, 2 - Посевы", true, "1")]
//        public string ImportType { get; set; } = "1";

//        [TaskParameter("Режим: 1 - один файл, 2 - папка", true, "1")]
//        public string Mode { get; set; } = "1";

//        [TaskParameter("Путь к файлу или папке", true, "C:\\data\\file.kml")]
//        public string Path { get; set; } = string.Empty;

//        [TaskParameter("FieldId для посевов (только для режима 2)", false, "")]
//        public string FieldId { get; set; } = string.Empty;

//        public static ImportKmlCommandParameters Parse(string[] args)
//        {
//            if (args.Length < 3)
//                throw new ArgumentException("Неверные параметры. Используйте: <importType> <mode> <path> [fieldId]");

//            var parameters = new ImportKmlCommandParameters
//            {
//                ImportType = args[0],
//                Mode = args[1],
//                Path = args[2]
//            };

//            if (args.Length > 3)
//                parameters.FieldId = args[3];

//            return parameters;
//        }

//        public static ImportKmlCommandParameters GetFromUser()
//        {
//            Console.WriteLine("Импорт KML. Выберите тип: 1 - Поля, 2 - Посевы(по FieldId)");
//            var importType = Console.ReadLine() ?? "1";

//            Console.WriteLine("Режим: 1 - один файл, 2 - папка (массово)");
//            var mode = Console.ReadLine() ?? "1";

//            string path;
//            if (mode == "1")
//            {
//                Console.Write("Укажите путь к .kml: ");
//                path = Console.ReadLine() ?? string.Empty;
//            }
//            else
//            {
//                Console.Write("Укажите папку с *.kml: ");
//                path = Console.ReadLine() ?? string.Empty;
//            }

//            string fieldId = string.Empty;
//            if (importType == "2" && mode == "2")
//            {
//                Console.Write("FieldId (Guid) для всех файлов (пусто — берем Guid из имени файла): ");
//                fieldId = Console.ReadLine() ?? string.Empty;
//            }

//            return new ImportKmlCommandParameters
//            {
//                ImportType = importType,
//                Mode = mode,
//                Path = path,
//                FieldId = fieldId
//            };
//        }
//    }

//    [BaseCommand("KML: Import (single/folder)", "Импорт KML файлов", typeof(ImportKmlCommandParameters))]
//    public class ImportKmlCommand : IBaseCommand
//    {
//        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext;
//        private readonly IKmlImportService KmlImportService;
//        private readonly ILogger<ImportKmlCommand> logger;

//        public ImportKmlCommand(IKmlImportService kmlImportService, MasofaCropMonitoringDbContext masofaCropMonitoringDbContext)
//        {
//            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
//            KmlImportService = kmlImportService;
//        }

//        public void Dispose()
//        {

//        }

//        public async Task Execute()
//        {
//            var parameters = ImportKmlCommandParameters.GetFromUser();
//            await ExecuteCore(parameters);
//        }

//        public async Task Execute(string[] args)
//        {
//            var parameters = ImportKmlCommandParameters.Parse(args);
//            await ExecuteCore(parameters);
//        }

//        private async Task ExecuteCore(ImportKmlCommandParameters parameters)
//        {
//            if (parameters.Mode == "1")
//            {
//                await ImportSingleAsync(parameters);
//            }
//            else
//            {
//                await ImportFolderAsync(parameters);
//            }
//        }

//        private async Task ImportFolderAsync(ImportKmlCommandParameters parameters)
//        {
//            if (string.IsNullOrWhiteSpace(parameters.Path) || !Directory.Exists(parameters.Path))
//            {
//                Console.WriteLine("Папка не найдена.");
//                return;
//            }

//            Guid? commonFieldId = null;
//            if (parameters.ImportType == "2" && !string.IsNullOrEmpty(parameters.FieldId))
//            {
//                if (Guid.TryParse(parameters.FieldId, out var f)) 
//                    commonFieldId = f;
//            }

//            var files = Directory.EnumerateFiles(parameters.Path, "*.kml", SearchOption.AllDirectories).ToList();
//            Console.WriteLine($"Найдено файлов: {files.Count}");

//            var total = 0;
//            foreach (var file in files)
//            {
//                try
//                {
//                    await using var s = File.OpenRead(file);
//                    if (parameters.ImportType == "1")
//                    {
//                        var fields = await KmlImportService.ImportFieldsAsync(s);
//                        MasofaCropMonitoringDbContext.Fields.AddRange(fields);
//                        total += fields.Count;
//                    }
//                    else
//                    {
//                        var fieldId = commonFieldId ?? TryParseGuidFromFileName(file);
//                        if (fieldId == null)
//                        {
//                            logger.LogWarning("Пропуск {File}: не удалось определить FieldId.", file);
//                            continue;
//                        }

//                        var seasons = await KmlImportService.ImportSeasonsAsync(fieldId.Value, s);
//                        MasofaCropMonitoringDbContext.Seasons.AddRange(seasons);
//                        total += seasons.Count;
//                    }
//                }
//                catch (Exception ex)
//                {
//                    logger.LogError(ex, "Ошибка обработки файла {File}", file);
//                }
//            }

//            await MasofaCropMonitoringDbContext.SaveChangesAsync();
//            Console.WriteLine($"Готово. Импортировано записей: {total}");
//        }

//        private static Guid? TryParseGuidFromFileName(string path)
//        {
//            var name = Path.GetFileNameWithoutExtension(path);
//            return Guid.TryParse(name, out var id) ? id : null;
//        }

//        private async Task ImportSingleAsync(ImportKmlCommandParameters parameters)
//        {
//            if (string.IsNullOrWhiteSpace(parameters.Path) || !File.Exists(parameters.Path))
//            {
//                Console.WriteLine("Файл не найден.");
//                return;
//            }

//            await using var stream = File.OpenRead(parameters.Path);

//            if (parameters.ImportType == "1")
//            {
//                var fields = await KmlImportService.ImportFieldsAsync(stream);
//                MasofaCropMonitoringDbContext.Fields.AddRange(fields);
//                await MasofaCropMonitoringDbContext.SaveChangesAsync();
//                Console.WriteLine($"Импортировано полей: {fields.Count}");
//            }
//            else
//            {
//                Console.Write("FieldId (Guid): ");
//                if (!Guid.TryParse(Console.ReadLine(), out var fieldId))
//                {
//                    Console.WriteLine("Некорректный Guid.");
//                    return;
//                }

//                var seasons = await KmlImportService.ImportSeasonsAsync(fieldId, stream);
//                MasofaCropMonitoringDbContext.Seasons.AddRange(seasons);
//                await MasofaCropMonitoringDbContext.SaveChangesAsync();
//                Console.WriteLine($"Импортировано посевов: {seasons.Count}");
//            }
//        }
//    }
//}