//using Masofa.BusinessLogic.GeoIo;
//using Masofa.Common.Models;
//using Masofa.Common.Models.System;
//using Masofa.DataAccess;
//using Microsoft.EntityFrameworkCore;

//namespace Masofa.Cli.DevopsUtil.Commands.Import
//{
//    public class ImportShapeSeasonsCommandParameters
//    {
//        [TaskParameter("Путь к архиву (.zip) с ShapeFile для сезонов", true, "C:\\data\\seasons.zip")]
//        public string FilePath { get; set; } = string.Empty;

//        public static ImportShapeSeasonsCommandParameters Parse(string[] args)
//        {
//            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
//                throw new ArgumentException("Необходимо указать путь к архиву");

//            return new ImportShapeSeasonsCommandParameters { FilePath = args[0] };
//        }

//        public static ImportShapeSeasonsCommandParameters GetFromUser()
//        {
//            Console.WriteLine("Укажите путь к архиву (.zip) c ShapeFile (сезоны):");
//            var filePath = (Console.ReadLine() ?? string.Empty).Trim().Trim('"');
//            return new ImportShapeSeasonsCommandParameters { FilePath = filePath };
//        }
//    }

//    [BaseCommand("Shape: Import Seasons", "Импорт сезонов из ShapeFile", typeof(ImportShapeSeasonsCommandParameters))]
//    public class ImportShapeSeasonsCommand : IBaseCommand
//    {
//        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext {  get; set; }
//        private IShapeImportService ShapeImportService { get; set; }
        
//        public ImportShapeSeasonsCommand(MasofaCropMonitoringDbContext masofaCropMonitoringDbContext, IShapeImportService shapeImportService)
//        {
//            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
//            ShapeImportService = shapeImportService;
//        }
        
//        public void Dispose()
//        {
//            throw new NotImplementedException();
//        }

//        public async Task Execute()
//        {
//            var parameters = ImportShapeSeasonsCommandParameters.GetFromUser();
//            await ExecuteCore(parameters);
//        }

//        public async Task Execute(string[] args)
//        {
//            var parameters = ImportShapeSeasonsCommandParameters.Parse(args);
//            await ExecuteCore(parameters);
//        }

//        private async Task ExecuteCore(ImportShapeSeasonsCommandParameters parameters)
//        {
//            if (!File.Exists(parameters.FilePath))
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine("Файл не найден.");
//                Console.ResetColor();
//                return;
//            }

//            await using var fs = File.OpenRead(parameters.FilePath);
//            var seasons = await ShapeImportService.ImportSeasonsFromShapeZipAsync(fs);

//            foreach (var s in seasons)
//            {
//                if (s.FieldId != null) continue;

//                Guid? fid = null;

//                if (!string.IsNullOrWhiteSpace(s.Title))
//                {
//                    fid = await MasofaCropMonitoringDbContext.Fields
//                        .AsNoTracking()
//                        .Where(f => f.Name == s.Title)
//                        .Select(f => (Guid?)f.Id)
//                        .FirstOrDefaultAsync();
//                }

//                if (fid == null && s.Polygon != null)
//                {
//                    var centroid = s.Polygon.Centroid;
//                    if (centroid.SRID == 0)
//                    {
//                        centroid.SRID = 4326;
//                    }
//                    try
//                    {
//                        var x = centroid.X;
//                        var y = centroid.Y;

//                        fid = await MasofaCropMonitoringDbContext.Fields
//                            .FromSqlInterpolated($@"
//                            SELECT * FROM ""Fields""
//                            WHERE ""Polygon"" IS NOT NULL
//                              AND ST_Contains(
//                                    CASE WHEN ST_SRID(""Polygon"") = 0
//                                         THEN ST_SetSRID(""Polygon"", 4326)
//                                         ELSE ""Polygon""
//                                    END,
//                                    ST_SetSRID(ST_Point({x}, {y}), 4326)
//                              )
//                            LIMIT 1")
//                            .AsNoTracking()
//                            .Select(f => (Guid?)f.Id)
//                            .FirstOrDefaultAsync();
//                    }
//                    catch(Exception ex)
//                    {
//                        Console.WriteLine($"ERROR: {ex.Message}");
//                        Console.WriteLine($"ERROR: {ex?.InnerException?.Message}");
//                    }
//                }

//                if(fid == null)
//                {
//                    fid = Guid.Empty;
//                }

//                s.FieldId = fid;

//                MasofaCropMonitoringDbContext.Seasons.Add(s);
//            }

//            var toInsert = seasons.Where(x => x.FieldId != null).ToList();
//            try
//            {
//                await MasofaCropMonitoringDbContext.Seasons.AddRangeAsync(toInsert);
//                var saved = await MasofaCropMonitoringDbContext.SaveChangesAsync();

//                Console.ForegroundColor = ConsoleColor.Green;
//                Console.WriteLine($"Импорт сезонов завершён: всего {seasons.Count}, привязано к полям {toInsert.Count}, сохранено {saved}, пропущено {seasons.Count - toInsert.Count}.");
//                Console.ResetColor();
//            }
//            catch (DbUpdateException ex)
//            {

//            }
//        }
//    }
//}
