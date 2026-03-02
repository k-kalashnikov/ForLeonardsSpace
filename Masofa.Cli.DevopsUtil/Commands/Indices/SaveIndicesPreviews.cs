using Masofa.Common.Models;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Minio;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Linq.Expressions;

namespace Masofa.Cli.DevopsUtil.Commands.Indices
{
    [BaseCommand("Create previews for indices", "Create previews for indices")]
    public class SaveIndicesPreviews : IBaseCommand
    {
        private IFileStorageProvider FileStorageProvider { get; set; }
        private ILogger<SaveIndicesPreviews> Logger { get; set; }

        private MasofaCommonDbContext CommonDbContext { get; set; }
        private MasofaIndicesDbContext IndicesDbContext { get; set; }

        public SaveIndicesPreviews(MasofaIndicesDbContext indicesDbContext, MasofaCommonDbContext commonDbContext, ILogger<SaveIndicesPreviews> logger, IFileStorageProvider fileStorageProvider)
        {
            IndicesDbContext = indicesDbContext;
            CommonDbContext = commonDbContext;
            Logger = logger;
            FileStorageProvider = fileStorageProvider;
        }

        public void Dispose()
        {
            Console.WriteLine("\nSaveIndicesPreviews END");
        }

        public async Task Execute()
        {
            Console.WriteLine("SaveIndicesPreviews START\n");

            await ProcessIndicesPreviews();
        }

        private async Task ProcessIndicesPreviews()
        {
            var minioClient = new MinioClient()
                .WithEndpoint("185.100.234.107:20040")
                .WithCredentials("sixgrain-test", "R4tY6uI8oP2Q")
                .WithSSL(false)
                .Build();

            FileStorageProvider = new MinIOStorageProvider(minioClient);

            string[] indices = ["ARVI", "EVI", "GNDVI", "MNDWI", "NDMI", "NDVI", "ORVI", "OSAVI"];
            foreach (var index in indices)
            {
                Console.Write($"{index} ");
                string typeName = $"{index[0].ToString().ToUpper()}{index[1..].ToLower()}Polygon";
                var dbSet = IndicesDbContext.GetQueryableSet(typeName);

                var type = FindTypeByName(typeName);

                if (type == null)
                {
                    continue;
                }

                var param = Expression.Parameter(type, "h");

                var prop1 = Expression.Property(param, "PreviewImagePath");
                var value1 = Expression.Constant(null);
                var condition1 = Expression.Equal(prop1, value1);

                var predicate = Expression.Lambda(condition1, param);

                var whereMethod = typeof(Queryable).GetMethods()
                    .First(m => m.Name == "Where" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(type);

                var filteredQuery = (IQueryable)whereMethod.Invoke(null, [dbSet, predicate]);

                var listMethod = typeof(Enumerable).GetMethod("ToList")!
                    .MakeGenericMethod(type);

                if (listMethod.Invoke(null, [filteredQuery]) is IEnumerable<BaseIndexPolygon> result)
                {
                    Console.Write($"{result.Count()} ");
                    foreach (var item in result)
                    {
                        var productId = item.SatelliteProductId;
                        try
                        {
                            var isColored = item.IsColored;
                            var fsi = await CommonDbContext.FileStorageItems.FirstOrDefaultAsync(f => f.Id == item.FileStorageItemId);
                            if (fsi == null)
                            {
                                continue;
                            }

                            var fileBytes = await FileStorageProvider.GetFileBytesAsync(fsi);

                            await using var inputStream = new MemoryStream(fileBytes);

                            using var image = await Image.LoadAsync<Rgba32>(inputStream);
                            image.Mutate(x => x.Resize(new ResizeOptions
                            {
                                Size = new Size(250, 250),
                                Mode = ResizeMode.Max
                            }));

                            string previewImagePath = Path.Combine(Path.GetTempPath(), $"{productId}_{index}{(isColored ? "_Colored" : "")}_preview.png");
                            await image.SaveAsPngAsync(previewImagePath);

                            Logger.LogInformation($"{index} index preview image saved to: {previewImagePath}");

                            var previewBucketName = $"sentinel{index.ToLower()}{(isColored ? "colored" : "")}preview";
                            var previewObjectName = $"{productId}.png";

                            using var previewFileStream = File.OpenRead(previewImagePath);
                            string minioPreviewfPath = await FileStorageProvider.PushFileAsync(previewFileStream, previewObjectName, previewBucketName);
                            Logger.LogInformation($"Preview uploaded to MinIO: {previewBucketName}/{previewObjectName}");
                            var previewFileLength = new FileInfo(previewImagePath).Length;

                            var tiffFileStorageItem = new FileStorageItem()
                            {
                                CreateAt = DateTime.UtcNow,
                                CreateUser = Guid.Empty,
                                OwnerId = productId,
                                OwnerTypeFullName = typeof(SatelliteProduct).FullName,
                                FileContentType = FileContentType.ImagePNG,
                                Status = StatusType.Active,
                                FileStoragePath = minioPreviewfPath,
                                FileStorageBacket = previewBucketName,
                                FileLength = previewFileLength,
                            };

                            tiffFileStorageItem = (await CommonDbContext.FileStorageItems.AddAsync(tiffFileStorageItem)).Entity;
                            await CommonDbContext.SaveChangesAsync();

                            item.PreviewImagePath = tiffFileStorageItem.Id;
                            await IndicesDbContext.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, $"Error in {index} index preview create for product = {productId}");
                        }
                    }
                }

            }
            Console.WriteLine();
        }

        public static Type? FindTypeByName(string name)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(name);
                if (type != null)
                {
                    return type;
                }

                type = assembly.GetTypes().FirstOrDefault(t => t.Name == name);

                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
