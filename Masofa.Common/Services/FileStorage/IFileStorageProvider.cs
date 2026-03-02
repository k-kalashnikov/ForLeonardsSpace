using Masofa.Common.Models.SystemCrical;

namespace Masofa.Common.Services.FileStorage
{
    public interface IFileStorageProvider
    {
        public Task<byte[]> GetFileBytesAsync(FileStorageItem file);
        public Task<Stream> GetFileStreamAsync(FileStorageItem file);
        public Task<string> PushFileAsync(byte[] data, string fileName, string backet);
        public Task<string> PushFileAsync(Stream? data, string fileName, string backet);
        public Task<IEnumerable<string>> ListFilesAsync(string bucket, string? prefix = null, int take = 100);
        public Task<byte[]> GetFileBytesAsyncWithProgress(FileStorageItem file);
        public Task<Stream> GetFileStreamAsyncWithProgress(FileStorageItem file);
        public Task<byte[]> GetFileBytesAsync(string fileName, string backet);
        public Task<Stream> GetFileStreamAsync(string fileName, string backet);
        public Task CopyObjectAsync(string sourceBucket, string sourceKey, string destBucket, string destKey);
        public Task DeleteObjectAsync(string bucket, string key);
    }
}
