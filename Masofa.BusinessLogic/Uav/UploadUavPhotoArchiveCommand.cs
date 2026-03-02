using Masofa.BusinessLogic.Common;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Identity;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Models.Uav;
using Masofa.Common.Resources;
using Masofa.DataAccess;
using MaxRev.Gdal.Core;
using MediatR;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using OSGeo.GDAL;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using Directory = System.IO.Directory;

namespace Masofa.BusinessLogic.Uav
{
    public class UploadUavSurveyArchiveCommand : IRequest<Guid>
    {
        public IFormFile ZipFile { get; set; }
        public string? Comment { get; set; }
        public Guid? DataTypeId { get; set; }
        public Guid? CameraTypeId { get; set; }
        public bool AnalysisOnly { get; set; } = true;
        public List<Guid>? TagIds { get; set; }
    }

    public class UploadUavSurveyArchiveCommandHandler : IRequestHandler<UploadUavSurveyArchiveCommand, Guid>
    {
        private readonly MasofaUAVDbContext _uavDbContext;
        private readonly MasofaCommonDbContext _commonDbContext;
        private readonly MasofaCropMonitoringDbContext _cropMonitoringDbContext;
        private readonly MasofaDictionariesDbContext _dictionariesDbContext;
        private readonly MasofaIdentityDbContext _identityDbContext;
        private readonly IMediator _mediator;
        private readonly ILogger<UploadUavSurveyArchiveCommandHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly GeometryFactory _geometryFactory;
        private readonly Regex _channelFileRegex = new Regex(@"(.*)_(\d+)\.(tif|tiff|jpg|jpeg|png)$", RegexOptions.IgnoreCase);

        private record SpatialAnalysisResult(
            Guid? RegionId,
            Guid? FieldId,
            Guid? SeasonId,
            Guid? CropId,
            Guid? FirmId
        );

        public UploadUavSurveyArchiveCommandHandler(
            MasofaUAVDbContext uavDbContext,
            MasofaCommonDbContext commonDbContext,
            MasofaCropMonitoringDbContext cropMonitoringDbContext,
            MasofaDictionariesDbContext dictionariesDbContext,
            MasofaIdentityDbContext identityDbContext,
            IMediator mediator,
            ILogger<UploadUavSurveyArchiveCommandHandler> logger,
            IBusinessLogicLogger businessLogicLogger,
            IHttpContextAccessor httpContextAccessor)
        {
            _uavDbContext = uavDbContext;
            _commonDbContext = commonDbContext;
            _cropMonitoringDbContext = cropMonitoringDbContext;
            _dictionariesDbContext = dictionariesDbContext;
            _identityDbContext = identityDbContext;
            _mediator = mediator;
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
            _httpContextAccessor = httpContextAccessor;
            _geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
        }

