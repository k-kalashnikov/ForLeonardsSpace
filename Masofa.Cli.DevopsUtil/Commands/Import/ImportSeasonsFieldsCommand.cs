using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.CoordinateSystems;
using NetTopologySuite.CoordinateSystems.Transformations;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.Operation.Valid;
using NetTopologySuite.Utilities;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System.Text.RegularExpressions;


namespace Masofa.Cli.DevopsUtil.Commands.Import
{
    [BaseCommand("Shape: Import Seasons Fields", "Импорт данных полей и посевов из Shape", typeof(ImportSoilDataTiffCommandParameters))]
    public class ImportSeasonsFieldsCommand : IBaseCommand
    {
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext {  get; set; }
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
        private IFileStorageProvider FileStorageProvider { get; set; }
        public ImportSeasonsFieldsCommand(MasofaCropMonitoringDbContext masofaCropMonitoringDbContext, MasofaDictionariesDbContext masofaDictionariesDbContext, IFileStorageProvider fileStorageProvider)
        {
            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
            FileStorageProvider = fileStorageProvider;
        }

        public async Task Execute()
        {
            Console.WriteLine("Скачиваем файлы из хранилища...");
            var folderPath = await GetFilePath();

            try
            {
                await ExecuteAsync(folderPath);
            }
            finally
            {
                try
                {
                    Directory.Delete(folderPath, true);
                    Console.WriteLine($"Временная папка удалена: {folderPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Не удалось удалить временную папку: {ex.Message}");
                }
            }
        }

        public async Task<string> GetFilePath()
        {
            var tempDir = Path.Combine(Environment.CurrentDirectory, $"shape_import_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}");
            Directory.CreateDirectory(tempDir);
            Console.WriteLine($"Временная папка создана: {tempDir}");

            try
            {
                var baseName = "Monitoring2804";
                var remotePrefix = "Crop monitoring 2804/";
                var bucket = "develop";

                var fileExtensions = new[]
                {
                    ".cpg", ".dbf", ".prj", ".sbn", ".sbx",
                    ".shp", ".shx", ".shp.xml"
                };

                foreach (var ext in fileExtensions)
                {
                    var remotePath = $"{remotePrefix}{baseName}{ext}";
                    var localPath = Path.Combine(tempDir, $"{baseName}{ext}");

                    try
                    {
                        using var remoteStream = await FileStorageProvider.GetFileStreamAsync(remotePath, bucket);
                        using var localFileStream = File.Create(localPath);

                        await remoteStream.CopyToAsync(localFileStream);
                        Console.WriteLine($"Скачан: {remotePath} → {localPath}");
                    }
                    catch (Exception ex)
                    {
                        if (ext is ".sbn" or ".sbx" or ".shp.xml" or ".cpg")
                        {
                            Console.WriteLine($"Файл не найден (не критично): {remotePath}");
                        }
                        else
                        {
                            Console.WriteLine($"Ошибка при скачивании обязательного файла {remotePath}: {ex.Message}");
                            throw;
                        }
                    }
                }

                var requiredFiles = new[] { ".shp", ".shx", ".dbf" };
                foreach (var ext in requiredFiles)
                {
                    var filePath = Path.Combine(tempDir, $"{baseName}{ext}");
                    if (!File.Exists(filePath))
                    {
                        throw new InvalidOperationException($"Обязательный файл отсутствует: {filePath}");
                    }
                }

                Console.WriteLine($"Все файлы успешно скачаны в: {tempDir}");
                return tempDir;
            }
            catch
            {
                try { Directory.Delete(tempDir, true); } catch { }
                throw;
            }

        }

        public async Task Execute(string[] args)
        {
            Console.WriteLine("Скачиваем файлы из хранилища...");
            var folderPath = await GetFilePath();

            try
            {
                await ExecuteAsync(folderPath);
            }
            finally
            {
                try
                {
                    Directory.Delete(folderPath, true);
                    Console.WriteLine($"Временная папка удалена: {folderPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Не удалось удалить временную папку: {ex.Message}");
                }
            }
        }

        public void Dispose()
        {

        }

        public async Task ExecuteAsync(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"Папка {folderPath} не найдена.");
                return;
            }

            // Находим .shp файл в папке
            var shpFiles = Directory.GetFiles(folderPath, "*.shp", SearchOption.TopDirectoryOnly);
            if (shpFiles.Length == 0)
            {
                Console.WriteLine($"В папке {folderPath} не найден файл с расширением .shp");
                return;
            }

            if (shpFiles.Length > 1)
            {
                Console.WriteLine($"В папке найдено несколько .shp файлов. Используем первый: {shpFiles[0]}");
            }

