//namespace Masofa.Web.Monolith.Services.FileStorage
//{
//    public class FileSystemStorageProvider : IFileStorageProvider
//    {
//        private IConfiguration Configuration { get; }
//        private string BasePath { get; }
//        private ILogger Logger { get; }

//        public FileSystemStorageProvider(IConfiguration configuration, ILogger<IFileStorageProvider> logger)
//        {
//            Configuration = configuration;
//            BasePath = configuration.GetValue<string>("FileStorage:FileSystemBasePath");
//            Logger = logger;
//        }

//        public async Task<byte[]> GetFileBytesAsync(string path)
//        {
//            return await System.IO.File.ReadAllBytesAsync(path);
//        }

//        public async Task<Stream> GetFileStreamAsync(string path)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<string> PushFileAsync(byte[] data, string fileName, string sender)
//        {
//            try
//            {
//                var filePath = Path.Combine(BasePath, sender, DateTime.Now.ToString("yyyy_MM_dd"), fileName);

//                EnsureDirectoriesExistRecursive(filePath);

//                await WriteAllBytesAsync(filePath, data);

//                return filePath;
//            }
//            catch (Exception ex)
//            {
//                Logger.LogCritical(ex, $"Something wrong in {GetType().FullName}=>{nameof(PushFileAsync)}");
//                throw ex;
//            }
//        }
//        public Task<string> PushFileAsync(Stream? data, string fileName, string sender)
//        {
//            throw new NotImplementedException();
//        }

//        private async Task WriteAllBytesAsync(string filePath, byte[] bytes)
//        {
//            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
//            {
//                await fileStream.WriteAsync(bytes, 0, bytes.Length);
//            }
//        }

//        private void EnsureDirectoriesExistRecursive(string filePath)
//        {
//            if (string.IsNullOrEmpty(filePath))
//            {
//                throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(filePath));
//            }

//            string directoryPath = Path.GetDirectoryName(filePath);

//            if (string.IsNullOrEmpty(directoryPath))
//            {
//                return;
//            }

//            string[] pathParts = directoryPath.Split(Path.DirectorySeparatorChar);
//            string currentPath = string.Empty;

//            foreach (string part in pathParts)
//            {
//                currentPath = Path.Combine(currentPath, part);

//                if (!Directory.Exists(currentPath))
//                {
//                    Directory.CreateDirectory(currentPath);
//                }
//            }
//        }
//    }
//}