        public async Task<Guid> Handle(UploadUavSurveyArchiveCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            string tempZipPath = Path.GetTempFileName();
            string extractPath = Path.Combine(Path.GetTempPath(), $"uav_survey_processing_{Guid.NewGuid()}");
            var uploadedFileIds = new List<Guid>();
            var strategy = _uavDbContext.Database.CreateExecutionStrategy();
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                using (var stream = new FileStream(tempZipPath, FileMode.Create))
                {
                    await request.ZipFile.CopyToAsync(stream, cancellationToken);
                }
                ZipFile.ExtractToDirectory(tempZipPath, extractPath);
                var allFiles = Directory.GetFiles(extractPath, "*.*", SearchOption.AllDirectories)
                    .Where(f => IsImageFile(f))
                    .ToList();
                var fileGroups = GroupFilesByShot(allFiles);
                if (fileGroups.Count == 0)
                {
                    throw new Exception("No valid image files found in archive.");
                }
                return await strategy.ExecuteAsync(async () =>
                {
                    using var transaction = await _uavDbContext.Database.BeginTransactionAsync(cancellationToken);
                    try
                    {
                        var authorUserName = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
                        var user = await _identityDbContext.Set<User>().FirstOrDefaultAsync(u => u.UserName == authorUserName, cancellationToken);
                        var userId = user?.Id ?? Guid.Empty;

                        var newFlyPath = new UAVFlyPath
                        {
                            Id = Guid.NewGuid(),
                            Comment = !string.IsNullOrEmpty(request.Comment)
                                ? request.Comment
                                : "",
                            DataTypeId = request.DataTypeId,
                            CameraTypeId = request.CameraTypeId,
                            CreateAt = DateTime.UtcNow,
                            CreateUser = userId,
                            LastUpdateAt = DateTime.UtcNow,
                            LastUpdateUser = userId,
                            Status = Masofa.Common.Models.StatusType.Active,
                        };
                        await _uavDbContext.Set<UAVFlyPath>().AddAsync(newFlyPath, cancellationToken);
                        await _uavDbContext.SaveChangesAsync(cancellationToken);
                        var flyPathId = newFlyPath.Id;
                        var newCollections = new List<UAVPhotoCollection>();
                        var newRelations = new List<UAVPhotoCollectionRelation>();
                        var routePoints = new List<(DateTime Time, Coordinate Coord)>();
                        foreach (var group in fileGroups)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            var mainFile = group.Files.OrderBy(f => f.ChannelIndex).First().FilePath;
                            (DateTime? originalDate, Point point, double width, double height) = ExtractMetadataFromFile(mainFile);
                            if (point == null)
                            {
                                _logger.LogWarning($"Skipping group {group.ShotName}: No GPS data.");
                                continue;
                            }
                            DateTime finalDate = originalDate.HasValue
                                ? DateTime.SpecifyKind(originalDate.Value, DateTimeKind.Utc)
                                : DateTime.UtcNow;

                            // N+1 проблема. Лучше использовать пакетную обработку в будущем.
                            var spatialData = await AnalyzeSpatialData(point, finalDate, cancellationToken);

                            Guid? previewFileStorageId = null;
                            try
                            {
                                previewFileStorageId = await GenerateAndUploadPreview(mainFile, userId, cancellationToken);
                                if (previewFileStorageId.HasValue)
                                {
                                    uploadedFileIds.Add(previewFileStorageId.Value);
                                }
                            }
                            catch (OperationCanceledException) { throw; }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Failed to generate preview for {mainFile}");
                            }

                            var collection = new UAVPhotoCollection
                            {
                                Id = Guid.NewGuid(),
                                UAVFlyPathId = flyPathId,
                                Point = point,
                                AnalysisOnly = request.AnalysisOnly,
                                PreviewFileStorageId = previewFileStorageId,
                                CreateAt = DateTime.UtcNow,
                                CreateUser = userId,
                                LastUpdateAt = DateTime.UtcNow,
                                LastUpdateUser = userId,
                                Status = Masofa.Common.Models.StatusType.Active
                            };

                            routePoints.Add((finalDate, new Coordinate(point.X, point.Y)));

                            foreach (var fileItem in group.Files.OrderBy(f => f.ChannelIndex))
                            {
                                var meta = ExtractMetadataFromFile(fileItem.FilePath);
                                Guid fileStorageId = await UploadFileToStorage(fileItem.FilePath, userId, cancellationToken);
                                uploadedFileIds.Add(fileStorageId);
                                var photo = new UAVPhoto
                                {
                                    Id = Guid.NewGuid(),
                                    UAVPhotoCollectionId = collection.Id,
                                    FileStorageId = fileStorageId,
                                    Title = Path.GetFileName(fileItem.FilePath),
                                    Width = meta.width > 0 ? meta.width : width,
                                    Height = meta.height > 0 ? meta.height : height,
                                    OriginalDate = DateOnly.FromDateTime(finalDate),
                                    Comment = $"Channel {fileItem.ChannelIndex}",
                                    CreateAt = DateTime.UtcNow,
                                    CreateUser = userId,
                                    LastUpdateAt = DateTime.UtcNow,
                                    LastUpdateUser = userId,
                                    Status = Masofa.Common.Models.StatusType.Active
                                };
                                await _uavDbContext.Set<UAVPhoto>().AddAsync(photo, cancellationToken);
                            }
                            newCollections.Add(collection);
                            if (spatialData.RegionId.HasValue || spatialData.FieldId.HasValue)
                            {
                                var relation = new UAVPhotoCollectionRelation
                                {
                                    Id = Guid.NewGuid(),
                                    UAVPhotoCollectionId = collection.Id,
                                    RegionId = spatialData.RegionId,
                                    FieldId = spatialData.FieldId,
                                    SeasonId = spatialData.SeasonId,
                                    CropId = spatialData.CropId,
                                    FirmId = spatialData.FirmId,
                                    CreateAt = DateTime.UtcNow,
                                    CreateUser = userId,
                                    LastUpdateAt = DateTime.UtcNow,
                                    LastUpdateUser = userId,
                                    Status = Masofa.Common.Models.StatusType.Active
                                };
                                newRelations.Add(relation);
                            }
                        }
                        if (newCollections.Count == 0)
                        {
                            throw new Exception("No valid geotagged photos found in archive after processing.");
                        }
                        await _uavDbContext.Set<UAVPhotoCollection>().AddRangeAsync(newCollections, cancellationToken);
                        if (newRelations.Any())
                        {
                            await _uavDbContext.Set<UAVPhotoCollectionRelation>().AddRangeAsync(newRelations, cancellationToken);
                        }
                        if (request.TagIds != null && request.TagIds.Any())
                        {
                            var relations = request.TagIds.Distinct().Select(tagId => new TagRelation
                            {
                                Id = Guid.NewGuid(),
                                TagId = tagId,
                                OwnerId = flyPathId,
                                OwnerTypeFullName = typeof(UAVFlyPath).FullName ?? "Masofa.Common.Models.Uav.UAVFlyPath",
                                Status = Masofa.Common.Models.StatusType.Active,
                                CreateAt = DateTime.UtcNow,
                                CreateUser = userId,
                                LastUpdateAt = DateTime.UtcNow,
                                LastUpdateUser = userId
                            });
                            await _commonDbContext.TagRelations.AddRangeAsync(relations, cancellationToken);
                            await _commonDbContext.SaveChangesAsync(cancellationToken);
                        }
                        var sortedCoordinates = routePoints
                            .OrderBy(x => x.Time)
                            .Select(x => x.Coord)
                            .ToArray();
                        if (sortedCoordinates.Length > 1)
                        {
                            newFlyPath.FlyPath = _geometryFactory.CreateLineString(sortedCoordinates);
                        }
                        else if (sortedCoordinates.Length == 1)
                        {
                            newFlyPath.FlyPath = _geometryFactory.CreatePoint(sortedCoordinates[0]);
                        }
                        _uavDbContext.Set<UAVFlyPath>().Update(newFlyPath);
                        await _uavDbContext.SaveChangesAsync(cancellationToken);
                        await transaction.CommitAsync(cancellationToken);
                        await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, $"Created FlyPath {flyPathId} with {newCollections.Count} collections"), requestPath);
                        return flyPathId;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        if (uploadedFileIds.Any())
                        {
                            var orphans = string.Join(", ", uploadedFileIds);
                            var orphanMsg = $"TRANSACTION ROLLBACK: The following FileStorage items were uploaded but not linked to DB. Manual cleanup required: {orphans}";
                            _logger.LogCritical(orphanMsg);
                            await _businessLogicLogger.LogCriticalAsync(orphanMsg, requestPath);
                        }