            string shapefilePath = shpFiles[0];
            var (mt, gf4326) = BuildTransformerFromPrj(shapefilePath);
            IEnumerable<IFeature> feats = NetTopologySuite.IO.Esri.Shapefile.ReadAllFeatures(shapefilePath);

            Console.WriteLine($"Начинаем импорт... Найдено {feats.Count()} записей.");

            int importedCount = 0;
            int skippedCount = 0;

            // Группируем по farmer_cad
            var groupedByCad = feats
                .GroupBy(f => f.Attributes["farmer_cad"]?.ToString() ?? "")
                .ToDictionary(g => g.Key, g => g.ToList());

            string konturRaq = "";

            foreach (var group in groupedByCad)
            {
                string farmerCad = group.Key;
                if (string.IsNullOrWhiteSpace(farmerCad))
                {
                    Console.WriteLine("Пропущена группа без farmer_cad");
                    continue;
                }

                var reservedNamesThisRun = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var featureList = group.Value;
                var groupKeys = featureList
                    .Select(f => GetSeasonKey(f.Attributes))
                    .Where(k => !string.IsNullOrWhiteSpace(k))
                    .Distinct()
                    .ToList();

                var existingKeys = await MasofaCropMonitoringDbContext.Seasons
                    .Where(s => groupKeys.Contains(s.Title))
                    .Select(s => s.Title)
                    .ToHashSetAsync();

                // === Шаг 1: Собираем посевы из Shape-файла ===
                var seasonsFromShape = new List<(Season Season, Polygon Polygon)>();

                foreach (var feature in featureList)
                {
                    try
                    {
                        var attributes = feature.Attributes;

                        var seasonKey = GetSeasonKey(attributes);

                        var rawGeom = feature.Geometry; // как пришло из шейпа (обычно SRID=0)
                        var rawPoly = ExtractPolygon(rawGeom);
                        if (rawPoly == null) continue;

                        var geometry = TransformToWgs84(rawPoly, mt, gf4326);

                        if (IsBad(geometry, out var why))
                        {
                            Console.WriteLine($"[INVALID] {why} — пытаюсь исправить");
                            geometry = FixGeom(geometry);
                            if (IsBad(geometry, out var why2))
                            {
                                Console.WriteLine($"[SKIP] геометрия не исправилась: {why2}");
                                skippedCount++;
                                continue;
                            }
                        }

                        if (geometry.Area <= 0 || geometry.NumPoints < 4)
                        {
                            Console.WriteLine("[SKIP] вырожденная геометрия (нулевая площадь/точек мало)");
                            skippedCount++;
                            continue;
                        }

                        geometry.SRID = 4326;

                        if (geometry == null)
                        {
                            Console.WriteLine("Не удалось преобразовать геометрию в WGS84 — пропущено");
                            skippedCount++;
                            continue;
                        }

                        var yieldHa = ComputeAreaHa((Polygon?)geometry);

                        string cropName = attributes["crop_name1"]?.ToString() ?? "";
                        double cropArea = Convert.ToDouble(attributes["crop_area"] ?? 0);
                        konturRaq = attributes["kontur_raq"]?.ToString() ?? "";
                        string regionNam = attributes["region_nam"]?.ToString() ?? "";
                        string farmerNam = attributes["farmer_nam"]?.ToString() ?? "";
                        string landType1 = attributes["land_type1"]?.ToString() ?? "";

                        var season = new Season
                        {
                            Title = $"{cropName} - {konturRaq}",
                            CropId = await GetCropIdAsync(cropName),
                            FieldArea = cropArea,
                            PlantingDate = DateOnly.FromDateTime(DateTime.UtcNow),
                            HarvestingDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(90)),
                            YieldHaFact = yieldHa,
                            Polygon = (Polygon)geometry,
                            Latitude = geometry.Centroid?.Y,
                            Longitude = geometry.Centroid?.X,
                            CreateAt = DateTime.UtcNow,
                            LastUpdateAt = DateTime.UtcNow
                        };

                        seasonsFromShape.Add(((Season Season, Polygon Polygon))(season, geometry));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при обработке посева: {ex.Message}");
                        skippedCount++;
                    }
                }

                if (!seasonsFromShape.Any()) continue;

                // === Шаг 2: Разбиваем на связные компоненты ===
                var components = SplitIntoConnectedComponents(seasonsFromShape);

                // === Шаг 3: Получаем существующие поля с этим ExternalId ===
                var existingFields = await MasofaCropMonitoringDbContext.Fields
                    .Where(f => f.ExternalId == farmerCad)
                    .ToListAsync();

                static string Key(string? s) => string.IsNullOrWhiteSpace(s) ? "" : s.Trim().ToLowerInvariant();
                var fieldsByName = existingFields.ToDictionary(f => Key(f.Name));

