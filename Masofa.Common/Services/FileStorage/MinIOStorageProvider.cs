using Masofa.Common.Extentions;
using Masofa.Common.Models.SystemCrical;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using System.Net.Sockets;

namespace Masofa.Common.Services.FileStorage
{
    public class MinIOStorageProvider : IFileStorageProvider
    {
        private IMinioClient MinioClient { get; set; }

        public MinIOStorageProvider(IMinioClient minioClient)
        {
            MinioClient = minioClient;
        }

        public async Task<byte[]> GetFileBytesAsync(FileStorageItem file)
        {
            using var stream = await GetFileStreamAsync(file);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            return ms.ToArray();
        }

        public async Task<Stream> GetFileStreamAsync(FileStorageItem file)
        {
            var ms = new MemoryStream();
            var args = new GetObjectArgs()
                .WithBucket(file.FileStorageBacket)
                .WithObject(file.FileStoragePath)
                .WithCallbackStream(stream => stream.CopyTo(ms));

            await MinioClient.GetObjectAsync(args);

            ms.Position = 0;
            return ms;
        }

        public async Task<string> PushFileAsync(byte[] data, string fileName, string backet)
        {
            using var stream = new MemoryStream(data);
            return await PushFileAsync(stream, fileName, backet);
        }

        public async Task<string> PushFileAsync(Stream? data, string fileName, string backet)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            await CreateBucketIfNotExistsAsync(backet);

            var args = new PutObjectArgs()
                .WithBucket(backet)
                .WithObject(fileName)
                .WithStreamData(data)
                .WithObjectSize(data.Length)
                .WithContentType("application/octet-stream");

            await MinioClient.PutObjectAsync(args);

            // Очищаем временный файл после использования
            if (!data.CanSeek && data is FileStream tempFileStream)
            {
                var tempFilePath = tempFileStream.Name;
                data.Dispose();
                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);
            }

            return fileName;
        }


        public async Task<IEnumerable<string>> ListFilesAsync(string bucket, string? prefix = null, int take = 100)
        {
            var files = new List<string>();
            var args = new Minio.DataModel.Args.ListObjectsArgs()
                .WithBucket(bucket)
                .WithPrefix(prefix ?? string.Empty)
                .WithRecursive(true);

            var observable = MinioClient.ListObjectsEnumAsync(args);
            int count = 0;
            await foreach (var item in observable)
            {
                if (!item.IsDir)
                {
                    files.Add(item.Key);
                    count++;
                    if (count >= take)
                        break;
                }
            }
            return files;
        }

        private async Task CreateBucketIfNotExistsAsync(string backetName)
        {
            try
            {
                var exists = await MinioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(backetName));
                if (!exists)
                {
                    await MinioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(backetName));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not create bucket {backetName}: {ex.Message}");
            }
        }

        public async Task<byte[]> GetFileBytesAsyncWithProgress(FileStorageItem file)
        {
            // Сначала получим размер файла, чтобы знать максимум для прогресса
            var statArgs = new StatObjectArgs().WithBucket(file.FileStorageBacket)
                .WithObject(file.FileStoragePath);
            var stat = await MinioClient.StatObjectAsync(statArgs);
            long totalSize = stat.Size;



            var ms = new MemoryStream();
            long lastReported = 0;

            var progressStream = new ProgressStream(ms, (current) =>
            {
                // Обновляем прогресс каждые 1% или при завершении
                const int reportIntervalPercent = 1;
                long interval = totalSize / (100 / reportIntervalPercent);

                if (current - lastReported >= interval || current == totalSize)
                {
                    int percent = (int)(100 * current / totalSize);
                    string progressLine = $"\rDownloading {file.FileStoragePath}: {percent,3}% ({current} / {totalSize} bytes)";
                    Console.Write(progressLine.PadRight(Console.WindowWidth - 1));
                    lastReported = current;
                }
            });

            var args = new GetObjectArgs()
                .WithBucket(file.FileStorageBacket)
                .WithObject(file.FileStoragePath)
                .WithCallbackStream(stream => stream.CopyTo(progressStream));

            await MinioClient.GetObjectAsync(args);

            Console.WriteLine(); // Перевод строки после завершения

            ms.Position = 0;
            return ms.ToArray();
        }

        public async Task<Stream> GetFileStreamAsyncWithProgress(FileStorageItem file)
        {
            // 1. Получаем размер файла для расчёта прогресса
            var statArgs = new StatObjectArgs()
                .WithBucket(file.FileStorageBacket)
                .WithObject(file.FileStoragePath);
            var stat = await MinioClient.StatObjectAsync(statArgs);
            long totalSize = stat.Size;

            if (totalSize == 0)
            {
                return new MemoryStream(); // пустой поток
            }

            var ms = new MemoryStream();
            long lastReported = 0;
            const int reportIntervalPercent = 1;
            long reportIntervalBytes = Math.Max(1, totalSize / (100 / reportIntervalPercent));

            // Оборачиваем MemoryStream в ProgressStream
            var progressStream = new ProgressStream(ms, (current) =>
            {
                if (current - lastReported >= reportIntervalBytes || current == totalSize)
                {
                    int percent = (int)(100 * current / totalSize);
                    string progressLine = $"\rDownloading stream for {file.FileStoragePath}: {percent,3}% ({current} / {totalSize} bytes)";
                    // Очищаем "хвост" предыдущей строки
                    Console.Write(progressLine.PadRight(Console.WindowWidth - 1));
                    lastReported = current;
                }
            });

            // 2. Загружаем объект, копируя данные в обёрнутый поток
            var args = new GetObjectArgs()
                .WithBucket(file.FileStorageBacket)
                .WithObject(file.FileStoragePath)
                .WithCallbackStream(sourceStream => sourceStream.CopyTo(progressStream));

            await MinioClient.GetObjectAsync(args);

            // 3. Завершаем прогресс — переходим на новую строку
            Console.WriteLine();

            // 4. Сбрасываем позицию и возвращаем оригинальный MemoryStream
            ms.Position = 0;
            return ms;
        }

        public async Task<byte[]> GetFileBytesAsync(string fileName, string bucket)
        {
            using var stream = await GetFileStreamAsync(fileName, bucket);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            return ms.ToArray();
        }

        public async Task<Stream> GetFileStreamAsync(string fileName, string bucket)
        {
            var ms = new MemoryStream();
            var args = new GetObjectArgs()
                .WithBucket(bucket)
                .WithObject(fileName)
                .WithCallbackStream(stream => stream.CopyTo(ms));

            await MinioClient.GetObjectAsync(args);

            ms.Position = 0;
            return ms;
        }

        public async Task CopyObjectAsync(string sourceBucket, string sourceKey, string destBucket, string destKey)
        {
            var copySource = new CopySourceObjectArgs()
                .WithBucket(sourceBucket)
                .WithObject(sourceKey);

            var copyArgs = new CopyObjectArgs()
                .WithBucket(destBucket)
                .WithObject(destKey)
                .WithCopyObjectSource(copySource);

            await MinioClient.CopyObjectAsync(copyArgs);
        }

        public async Task DeleteObjectAsync(string bucket, string key)
        {
            var args = new RemoveObjectArgs()
            .WithBucket(bucket)
            .WithObject(key);

            await MinioClient.RemoveObjectAsync(args);
        }
    }
}