                        throw;
                    }
                });
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                throw;
            }
            finally
            {
                try
                {
                    if (File.Exists(tempZipPath)) File.Delete(tempZipPath);
                    if (Directory.Exists(extractPath)) Directory.Delete(extractPath, true);
                }
                catch (Exception cleanupEx)
                {
                    _logger.LogWarning($"Cleanup failed in UploadUavSurveyArchiveCommandHandler: {cleanupEx.Message}");
                }
            }
        }

        private async Task<Guid?> GenerateAndUploadPreview(string sourceFilePath, Guid userId, CancellationToken token)
        {
            string tempOutputPath = Path.Combine(Path.GetTempPath(), $"preview_{Guid.NewGuid()}.jpg");
            bool isTiff = sourceFilePath.EndsWith(".tif", StringComparison.OrdinalIgnoreCase) ||
                          sourceFilePath.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase);
            try
            {
                if (isTiff)
                {
                    GdalBase.ConfigureAll();
                    using var ds = Gdal.Open(sourceFilePath, Access.GA_ReadOnly);
                    if (ds == null) throw new InvalidOperationException("Failed to open input TIFF with GDAL.");
                    string vrtXml = GenerateScaleVrtXml(ds, sourceFilePath);
                    using var vrtDs = Gdal.Open(vrtXml, Access.GA_ReadOnly);
                    using var driver = Gdal.GetDriverByName("JPEG");
                    if (driver == null || vrtDs == null) throw new Exception("GDAL initialization failed.");
                    using var dstDs = driver.CreateCopy(tempOutputPath, vrtDs, 0, new[] { "QUALITY=85" }, null, null);
                }
                else
                {
                    File.Copy(sourceFilePath, tempOutputPath, overwrite: true);
                }
                if (File.Exists(tempOutputPath))
                {
                    using var fileStream = new FileStream(tempOutputPath, FileMode.Open, FileAccess.Read);
                    var originalName = Path.GetFileNameWithoutExtension(sourceFilePath);
                    var safeFileName = $"{originalName}_{Guid.NewGuid()}_preview.jpg";
                    var formFile = new FormFile(fileStream, 0, fileStream.Length, "file", safeFileName);
                    var uploadCommand = new UploadDocumentCommand
                    {
                        File = formFile,
                        OwnerTypeFullName = typeof(UAVPhotoCollection).FullName,
                        Bucket = "uav-previews"
                    };
                    var fileStorageItem = await _mediator.Send(uploadCommand, token);
                    return fileStorageItem?.Id;
                }
                return null;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to generate or upload preview for {sourceFilePath}");
                return null;
            }
            finally
            {
                try
                {
                    if (File.Exists(tempOutputPath)) File.Delete(tempOutputPath);
                }
                catch
                {
                }
            }
        }

        private string GenerateScaleVrtXml(Dataset ds, string sourcePath)
        {
            int w = ds.RasterXSize;
            int h = ds.RasterYSize;
            int bandCount = ds.RasterCount > 3 ? 3 : ds.RasterCount;
            StringBuilder sb = new StringBuilder();
            sb.Append($@"<VRTDataset rasterXSize=""{w}"" rasterYSize=""{h}"">");
            for (int i = 1; i <= bandCount; i++)
            {
                using (var band = ds.GetRasterBand(i))
                {
                    double minVal = 0;
                    double maxVal = 255;
                    double[] minMax = new double[2];
                    band.ComputeRasterMinMax(minMax, 0);
                    minVal = minMax[0];
                    maxVal = minMax[1];
                    if (maxVal <= minVal)
                    {
                        minVal = 0;
                        maxVal = 255;
                    }
                    double range = maxVal - minVal;
                    double scaleRatio = 255.0 / range;
                    double scaleOffset = -minVal * scaleRatio;
                    string sRatio = scaleRatio.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    string sOffset = scaleOffset.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    sb.Append($@"
  <VRTRasterBand dataType=""Byte"" band=""{i}"">
    <ComplexSource>
      <SourceFilename relativeToVRT=""0"">{sourcePath}</SourceFilename>
      <SourceBand>{i}</SourceBand>
      <ScaleOffset>{sOffset}</ScaleOffset>
      <ScaleRatio>{sRatio}</ScaleRatio>
      <DataType>Byte</DataType>
    </ComplexSource>
  </VRTRasterBand>");
                }
            }
            sb.Append("</VRTDataset>");
            return sb.ToString();
        }

        private bool IsImageFile(string path)
        {
            var ext = Path.GetExtension(path).ToLower();
            return ext == ".jpg" || ext == ".jpeg" || ext == ".tif" || ext == ".tiff" || ext == ".png";
        }

        private class FileGroup
        {
            public string ShotName { get; set; }
            public List<(string FilePath, int ChannelIndex)> Files { get; set; } = new();
        }

        private List<FileGroup> GroupFilesByShot(List<string> filePaths)
        {
            var groups = new Dictionary<string, FileGroup>();
            foreach (var path in filePaths)
            {
                var fileName = Path.GetFileName(path);
                var match = _channelFileRegex.Match(fileName);
                string shotName;
                int channelIndex;
                if (match.Success)
                {
                    shotName = match.Groups[1].Value;
                    channelIndex = int.Parse(match.Groups[2].Value);
                }
                else
                {
                    shotName = Path.GetFileNameWithoutExtension(fileName);
                    channelIndex = 0;
                }
                if (!groups.ContainsKey(shotName))
                {
                    groups[shotName] = new FileGroup { ShotName = shotName };
                }
                groups[shotName].Files.Add((path, channelIndex));
            }
            return groups.Values.ToList();
        }

        private async Task<Guid> UploadFileToStorage(string filePath, Guid userId, CancellationToken token)
        {
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                string originalName = Path.GetFileName(filePath);
                string uniqueFileName = $"{Guid.NewGuid()}_{originalName}";
                var formFile = new FormFile(fileStream, 0, fileStream.Length, "file", uniqueFileName);
                var uploadCommand = new UploadDocumentCommand
                {
                    File = formFile,
                    OwnerTypeFullName = typeof(UAVPhoto).FullName,
                    Bucket = "uav-photos"
                };
                var fileStorageItem = await _mediator.Send(uploadCommand, token);
                return fileStorageItem.Id;
            }
            finally
            {
                await fileStream.DisposeAsync();
            }
        }

        private async Task<SpatialAnalysisResult> AnalyzeSpatialData(Point point, DateTime originalDate, CancellationToken token)
        {
            Guid? fieldId = null;
            Guid? regionId = null;
            Guid? seasonId = null;
            Guid? cropId = null;
            Guid? firmId = null;
            var field = await _cropMonitoringDbContext.Fields
                .FromSqlInterpolated($@"
                    SELECT * FROM ""Fields""
                    WHERE ""Status"" = {(int)Masofa.Common.Models.StatusType.Active}
                    AND ""Polygon"" IS NOT NULL
                    AND ST_Covers(ST_SetSRID(""Polygon""::geometry, 4326), {point})
                    LIMIT 1
                ")
                .AsNoTracking()
                .FirstOrDefaultAsync(token);
            if (field != null)
            {
                fieldId = field.Id;
                regionId = field.RegionId;
                firmId = field.AgricultureProducerId;
                var season = await _cropMonitoringDbContext.Seasons
                    .AsNoTracking()
                    .Where(s => s.FieldId == field.Id
                             && DateOnly.FromDateTime(originalDate) >= s.PlantingDate
                             && DateOnly.FromDateTime(originalDate) <= s.HarvestingDate)
                    .FirstOrDefaultAsync(token);
                if (season != null)
                {
                    seasonId = season.Id;
                    cropId = season.CropId;
                }
            }
            if (regionId == null)
            {
                var regionMap = await _dictionariesDbContext.RegionMaps
                    .FromSqlInterpolated($@"
                        SELECT * FROM ""RegionMaps""
                        WHERE ""Polygon"" IS NOT NULL
                        AND ST_Covers(ST_SetSRID(""Polygon""::geometry, 4326), {point})
                        ORDER BY ST_Area(""Polygon"") ASC
                        LIMIT 1
                    ")
                    .AsNoTracking()
                    .FirstOrDefaultAsync(token);
                if (regionMap != null)
                {
                    var region = await _dictionariesDbContext.Regions
                        .AsNoTracking()
                        .FirstOrDefaultAsync(r => r.RegionMapId == regionMap.Id, token);
                    if (region != null)
                    {
                        regionId = region.Id;
                    }
                }
            }
            return new SpatialAnalysisResult(regionId, fieldId, seasonId, cropId, firmId);
        }

        private (DateTime? date, Point point, double width, double height) ExtractMetadataFromFile(string filePath)
        {
            DateTime? date = null;
            Point point = null;
            double width = 0;
            double height = 0;
            try
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var directories = ImageMetadataReader.ReadMetadata(stream);
                    var gpsDir = directories.OfType<GpsDirectory>().FirstOrDefault();
                    if (gpsDir != null && gpsDir.TryGetGeoLocation(out var location))
                    {
                        point = new Point(location.Longitude, location.Latitude) { SRID = 4326 };
                    }
                    var subIfd = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                    if (subIfd?.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out var dt) == true)
                    {
                        date = dt;
                    }
                    if (date == null)
                    {
                        var ifd0 = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
                        if (ifd0?.TryGetDateTime(ExifDirectoryBase.TagDateTime, out var dt0) == true)
                        {
                            date = dt0;
                        }
                    }
                    var jpegDir = directories.OfType<MetadataExtractor.Formats.Jpeg.JpegDirectory>().FirstOrDefault();
                    if (jpegDir != null)
                    {
                        if (jpegDir.TryGetInt32(MetadataExtractor.Formats.Jpeg.JpegDirectory.TagImageWidth, out int w)) width = w;
                        if (jpegDir.TryGetInt32(MetadataExtractor.Formats.Jpeg.JpegDirectory.TagImageHeight, out int h)) height = h;
                    }
                    else
                    {
                        var ifd0 = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
                        if (ifd0 != null)
                        {
                            if (ifd0.TryGetInt32(ExifDirectoryBase.TagImageWidth, out int w)) width = w;
                            if (ifd0.TryGetInt32(ExifDirectoryBase.TagImageHeight, out int h)) height = h;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"MetadataExtractor error on {filePath}: {ex.Message}");
            }
            bool isTiff = filePath.EndsWith(".tif", StringComparison.OrdinalIgnoreCase) ||
                          filePath.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase);
            if (isTiff || point == null)
            {
                try
                {
                    GdalBase.ConfigureAll();
                    using (var ds = Gdal.Open(filePath, Access.GA_ReadOnly))
                    {
                        if (ds != null)
                        {
                            if (width == 0) width = ds.RasterXSize;
                            if (height == 0) height = ds.RasterYSize;
                            double[] adfGeoTransform = new double[6];
                            ds.GetGeoTransform(adfGeoTransform);
                            if (adfGeoTransform[0] != 0 || adfGeoTransform[3] != 0)
                            {
                                point = new Point(adfGeoTransform[0], adfGeoTransform[3]) { SRID = 4326 };
                            }
                            if (date == null)
                            {
                                string tiffDate = ds.GetMetadataItem("TIFFTAG_DATETIME", "");
                                if (!string.IsNullOrEmpty(tiffDate) &&
                                    DateTime.TryParseExact(tiffDate, "yyyy:MM:dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out var d))
                                {
                                    date = d;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"GDAL extraction failed for {filePath}: {ex.Message}");
                }
            }

            return (date, point, width, height);
        }
    }
}