                var usedFieldIds = new HashSet<Guid>();

                // === Шаг 4: Обрабатываем каждую компоненту ===
                foreach (var component in components)
                {
                    // Средний центроид компоненты
                    var componentCentroids = component.Select(c => c.Polygon.Centroid).ToList();
                    var avgX = componentCentroids.Average(p => p.X);
                    var avgY = componentCentroids.Average(p => p.Y);
                    var componentCentroid = new Point(avgX, avgY) { SRID = 4326 };

                    Field? targetField = null;

                    if (existingFields.Any())
                    {
                        const double maxDistanceMeters = 50.0;
                        const double maxDistanceDegrees = maxDistanceMeters / 111000.0;

                        targetField = existingFields
                            .Where(f => f.Polygon != null && !usedFieldIds.Contains(f.Id))
                            .Where(f => f.Polygon.Centroid.Distance(componentCentroid) <= maxDistanceDegrees)
                            .OrderBy(f => f.Polygon.Centroid.Distance(componentCentroid))
                            .FirstOrDefault();
                    }

                    if (targetField == null)
                    {
                        var konturForComponent = KonturFromComponent(component);
                        var desiredName = string.IsNullOrWhiteSpace(konturForComponent)
                            ? farmerCad
                            : $"{farmerCad}/{konturForComponent}";

                        if (fieldsByName.TryGetValue(Key(desiredName), out var byName))
                            targetField = byName;
                    }

                    if (targetField != null)
                    {
                        // Обновляем существующее поле
                        usedFieldIds.Add(targetField.Id);

                        foreach (var (season, _) in component)
                        {
                            season.FieldId = targetField.Id;
                            MasofaCropMonitoringDbContext.Seasons.Add(season);
                        }
                        await MasofaCropMonitoringDbContext.SaveChangesAsync();

                        // Обновляем полигон поля: все посевы (старые + новые)
                        var allSeasonsForField = await MasofaCropMonitoringDbContext.Seasons
                            .Where(s => s.FieldId == targetField.Id && s.Polygon != null)
                            .ToListAsync();

                        var allGeometries = allSeasonsForField.Select(s => s.Polygon).Where(g => g != null).ToArray();

                        if (allGeometries.Length > 0 && targetField.Polygon == null)
                        {
                            var unioned = RobustUnion4326(allGeometries);
                            if (unioned is NetTopologySuite.Geometries.Polygon up) targetField.Polygon = up;
                            else if (unioned is NetTopologySuite.Geometries.MultiPolygon ump)
                                targetField.Polygon = ump.Geometries.OfType<NetTopologySuite.Geometries.Polygon>()
                                    .OrderByDescending(p => p.Area).FirstOrDefault();
                        }

                        // Пересчёт площади поля, если есть полигон
                        if (targetField.Polygon != null)
                        {
                            var ha = ComputeAreaHa(targetField.Polygon);
                            targetField.FieldArea = ha;
                        }

                        //if (allGeometries.Length > 0 && targetField.Polygon == null)
                        //{
                        //    var collection = new GeometryCollection(allGeometries);
                        //    var unioned = collection.Union();
                            
                        //    if(targetField.Polygon != null)
                        //    {
                        //        continue;
                        //    }
                        //    if (unioned is Polygon poly)
                        //    {
                        //        targetField.Polygon = poly;
                        //    }
                        //    else if (unioned is MultiPolygon mp)
                        //    {
                        //        targetField.Polygon = mp.Geometries.OfType<Polygon>().OrderByDescending(p => p.Area).FirstOrDefault();
                        //    }
                        //}

                        await MasofaCropMonitoringDbContext.SaveChangesAsync();
                        importedCount += component.Count;
                        Console.WriteLine($"Добавлено {component.Count} посевов в существующее поле: {targetField.Name}");
                    }
                    else
                    {
                        // Создаём новое поле

                        var firstAttrs = featureList[0].Attributes;
                        konturRaq = firstAttrs["kontur_raq"]?.ToString() ?? "";
                        string regionNam = firstAttrs["region_nam"]?.ToString() ?? "";
                        string farmerNam = firstAttrs["farmer_nam"]?.ToString() ?? "";
                        string landType1 = firstAttrs["land_type1"]?.ToString() ?? "";
                        var baseName = string.IsNullOrWhiteSpace(konturRaq)
                            ? farmerCad
                            : $"{farmerCad}/{konturRaq}";

                        // 2) Берём уникальное имя (учитывает БД + reservedNamesThisRun)
                        var fieldName = await GetUniqueFieldNameAsync(
                            MasofaCropMonitoringDbContext,
                            farmerCad,
                            baseName,
                            reservedNamesThisRun
                        );

                        var newField = new Field
                        {
                            Name = fieldName,
                            ExternalId = farmerCad,
                            CreateAt = DateTime.UtcNow,
                            LastUpdateAt = DateTime.UtcNow,
                            RegionId = await GetRegionIdAsync(regionNam),
                            SoilTypeId = await GetSoilTypeIdAsync(landType1),
                            AgricultureProducerId = await GetProducerIdAsync(farmerNam),
                        };

                        MasofaCropMonitoringDbContext.Fields.Add(newField);
                        await MasofaCropMonitoringDbContext.SaveChangesAsync();

                        reservedNamesThisRun.Add(fieldName);
                        existingFields.Add(newField);

                        foreach (var (season, _) in component)
                        {
                            season.FieldId = newField.Id;
                            MasofaCropMonitoringDbContext.Seasons.Add(season);
                        }
                        await MasofaCropMonitoringDbContext.SaveChangesAsync();

                        try
                        {
                            var geometries = component.Select(c => c.Polygon).ToArray();
                            var unioned = RobustUnion4326(geometries);

                            if (unioned is NetTopologySuite.Geometries.Polygon poly)
                                newField.Polygon = poly;
                            else if (unioned is NetTopologySuite.Geometries.MultiPolygon mp)
                                newField.Polygon = mp.Geometries.OfType<NetTopologySuite.Geometries.Polygon>()
                                    .OrderByDescending(p => p.Area).FirstOrDefault();

                            newField.FieldArea = ComputeAreaHa(newField.Polygon);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[WARN] union нового поля не выполнен: {ex.Message} — поле оставлено без полигона");
                        }

                        await MasofaCropMonitoringDbContext.SaveChangesAsync();
                        importedCount += component.Count;
                        Console.WriteLine($"Создано новое поле: {newField.Name} с {component.Count} посевами.");
                    }
                }
            }

            Console.WriteLine($"Готово! Импортировано посевов: {importedCount}, Пропущено: {skippedCount}");
        }

        static string KonturFromComponent(List<(Season Season, Polygon Polygon)> comp)
        {
            return comp.Select(c => c.Season.Title)
                       .Select(t => {
                           if (string.IsNullOrWhiteSpace(t)) return "";
                           var i = t.LastIndexOf('-');
                           return i >= 0 ? t.Substring(i + 1).Trim() : "";
                       })
                       .Where(s => !string.IsNullOrWhiteSpace(s))
                       .GroupBy(s => s)
                       .OrderByDescending(g => g.Count())
                       .FirstOrDefault()?.Key ?? "";
        }

        private static async Task<string> GetUniqueFieldNameAsync(
            MasofaCropMonitoringDbContext db,
            string farmerCad,                 
            string baseName,
            ISet<string>? reservedThisRun = null) 
        {
            reservedThisRun ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var existingNames = await db.Fields
                .AsNoTracking()
                .Where(f => f.ExternalId == farmerCad &&
                            (f.Name == baseName || EF.Functions.Like(f.Name!, baseName + "_part%")))
                .Select(f => f.Name!)
                .ToListAsync();

            foreach (var name in reservedThisRun)
            {
                if (!existingNames.Contains(name, StringComparer.OrdinalIgnoreCase))
                {
                    existingNames.Add(name);
                }
            }

            if (!existingNames.Contains(baseName, StringComparer.OrdinalIgnoreCase))
            {
                return baseName;
            }

            var rx = new Regex("^" + Regex.Escape(baseName) + "(?:_part(?<n>\\d+))?$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            int max = 1;
            foreach (var name in existingNames)
            {
                var m = rx.Match(name);
                if (!m.Success) continue;
                if (m.Groups["n"].Success && int.TryParse(m.Groups["n"].Value, out var n))
                {
                    max = Math.Max(max, n);
                }
                else
                {
                    max = Math.Max(max, 1);
                }
            }

            int next = max + 1;
            string candidate;
            do
            {
                candidate = $"{baseName}_part{next}";
                next++;
            } while (existingNames.Contains(candidate, StringComparer.OrdinalIgnoreCase));

            return candidate;
        }

        NetTopologySuite.Geometries.Geometry ApplyMt(NetTopologySuite.Geometries.Geometry g, ProjNet.CoordinateSystems.Transformations.MathTransform mt)
        {
            var copy = (NetTopologySuite.Geometries.Geometry)g.Copy();
            copy.Apply(new MathTransformFilter(mt)); // твой фильтр из класса
            return copy;
        }

        (ProjNet.CoordinateSystems.Transformations.MathTransform toUtm, ProjNet.CoordinateSystems.Transformations.MathTransform toWgs) BuildLocalUtm(NetTopologySuite.Geometries.Geometry g4326)
        {
            var c = g4326.Centroid;
            var lon = c.X; var lat = c.Y;
            int zone = (int)Math.Floor((lon + 180.0) / 6.0) + 1;
            bool north = lat >= 0;

            var csf = new ProjNet.CoordinateSystems.CoordinateSystemFactory();
            var wgs = ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84;
            var utm = ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WGS84_UTM(zone, north);

            var ctf = new ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory();
            return (ctf.CreateFromCoordinateSystems(wgs, utm).MathTransform,
                    ctf.CreateFromCoordinateSystems(utm, wgs).MathTransform);
        }

        NetTopologySuite.Geometries.Geometry? RobustUnion4326(IEnumerable<NetTopologySuite.Geometries.Geometry> input4326)
        {
            var list = input4326.Where(g => g != null && !g.IsEmpty).ToList();
            if (list.Count == 0) return null;

            var seed = list[0];
            if (seed.SRID == 0) seed.SRID = 4326;
            if (seed.SRID != 4326) throw new InvalidOperationException("RobustUnion ожидает SRID=4326");

            var (toUtm, toWgs) = BuildLocalUtm(seed);

            // → UTM + фиксы
            var utmList = new List<NetTopologySuite.Geometries.Geometry>(list.Count);
            foreach (var g in list)
            {
                var gg = ApplyMt(g, toUtm);
                if (!gg.IsValid) gg = gg.Buffer(0);
                utmList.Add(gg);
            }

            // Понижаем точность до сантиметра (scale=100)
            var reducer = new NetTopologySuite.Precision.GeometryPrecisionReducer(new NetTopologySuite.Geometries.PrecisionModel(100));
            for (int i = 0; i < utmList.Count; i++)
                utmList[i] = reducer.Reduce(utmList[i]);

            NetTopologySuite.Geometries.Geometry utmUnion;
            try
            {
                utmUnion = NetTopologySuite.Operation.Union.UnaryUnionOp.Union(utmList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARN] UnaryUnionOp failed: {ex.Message} — fallback по одному");
                utmUnion = utmList[0];
                for (int i = 1; i < utmList.Count; i++)
                {
                    try { utmUnion = utmUnion.Union(utmList[i]); }
                    catch (Exception ex2) { Console.WriteLine($"[SKIP] piece union failed: {ex2.Message}"); }
                }
            }

            if (utmUnion == null || utmUnion.IsEmpty) return null;

            // ← 4326 + финальная шлифовка
            var res = ApplyMt(utmUnion, toWgs);
            res.SRID = 4326;
            if (!res.IsValid) res = res.Buffer(0);

            // Если мультиполигон — берём крупнейший
            if (res is NetTopologySuite.Geometries.MultiPolygon mp)
            {
                var biggest = mp.Geometries.OfType<NetTopologySuite.Geometries.Polygon>()
                    .OrderByDescending(p => p.Area).FirstOrDefault();
                if (biggest != null) res = biggest;
            }
            return res;
        }

        private static bool IsBad(Geometry g, out string reason)
        {
            var op = new IsValidOp(g);
            if (op.IsValid) { reason = ""; return false; }
            var err = op.ValidationError;
            reason = err == null ? "Unknown invalid geometry" : $"{err.Message} @ {err.Coordinate}";
            return true;
        }

        private static Geometry FixGeom(Geometry g)
        {
            // 1) Пытаемся аккуратно починить
            var fixedGeom = GeometryFixer.Fix(g);

            // 2) Старый трюк, если всё ещё плохо
            if (!fixedGeom.IsValid) fixedGeom = fixedGeom.Buffer(0);

            // 3) Если после фикса мультиполигон — возьми самый большой полигон
            if (fixedGeom is MultiPolygon mp)
            {
                var biggest = mp.Geometries.OfType<Polygon>()
                    .OrderByDescending(p => p.Area).FirstOrDefault();
                if (biggest != null) fixedGeom = biggest;
            }

            return fixedGeom;
        }

        //private static Polygon TransformFromUtmToWgs84(Polygon utmPolygon)
        //{
        //    try
        //    {
        //        var utm = ProjectedCoordinateSystem.WGS84_UTM(41, true); // Zone 41N
        //        var wgs84 = GeographicCoordinateSystem.WGS84;

        //        var transform = new CoordinateTransformationFactory()
        //            .CreateFromCoordinateSystems(utm, wgs84)
        //            .MathTransform;

        //        Coordinate[] TransformCoords(Coordinate[] src)
        //        {
        //            var dst = new Coordinate[src.Length];
        //            for (int i = 0; i < src.Length; i++)
        //            {
        //                var p = transform.Transform(new[] { src[i].X, src[i].Y });
        //                dst[i] = new Coordinate(p[0], p[1]);
        //            }
        //            return dst;
        //        }

        //        var gf = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        //        var shell = gf.CreateLinearRing(TransformCoords(utmPolygon.ExteriorRing.Coordinates));

        //        var holes = utmPolygon.NumInteriorRings == 0
        //            ? Array.Empty<NetTopologySuite.Geometries.LinearRing>()
        //            : Enumerable.Range(0, utmPolygon.NumInteriorRings)
        //                .Select(i => gf.CreateLinearRing(TransformCoords(utmPolygon.GetInteriorRingN(i).Coordinates)))
        //                .ToArray();

        //        return gf.CreatePolygon(shell, holes);
        //    }
        //    catch
        //    {
        //        return utmPolygon;
        //    }
        //}

        private static double? ComputeAreaHa(Polygon? poly)
        {
            if (poly is null) return null;

            var c = poly.Centroid;
            var lon = c.X; var lat = c.Y;

            int zone = (int)Math.Floor((lon + 180d) / 6d) + 1;
            bool north = lat >= 0;

            var csFactory = new CoordinateSystemFactory();
            var wgs84 = GeographicCoordinateSystem.WGS84;
            var utm = ProjectedCoordinateSystem.WGS84_UTM(zone, north);

            var ctFactory = new CoordinateTransformationFactory();
            var trans = ctFactory.CreateFromCoordinateSystems(wgs84, utm);

            var transform = new CoordinateTransformationFactory()
                .CreateFromCoordinateSystems(GeographicCoordinateSystem.WGS84,
                                             ProjectedCoordinateSystem.WGS84_UTM(zone, north))
                .MathTransform;

            var polyUtm = TransformToUtm(poly, transform, poly.Factory);
            var m2 = polyUtm.Area;
            if (m2 <= 1.0) return 0;
            return polyUtm.Area / 10_000d; // м² -> гектары
        }

        private static Polygon TransformToUtm(Polygon poly, ProjNet.CoordinateSystems.Transformations.MathTransform mt, GeometryFactory gf)
        {
            Coordinate[] Tx(Coordinate[] src)
            {
                var dst = new Coordinate[src.Length];
                for (int i = 0; i < src.Length; i++)
                {
                    var p = mt.Transform(new[] { src[i].X, src[i].Y });
                    dst[i] = new Coordinate(p[0], p[1]);
                }
                return dst;
            }

            var shell = gf.CreateLinearRing(Tx(poly.ExteriorRing.Coordinates));
            var holes = poly.NumInteriorRings == 0
                ? Array.Empty<NetTopologySuite.Geometries.LinearRing>()
                : Enumerable.Range(0, poly.NumInteriorRings)
                    .Select(i => gf.CreateLinearRing(Tx(poly.GetInteriorRingN(i).Coordinates)))
                    .ToArray();

            return gf.CreatePolygon(shell, holes);
        }

        private List<List<(Season Season, Polygon Polygon)>> SplitIntoConnectedComponents(List<(Season Season, Polygon Polygon)> items,
            double toleranceMeters = 10.0)
        {
            // Преобразуем допуск в градусы (для WGS84)
            double tolerance = toleranceMeters / 111000.0;

            var unassigned = new List<(Season Season, Polygon Polygon)>(items);
            var components = new List<List<(Season Season, Polygon Polygon)>>();

            while (unassigned.Count > 0)
            {
                var component = new List<(Season Season, Polygon Polygon)>();
                var queue = new Queue<(Season Season, Polygon Polygon)>();
                var first = unassigned[0];
                queue.Enqueue(first);
                unassigned.Remove(first);

                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();
                    component.Add(current);

                    var neighbors = unassigned
                        .Where(other =>
                            current.Polygon.IsWithinDistance(other.Polygon, tolerance) ||
                            current.Polygon.Intersects(other.Polygon) ||
                            current.Polygon.Touches(other.Polygon))
                        .ToList();

                    foreach (var neighbor in neighbors)
                    {
                        queue.Enqueue(neighbor);
                        unassigned.Remove(neighbor);
                    }
                }

                components.Add(component);
            }

            return components;
        }

        private List<Region>? _cachedRegions;

        private async Task<List<Region>> GetRegionsCacheAsync()
        {
            return _cachedRegions ??= await MasofaDictionariesDbContext.Set<Region>()
                .Where(r => r.Status == StatusType.Active)
                .ToListAsync();
        }

        private List<Crop>? _cropCache;
        private async Task<List<Crop>> GetCropCacheAsync()
        {
            return _cropCache ??= await MasofaDictionariesDbContext.Set<Crop>()
                .Where(c => c.Status == StatusType.Active)
                .ToListAsync();
        }

        private List<Person>? _personCache;
        private async Task<List<Person>> GetPersonCacheAsync()
        {
            return _personCache ??= await MasofaDictionariesDbContext.Set<Person>()
                .Where(p => p.Status == StatusType.Active)
                .ToListAsync();
        }

        private List<SoilType>? _soilTypeCache;
        private async Task<List<SoilType>> GetSoilTypeCacheAsync()
        {
            return _soilTypeCache ??= await MasofaDictionariesDbContext.Set<SoilType>()
                .Where(s => s.Status == StatusType.Active)
                .ToListAsync();
        }

        private async Task<Guid?> GetRegionIdAsync(string regionNameFromShape)
        {
            if (string.IsNullOrWhiteSpace(regionNameFromShape))
                return null;

            // Нормализуем входное значение
            var input = regionNameFromShape.Trim();

            // Загружаем все активные регионы
            var regions = await GetRegionsCacheAsync();

            // 1. Попытка точного совпадения по ru-RU
            var exactMatch = regions.FirstOrDefault(r =>
                !string.IsNullOrEmpty(r.Names["ru-RU"]) &&
                string.Equals(r.Names["ru-RU"].Trim(), input, StringComparison.OrdinalIgnoreCase));

            if (exactMatch != null)
                return exactMatch.Id;

            // 2. Попытка по uz-Latn-UZ
            exactMatch = regions.FirstOrDefault(r =>
                !string.IsNullOrEmpty(r.Names["uz-Latn-UZ"]) &&
                string.Equals(r.Names["uz-Latn-UZ"].Trim(), input, StringComparison.OrdinalIgnoreCase));

            if (exactMatch != null)
                return exactMatch.Id;

            // 3. Нечёткое сравнение по ru-RU (Contains)
            var fuzzyMatch = regions.FirstOrDefault(r =>
                !string.IsNullOrEmpty(r.Names["ru-RU"]) &&
                r.Names["ru-RU"].Trim().Contains(input, StringComparison.OrdinalIgnoreCase));

            if (fuzzyMatch != null)
                return fuzzyMatch.Id;

            // 4. Нечёткое сравнение по uz-Latn-UZ
            fuzzyMatch = regions.FirstOrDefault(r =>
                !string.IsNullOrEmpty(r.Names["uz-Latn-UZ"]) &&
                r.Names["uz-Latn-UZ"].Trim().Contains(input, StringComparison.OrdinalIgnoreCase));

            if (fuzzyMatch != null)
                return fuzzyMatch.Id;

            // 5. Сравнение без "область", "район", "город" и т.д.
            var cleanedInput = CleanRegionName(input);
            var cleanedMatch = regions.FirstOrDefault(r =>
            {
                var ruName = r.Names["ru-RU"]?.Trim();
                var uzName = r.Names["uz-Latn-UZ"]?.Trim();
                return (!string.IsNullOrEmpty(ruName) && CleanRegionName(ruName).Equals(cleanedInput, StringComparison.OrdinalIgnoreCase)) ||
                       (!string.IsNullOrEmpty(uzName) && CleanRegionName(uzName).Equals(cleanedInput, StringComparison.OrdinalIgnoreCase));
            });

            if (cleanedMatch != null)
                return cleanedMatch.Id;

            // Логируем несовпадение
            Console.WriteLine($"Регион не найден: '{regionNameFromShape}'");
            return null;
        }

        private static string CleanRegionName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return name;

            // Удаляем тип региона и приводим к нормальной форме
            return name
                .Replace(" область", "", StringComparison.OrdinalIgnoreCase)
                .Replace(" район", "", StringComparison.OrdinalIgnoreCase)
                .Replace(" город", "", StringComparison.OrdinalIgnoreCase)
                .Replace(" independent city", "", StringComparison.OrdinalIgnoreCase)
                .Replace(" gorhokimiyat towns", "", StringComparison.OrdinalIgnoreCase)
                .Replace(" autonomous republic", "", StringComparison.OrdinalIgnoreCase)
                .Replace(" viloyati", "", StringComparison.OrdinalIgnoreCase)
                .Replace(" tumani", "", StringComparison.OrdinalIgnoreCase)
                .Replace(" shahri", "", StringComparison.OrdinalIgnoreCase)
                .Replace(" Respublikasi", "", StringComparison.OrdinalIgnoreCase)
                .Replace(".", "")
                .Trim();
        }

        private static Polygon? ExtractPolygon(Geometry? geometry)
        {
            if (geometry == null) return null;

            // Если это Polygon — возвращаем его
            if (geometry is Polygon polygon)
                return polygon;

            // Если это MultiPolygon — берем первый полигон (если есть)
            if (geometry is MultiPolygon multiPolygon && multiPolygon.NumGeometries > 0)
            {
                var firstPoly = multiPolygon.Geometries[0] as Polygon;
                if (firstPoly != null)
                    return firstPoly;
            }

            Console.WriteLine($"Неподдерживаемый тип геометрии: {geometry.GetType().Name}");
            return null;
        }

        private async Task<Guid?> GetProducerIdAsync(string producerName)
        {
            if (string.IsNullOrWhiteSpace(producerName)) return null;

            var persons = await GetPersonCacheAsync();

            var match = persons.FirstOrDefault(p =>
                !string.IsNullOrEmpty(p.FullName) &&
                string.Equals(p.FullName.Trim(), producerName.Trim(), StringComparison.OrdinalIgnoreCase));

            return match?.Id;
        }

        private async Task<Guid?> GetCropIdAsync(string cropName)
        {
            if (string.IsNullOrWhiteSpace(cropName)) return null;

            var normalizedCropName = cropName.Trim().ToLowerInvariant();

            var crops = await GetCropCacheAsync();

            var match = crops.FirstOrDefault(c =>
                !string.IsNullOrEmpty(c.Names["ru-RU"]) &&
                c.Names["ru-RU"].Trim().ToLowerInvariant() == normalizedCropName);

            if (match != null) return match.Id;

            match = crops.FirstOrDefault(c =>
                !string.IsNullOrEmpty(c.Names["uz-Latn-UZ"]) &&
                c.Names["uz-Latn-UZ"].Trim().ToLowerInvariant() == normalizedCropName);

            if (match != null) return match.Id;

            match = crops.FirstOrDefault(c =>
                !string.IsNullOrEmpty(c.Names["en-US"]) &&
                c.Names["en-US"].Trim().ToLowerInvariant() == normalizedCropName);

            if (match != null) return match.Id;

            Console.WriteLine($"Культура не найдена: '{cropName}'");
            return null;
        }

        private async Task<Guid?> GetSoilTypeIdAsync(string soilTypeName)
        {
            if (string.IsNullOrWhiteSpace(soilTypeName)) return null;

            var soilTypes = await GetSoilTypeCacheAsync();

            var match = soilTypes.FirstOrDefault(s =>
                !string.IsNullOrEmpty(s.Names["ru-RU"]) &&
                string.Equals(s.Names["ru-RU"].Trim(), soilTypeName.Trim(), StringComparison.OrdinalIgnoreCase));

            return match?.Id;
        }

        public sealed class MathTransformFilter : ICoordinateSequenceFilter
        {
            private readonly MathTransform _mt;
            public MathTransformFilter(MathTransform mt) => _mt = mt;

            public void Filter(CoordinateSequence seq, int i)
            {
                double x = seq.GetX(i);
                double y = seq.GetY(i);
                var xy = _mt.Transform(new[] { x, y });
                seq.SetX(i, xy[0]);
                seq.SetY(i, xy[1]);
            }

            public bool Done => false;
            public bool GeometryChanged => true;
        }

        private (MathTransform Transform, GeometryFactory Gf4326) BuildTransformerFromPrj(string shapefilePath)
        {
            var prjPath = Path.ChangeExtension(shapefilePath, ".prj");
            if (!File.Exists(prjPath))
                throw new InvalidOperationException($".prj не найден рядом с {shapefilePath}. " +
                    "Либо положи .prj, либо явно задай EPSG исходной СК.");

            var prjWkt = File.ReadAllText(prjPath);

            var csFactory = new CoordinateSystemFactory();
            var srcCs = csFactory.CreateFromWkt(prjWkt); // ICoordinateSystem
            var wgs84 = GeographicCoordinateSystem.WGS84;

            var ctFactory = new CoordinateTransformationFactory();
            var ct = ctFactory.CreateFromCoordinateSystems(srcCs, wgs84);
            var mt = ct.MathTransform; // MathTransform (ProjNet)

            // Геометрии в БД будут храниться с SRID=4326
            var gf4326 = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            return (mt, gf4326);
        }

        private string GetSeasonKey(IAttributesTable attributes)
        {
            var cropName = attributes["crop_name1"]?.ToString() ?? "";
            var konturRaq = attributes["kontur_raq"]?.ToString() ?? "";
            return $"{cropName} - {konturRaq}";
        }

        private Geometry TransformToWgs84(Geometry geom, MathTransform mt, GeometryFactory gf4326)
        {
            if (geom == null) return null;

            if (!geom.IsValid)
                geom = geom.Buffer(0);

            // Делаем глубокую копию геометрии в целевой фабрике (SRID=4326)
            var copy = gf4326.CreateGeometry(geom);
            copy.SRID = 4326;

            // Применяем MathTransform ко всем вершинам
            copy.Apply(new MathTransformFilter(mt));

            // На всякий — нормализуем и возвращаем
            if (!copy.IsValid)
                copy = copy.Buffer(0);

            return copy;
        }
    }
}
