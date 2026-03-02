
using Masofa.Common.Attributes;
using Masofa.Common.Models;
using Masofa.Common.Models.SystemCrical;
using Masofa.Depricated.DataAccess.DepricatedUmapiServerTwo;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Masofa.Cli.DevopsUtil.Commands.Export
{
    public class BidsWithArichiveExportCommandParameters
    {
        [TaskParameter("Путь для сохранения отчета", true, "C:\\Reports")]
        public string BaseReportPath { get; set; } = string.Empty;

        public static BidsWithArichiveExportCommandParameters Parse(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
                throw new ArgumentException("Необходимо указать путь для сохранения отчета");

            return new BidsWithArichiveExportCommandParameters { BaseReportPath = args[0] };
        }

        public static BidsWithArichiveExportCommandParameters GetFromUser()
        {
            Console.WriteLine("Enter pls path to save");
            var baseReportPath = Console.ReadLine() ?? string.Empty;
            return new BidsWithArichiveExportCommandParameters { BaseReportPath = baseReportPath };
        }
    }

    [BaseCommand("Bids With Archive Export", "Экспорт заявок с архивными данными", typeof(BidsWithArichiveExportCommandParameters))]
    public class BidsWithArichiveExportCommand : IBaseCommand
    {
        private string jwtToken = "eyJhbGciOiJBMjU2S1ciLCJlbmMiOiJBMjU2Q0JDLUhTNTEyIiwidHlwIjoiYXQrand0IiwiY3R5IjoiSldUIn0.6ABvNs3dSq6654q69nRn-sUsVujByk_6kvZ0zOhEqhDYWmc-nwuJE5jhLXVRbnr5zo5TF1QXffnCZP7jXToPYkpnTD3xRbIr.BAP3qcshRWH6rbWPrYAv4A.H52MsNVMKfam6hHJvMusXV17N1kPplYevqSHF_XyL-qF8LlC0cIHOtnOjciR4-Tm8wgOaXH-hHGL6FVCcZU_7Rb7pgjpxJnUAhUwtPff4MB-xmorqbCRclpM6rvFBsAQ6s2x-6p4j30ZdzTM_v8_uxSOVdhNSCZin2AdX3JNN8hKh7fsa2iMqEm6Fupc860sGN5eut6tVIONdGsXH-7bm4R4h0XmrxhLzvGQe_zuHbNPMIWEBVIKmqkQ9Zj6K6Y-Yebszqi1-hMcTO_4-TCq6Z5V7fknobs73_sYMYMB_vk44s-vcPJ7-aJcz2MxtMiAr2iOtqxjU5_xq541HKA-RBpE4s3Fo2ba8FQp3YIrfXgTEhnJMVgFFBNXjRojF5RAQNAVRAmq8QtIBJ6b3RTGz8Otl7vrThtwmiLRSs9mIqlPJH0DEZzca75ee2_DFzcJA-XGJ_aYOSafz8Kko6Hr_QcU3yQJgKPAKnsFuKyyaxlBm_S57AwIckB1i7syii-t-O47qjSlcjqSGOTGb_OPQ0x1agFbUQU9OuZ2PsnW9v5OYO0IsxuSNoVJvdPRtZRxI2zUvWfgG7w0OCGsNLEd1nlrD8rn7YsyNxAyZFtXIsDXBgY4qj-Au1TZaGBt9ffxJjN-xeDeuWQvXAJUmg86LFQEQh1xaAwtunu-fTwMZeM_U4zqHq_CZri8D_uiDt8IzABjPhslWOC7kkNG1J9fTqHP_J-O5pRfEHDzflfMIHjqolmPU_xeJGfcCPBJLG7axSy2cx5hUQBj-yAK7iPsc_8XQ-LbgbRBe0ADEd0XKllSY1nI3t1V1mp7A-dCdiodNzsHE0m2fKDJU9bdzP2fRnSF5IOluQNb85wTqsgdJQJZz-4lK8dc9lgCDuqe1t4FHmcI0buWZscWbX_sgW72krVzmW9XNTFmh-qvTofF_IotzfiApQvT4U79h3_E17-j7JlZ1vUfIDS3gtiSndA_bHf2SmXQu_eCVHIvs9W-VNGE9B6OfflMMZ7thYM9mydpEw7cQzuavFbvUGB24-wvLJ50vPiyEE1XrXkbMdyucShn2ii9_IFGif-E2odJfQ4vS3a44Y1xeO29MFWRbp57cokXjiZe6AnqvpTAnIkZ4IwtQHBttG8jIVomdvB2vNDMrP0QlhAVct0bn56SBDfzfemMaRUD_o05XdSkhOsLuFlcJmj4Jz6D2GkIN0bcv4TobXHNdDn5EYme-Hh5Fg1XkL4F6TW90eKITmGkOqfNw1Yomblix9j5ibOnp5SiVH6NJmEAhoXN_FIE5x-jg-d7kZ1EcAm1Sz3LpMj2IDCpRdBR3BZnSGafXG_oC7tgdeawOYIyb2hocVhha2doiJOMWMWa83UAjtR8UUZRVvATaDY3ssqt0qFWzc2c2lypVMJNhhOsHW9-L3P-P2oPpriaOhJJPu0vxtPxGh4X9TftpOKo93x0cGalKHSfPA41hUhP8RxtGyxgBl_hRtOwiypdazxK7wQvwFz36OkUt6GwVXXe-7UYESz_ltCkNLqN6VZEGIxqnKCxDDpAaMUakcLwDNRtTuzWkqpIfKO_uhu_ws34Q5gQE8seY1Fy8LJWYjuZ3hmZDccZhakqepJf1MJMyQHMIb3udKGVBhSvJFkWo_s._-V0nMgYmof34Ik_aN0UdF6ZDJlH0OiJeWbiRkER5pA";

        private List<long> bidNumbers = new List<long> {
                316,320,322,358,361,373,374,375,393,394,395,291,396,338,335,349,345,380,384,387,388,390,392,368,365,3369,354,353,342,382,386,381,367,366,399,402,401,398,407,408,409,411,412,418,413,414,415,416,417,419,420,421,422,427,428,429,430,433,424,426,432,434,435,436,437,442,443,445,447,446,459,460,461,456,455,462,463,464,465,466,468,470,471,472,473,475,469,480,481,482,483,484,487,488,489,490,491,492,567,568,569,570,571
        };

        private string url = "https://masofa-yer.agro.uz/api/v1/crops/bids/filter?pageSize=1000";
        private string bidStatesUrl = "https://masofa-yer.agro.uz/api/dictionaries/v1/bid-states/";
        private string cropsUrl = "https://masofa-yer.agro.uz/api/dictionaries/v1/crops/";
        private string baseReportPath = string.Empty;


        private DepricatedUmapiServerTwoDbContext UmapiServerTwoDbContext { get; set; }
        
        public BidsWithArichiveExportCommand(DepricatedUmapiServerTwoDbContext depricatedUmapiServerTwo)
        {
            UmapiServerTwoDbContext = depricatedUmapiServerTwo;
        }

        private HttpClient client = new HttpClient()
        {
            Timeout = TimeSpan.FromMinutes(5)
        };

        public void Dispose()
        {

        }

        public async Task Execute()
        {
            var parameters = BidsWithArichiveExportCommandParameters.GetFromUser();
            await ExecuteCore(parameters);
        }

        public async Task Execute(string[] args)
        {
            var parameters = BidsWithArichiveExportCommandParameters.Parse(args);
            await ExecuteCore(parameters);
        }

        private async Task ExecuteCore(BidsWithArichiveExportCommandParameters parameters)
        {
            baseReportPath = parameters.BaseReportPath;
            var bids = await UmapiServerTwoDbContext.Bids.ToListAsync();
            if (bids == null || bids.Count == 0) return;

            var resultCsv = "Id;CreateDate;ModifyDate;Active;StartDate;DeadlineDate;EndDate;Comment;Description;Lat;Lng;Published;Cancelled;Number;FieldPlantingDate;GeoJson;ContourId;" +
                "crop.name;zip_file_name;fill_photo";

            var foremanIds = new HashSet<Guid>();
            var num = 1;
            foreach (var bid in bids)
            {
                Console.Write($"{num++,4}/{bids.Count,4} bidId: {bid.Id} cropId: {bid.CropId}");
                var line = $"{bid.Id};" +
                    $"{bid.CreateDate};" +
                    $"{bid.ModifyDate};" +
                    $"{bid.Active};" +
                    $"{bid.StartDate};" +
                    $"{bid.DeadlineDate};" +
                    $"{bid.EndDate};" +
                    $"{bid.Comment};" +
                    $"{bid.Description};" +
                    $"{bid.Lat};" +
                    $"{bid.Lng};" +
                    $"{bid.Published};" +
                    $"{bid.Cancelled};" +
                    $"{bid.Number};" +
                    $"{bid.FieldPlantingDate};" +
                    $"{bid.GeoJson};" +
                    $"{bid.ContourId}";

                var bidStateNameRu = await GetBidState(bid.BidStateId);
                line += $";{bidStateNameRu}";

                line += $";";
                if (bid.CropId != null)
                {
                    var cropNameRu = await GetCropNameRu(bid.CropId.Value);
                    line += $"{cropNameRu}";
                    var archRes = await GetBidArchive(bid.CropId.Value, bid.Id, (int)bid.Number);
                    line += $";{archRes.Name};{archRes.HasImages}";
                }
                else
                {
                    line += ";;";
                }

                resultCsv += "\n" + line;
            }

            string csvPath = Path.Combine(baseReportPath, "report.csv");
            Directory.CreateDirectory(Path.GetDirectoryName(csvPath)!);
            await File.WriteAllTextAsync(csvPath, resultCsv, Encoding.UTF8);
            Console.WriteLine($"CSV сохранён: {csvPath}");
        }

        private async Task<ArchuveResult> GetBidArchive(Guid cropId, Guid bidId, int bidNumber)
        {
            var hasImages = false;
            var fileName = $"bid_{bidNumber}_{bidId}.zip";
            var filePath = Path.Combine(baseReportPath, fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            var isFileExist = File.Exists(filePath);
            Console.WriteLine($" isFileExist: {isFileExist}");
            if (!isFileExist)
            {
                try
                {
                    var resultUrl = $"https://masofa-yer.agro.uz/api/v1/crops/bids/{cropId}/{bidId}/result";
                    using var request = new HttpRequestMessage(HttpMethod.Get, resultUrl);

                    request.Headers.Add("Authorization", $"Bearer {jwtToken}");

                    using HttpResponseMessage response = await client.SendAsync(request);
                    //response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                        await response.Content.CopyToAsync(fileStream);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                    return new ArchuveResult
                    {
                        Name = fileName,
                        HasImages = hasImages
                    };
                }
            }

            if (isFileExist)
            {
                using var archiveStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                using var archive = new ZipArchive(archiveStream, ZipArchiveMode.Read);

                foreach (var entry in archive.Entries)
                {
                    if (IsImageFile(entry.FullName))
                    {
                        hasImages = true;
                    }
                }
            }
            else
            {
                fileName = "false";
            }

            return new ArchuveResult
            {
                Name = fileName,
                HasImages = hasImages
            };
        }


        static bool IsImageFile(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif" || ext == ".bmp";
        }

        async Task<string?> GetBidState(Guid bidStateId)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, $"{bidStatesUrl}{bidStateId}");

                request.Headers.Add("Authorization", $"Bearer {jwtToken}");

                using HttpResponseMessage r = await client.SendAsync(request);
                r.EnsureSuccessStatusCode();

                var content = await r.Content.ReadFromJsonAsync<JsonElement>();
                var result = content.GetProperty("nameRu").GetString();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        async Task<string?> GetCropNameRu(Guid cropId)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, $"{cropsUrl}{cropId}");

                request.Headers.Add("Authorization", $"Bearer {jwtToken}");

                using HttpResponseMessage r = await client.SendAsync(request);
                r.EnsureSuccessStatusCode();

                var bidState = await r.Content.ReadFromJsonAsync<JsonElement>();
                var bidStateNameRu = bidState.GetProperty("nameRu").GetString();

                return bidStateNameRu;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }


    }

    internal class ArchuveResult
    {
        public string? Name { get; set; }
        public bool? HasImages { get; set; }
    }

    /// <summary>
    /// Параметры загрузки результатов по заявке
    /// </summary>
    public class BidResultUploadOptions
    {
        /// <summary> Раздел файла конфигурации, откуда считываются настройки </summary>
        public const string Section = "BidResultUploadOptions";

        /// <summary>
        /// Путь к каталогу с загруженными файлами (Linux)
        /// </summary>
        public static string TempUploadDirPathLinux { get; set; } = "/var/tmp/";

        /// <summary>
        /// Путь к каталогу с загруженными файлами (Windows)
        /// </summary>
        public static string TempUploadDirPathWindows { get; set; } = "c:/temp";

        /// <summary>
        /// Имя каталога с загруженными файлами
        /// </summary>
        public static string TempUploadDirName { get; set; } = "masofa-mobile-api-uploads";

        /// <summary>
        /// Полный путь к каталогу с загруженными файлами для текущей системы
        /// </summary>
        public static string TempUploadDir => OperatingSystem.IsLinux()
                ? Path.Combine(TempUploadDirPathLinux, TempUploadDirName)
                : OperatingSystem.IsWindows()
                    ? Path.Combine(TempUploadDirPathWindows, TempUploadDirName)
                    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TempUploadDirName);

    }
}
