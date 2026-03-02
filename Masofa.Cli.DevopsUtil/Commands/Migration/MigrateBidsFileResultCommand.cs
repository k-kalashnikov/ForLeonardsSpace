using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.Identity;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using Masofa.Depricated.DataAccess.DepricatedAuthServerOne;
using Masofa.Depricated.DataAccess.DepricatedAuthServerTwo;
using Masofa.Depricated.DataAccess.DepricatedUalertsServerOne;
using Masofa.Depricated.DataAccess.DepricatedUdictServerTwo;
using Masofa.Depricated.DataAccess.DepricatedUmapiServerOne;
using Masofa.Depricated.DataAccess.DepricatedUmapiServerTwo;
using Microsoft.AspNetCore.Identity;
using Minio;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Cli.DevopsUtil.Commands.Migration
{
    [BaseCommand("Migrate Bids Result", "Миграция результатов файлов заявок из старой системы в новую")]
    public class MigrateBidsFileResultCommand : IBaseCommand
    {
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
        private UserManager<Masofa.Common.Models.Identity.User> UserManager { get; set; }
        private DepricatedUmapiServerOneDbContext DepricatedUmapiServerOneDbContext { get; set; }
        private DepricatedAuthServerOneDbContext DepricatedAuthServerOneDbContext { get; set; }
        private DepricatedUmapiServerTwoDbContext DepricatedUmapiServerTwoDbContext { get; set; }
        private DepricatedAuthServerTwoDbContext DepricatedAuthServerTwoDbContext { get; set; }
        private DepricatedUdictServerTwoDbContext DepricatedUdictServerTwoDbContext { get; set; }
        private RoleManager<Masofa.Common.Models.Identity.Role> RoleManager { get; set; }
        private DepricatedUalertsServerOneDbContext DepricatedUalertsServerOneDbContext { get; set; }
        IFileStorageProvider FileStorageProvider { get; set; }


        public MigrateBidsFileResultCommand(MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext, MasofaCommonDbContext masofaCommonDbContext,
            MasofaDictionariesDbContext masofaDictionariesDbContext, UserManager<User> userManager, RoleManager<Role> roleManager,
            DepricatedAuthServerTwoDbContext depricatedAuthServerTwoDbContext,
            DepricatedUmapiServerTwoDbContext depricatedUmapiServerTwoDbContext,
            DepricatedUmapiServerOneDbContext depricatedUmapiServerOneDbContext,
            DepricatedAuthServerOneDbContext depricatedAuthServerOneDbContext,
            DepricatedUalertsServerOneDbContext depricatedUalertsServerOneContext,
            DepricatedUdictServerTwoDbContext depricatedUdictServerTwoDbContext, IFileStorageProvider fileStorageProvider)
        {
            MasofaCropMonitoringDbContext = MasofaCropMonitoringDbContext;
            MasofaCommonDbContext = masofaCommonDbContext;
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
            UserManager = userManager;
            RoleManager = roleManager;
            DepricatedUmapiServerOneDbContext = depricatedUmapiServerOneDbContext;
            DepricatedAuthServerOneDbContext = depricatedAuthServerOneDbContext;
            DepricatedUalertsServerOneDbContext = depricatedUalertsServerOneContext;

            DepricatedAuthServerTwoDbContext = depricatedAuthServerTwoDbContext;
            DepricatedUmapiServerTwoDbContext = depricatedUmapiServerTwoDbContext;
            DepricatedUdictServerTwoDbContext = depricatedUdictServerTwoDbContext;
            FileStorageProvider = fileStorageProvider;
        }

        public async Task Execute()
        {
            var parameters = MigrateBidsParameters.GetFromUser();
            await ExecuteCore(parameters);
        }

        public async Task Execute(string[] args)
        {
            var parameters = MigrateBidsParameters.Parse(args);
            await ExecuteCore(parameters);
        }

        private async Task ExecuteCore(MigrateBidsParameters parameters)
        {
            await MigrateBidFilesAsync(parameters);
        }

        /// <summary>
        /// Выполняет миграцию с параметрами из JSON
        /// </summary>
        /// <param name="parametersJson">JSON с параметрами: {"minioUrl": "...", "minioLogin": "...", "minioPassword": "..."}</param>
        public async Task ExecuteWithParameters(string parametersJson)
        {
            try
            {
                var parameters = JsonConvert.DeserializeObject<MigrateBidsParameters>(parametersJson);
                await MigrateBidFilesAsync(parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error parsing parameters: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Параметры для миграции
        /// </summary>
        public class MigrateBidsParameters
        {
            public string MinioUrl { get; set; } = string.Empty;
            public string MinioLogin { get; set; } = string.Empty;
            public string MinioPassword { get; set; } = string.Empty;

            public static MigrateBidsParameters Parse(string[] args)
            {
                if (args.Length < 3)
                    throw new ArgumentException("Необходимо указать: <minioUrl> <minioLogin> <minioPassword>");

                return new MigrateBidsParameters
                {
                    MinioUrl = args[0],
                    MinioLogin = args[1],
                    MinioPassword = args[2]
                };
            }

            public static MigrateBidsParameters GetFromUser()
            {
                Console.WriteLine("Enter pls Minio Url");
                var minioUrl = Console.ReadLine() ?? string.Empty;
                Console.WriteLine("Enter pls Minio Login");
                var minioLogin = Console.ReadLine() ?? string.Empty;
                Console.WriteLine("Enter pls Minio PassWord");
                var minioPassword = Console.ReadLine() ?? string.Empty;

                return new MigrateBidsParameters
                {
                    MinioUrl = minioUrl,
                    MinioLogin = minioLogin,
                    MinioPassword = minioPassword
                };
            }
        }

        public void Dispose()
        {

        }

        #region MigrateFields


        private async Task MigrateBidFilesAsync(MigrateBidsParameters parameters)
        {
            BidResultDownloaderWithBasicAuth loader = new BidResultDownloaderWithBasicAuth("masofaapi", "strongAPIpassw0rd");
            
            // Используем параметры вместо Console.ReadLine
            var minioUrl = parameters.MinioUrl;
            var minioLogin = parameters.MinioLogin;
            var minioPassword = parameters.MinioPassword;

            var minioClient = new MinioClient()
                .WithEndpoint(minioUrl)
                .WithCredentials(minioLogin, minioPassword)
                .WithSSL(false)
                .Build();

            FileStorageProvider = new MinIOStorageProvider(minioClient);

            var bidsWithFiles = new List<(int Number, Depricated.DataAccess.DepricatedUmapiServerTwo.Models.Bid OldBid, Bid NewBid)>();
            var bids = DepricatedUmapiServerTwoDbContext.Bids.ToList();
            var bidFiles = DepricatedUmapiServerTwoDbContext.BidFiles.ToList();
            var newBids = MasofaCropMonitoringDbContext.Bids.ToList();

            foreach (var bidFile in bidFiles)
            {
                var oldBid = bids.First(m => m.Id.Equals(bidFile.BidId));
                var newBid = newBids.First(m => m.Number.Equals(oldBid.Number));

                if (!oldBid.CropId.HasValue)
                {
                    continue;
                }

                bidsWithFiles.Add(new()
                {
                    Number = (int)oldBid.Number,
                    OldBid = oldBid,
                    NewBid = newBid,
                });
            }

            foreach (var item in bidsWithFiles)
            {
                using (Stream stream = await loader.DownloadBidResultAsync(item.OldBid.CropId.Value, item.OldBid.Id))
                {
                    var fileName = $"result_{item.NewBid.Id}";
                    fileName = await FileStorageProvider.PushFileAsync(stream, fileName, "bids");

                    var fileItem = new FileStorageItem()
                    {
                        FileContentType = FileContentType.ArchiveZIP,
                        FileStoragePath = fileName,
                        FileStorageBacket = "bids",
                        OwnerTypeFullName = typeof(Bid).FullName,
                        OwnerId = item.NewBid.Id,
                        Status = Common.Models.StatusType.Active,
                        CreateAt = item.NewBid.LastUpdateAt,
                        CreateUser = item.NewBid.LastUpdateUser,
                        LastUpdateAt = item.NewBid.LastUpdateAt,
                        LastUpdateUser = item.NewBid.LastUpdateUser,
                    };

                    MasofaCommonDbContext.FileStorageItems.Add(fileItem);
                    MasofaCommonDbContext.SaveChanges();
                }
            }
        }
        #endregion

        private Guid ResolveUserId(string? userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return Guid.Empty;
            }
            return UserManager.Users.FirstOrDefault(m => m.UserName.Equals(userName))?.Id ?? Guid.Empty;
        }
    }

    public class BidResultDownloaderWithBasicAuth
    {
        private readonly HttpClient _httpClient;
        private string _baseUrl = "https://masofa-yer.agro.uz"; // Заменить на реальный URL

        public BidResultDownloaderWithBasicAuth(string username, string password)
        {
            _httpClient = new HttpClient();

            // Устанавливаем Basic Auth заголовок
            var credentials = $"{username}:{password}";
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encoded);
        }

        public async Task<Stream> DownloadBidResultAsync(Guid cropId, Guid bidId)
        {
            var url = $"{_baseUrl}/api/v1/dev/crops/bids/{cropId}/{bidId}/result";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    // Проверка типа содержимого
                    var contentType = response.Content.Headers.ContentType?.MediaType;
                    if (contentType != "application/zip")
                    {
                        Console.WriteLine($"Предупреждение: ожидается application/zip, получено: {contentType}");
                    }

                    return response.Content.ReadAsStream();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine("❌ Ошибка: файл не найден (404).");
                    return null;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("❌ Ошибка: доступ запрещён (401). Проверьте логин и пароль.");
                    return null;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Ошибка {response.StatusCode}: {errorContent}");
                    return null;
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"🌐 Ошибка HTTP: {httpEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❗ Неожиданная ошибка: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DownloadBidResultAsync(Guid cropId, Guid bidId, string outputPath)
        {
            var url = $"{_baseUrl}/api/v1/dev/crops/bids/{cropId}/{bidId}/result";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    // Проверка типа содержимого
                    var contentType = response.Content.Headers.ContentType?.MediaType;
                    if (contentType != "application/zip")
                    {
                        Console.WriteLine($"Предупреждение: ожидается application/zip, получено: {contentType}");
                    }

                    // Сохраняем как ZIP
                    await using var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
                    await response.Content.CopyToAsync(fs);
                    Console.WriteLine($"✅ Файл успешно сохранён: {outputPath}");
                    return true;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine("❌ Ошибка: файл не найден (404).");
                    return false;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("❌ Ошибка: доступ запрещён (401). Проверьте логин и пароль.");
                    return false;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Ошибка {response.StatusCode}: {errorContent}");
                    return false;
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"🌐 Ошибка HTTP: {httpEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❗ Неожиданная ошибка: {ex.Message}");
                return false;
            }
        }

        //private async Task<bool> ExecuteSqlScriptFromFile(string filePath)
        //{
        //    if (!File.Exists(filePath))
        //    {
        //        Console.WriteLine($"SQL файл не найден: {filePath}");
        //        return false;
        //    }

        //    try
        //    {
        //        var sql = await File.ReadAllTextAsync(filePath);
        //        await MasofaCropMonitoringDbContext.Database.ExecuteSqlRawAsync(sql);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Ошибка выполнения SQL скрипта: {ex.Message}");
        //        return false;
        //    }
        //}
    }
}
