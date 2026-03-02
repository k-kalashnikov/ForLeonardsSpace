using Masofa.Common.Models.Satellite;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace Masofa.Client.EarthExplorer.Repositories
{
    public class LandsatProductRepository : LandsatBaseRepository
    {
        
        public LandsatProductRepository(
            IHttpClientFactory httpClientFactory, 
            ILogger logger) 
            : base(httpClientFactory, logger)
        {
        }

        public async Task<List<string>> SearchProductsAsync(LandsatServiceOptions options, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrEmpty(TokenJWT))
            {
                throw new InvalidOperationException("API key (TokenJWT) is not set. You must login first.");
            }

            var url = options.SearchApiUrl;
            
            var requestBody = new
            {
                datasetName = "landsat_ot_c2_l2",
                sceneFilter = new
                {
                    spatialFilter = new
                    {
                        filterType = "mbr",
                        lowerLeft = new { latitude = options.SatelliteSearchConfig?.LandsatLeftDown?.Y ?? 37, longitude = options.SatelliteSearchConfig?.LandsatLeftDown?.X ?? 56 },
                        upperRight = new { latitude = options.SatelliteSearchConfig?.LandsatRightUp?.Y ?? 45, longitude = options.SatelliteSearchConfig?.LandsatRightUp?.X ?? 73 }
                    },
                    acquisitionFilter = new
                    {
                        start = startDate.ToString("yyyy-MM-dd"),
                        end = endDate.ToString("yyyy-MM-dd")
                    }
                },
                maxResults = 100,
                sortOrder = "DESC"
            };

            var json = JsonConvert.SerializeObject(requestBody);

            Logger.LogInformation(url);
            Logger.LogInformation(json);

            var message = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            message.Headers.Add("X-Auth-Token", TokenJWT);

            var responseJson = await GetJsonAsync(message);
            var data = responseJson["data"]?["results"] as JArray;

            var found = new List<string>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    var id = item["entityId"]?.ToString();
                    if (!string.IsNullOrEmpty(id))
                        found.Add(id);
                }
            }

            return found;
        }

        public async Task<LandsatProductMetadata> GetProductMetadataAsync(LandsatServiceOptions options, string datasetName, string productId, string productRefId)
        {
            var url = options.MetadataApiUrl;
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    datasetName,
                    entityIds = new[] { productId }
                }), Encoding.UTF8, "application/json")
            };  

            var json = await GetJsonAsync(AddAuthToken(request));
            var data = json["data"]?.FirstOrDefault() as JObject;

            if (data == null)
                throw new Exception("No metadata returned for product: " + productId);

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var dto = JsonConvert.DeserializeObject<LandsatProductMetadataDto>(data.ToString(), settings);

            return new LandsatProductMetadata
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                ProductRefId = productRefId,
                SpacecraftId = dto?.SpacecraftId,
                SensorId = dto?.SensorId,
                ProcessingLevel = dto?.ProcessingLevel,
                AcquisitionDate = DateTime.TryParse(dto?.AcquisitionDate, out var acqDate) ? acqDate : null,
                SceneCenterTime = DateTime.TryParse(dto?.SceneCenterTime, out var scTime) ? scTime : null,
                Path = dto?.Path,
                Row = dto?.Row,
                CloudCover = double.TryParse(dto?.CloudCover, out var cloud) ? cloud : null,
                DataType = dto?.DataType,
                CollectionCategory = dto?.CollectionCategory,
                WrsPathRow = $"{dto?.Path}-{dto?.Row}"
            };
        }

        public async Task<List<(string entityId, string productId)>> GetDownloadOptionsAsync(string datasetName, List<string> entityIds)
        {
            if (string.IsNullOrEmpty(TokenJWT))
                throw new InvalidOperationException("API key (TokenJWT) is not set.");

            var url = "https://m2m.cr.usgs.gov/api/api/json/stable/download-options";

            var requestBody = new
            {
                datasetName,
                entityIds,
                listId = Guid.NewGuid().ToString()
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json")
            };

            request.Headers.Add("X-Auth-Token", TokenJWT);

            var json = await GetJsonAsync(request);

            var result = new List<(string entityId, string productId)>();
            var data = json["data"] as JArray;
            if (data != null)
            {
                foreach (var item in data)
                {
                    var entityId = item["entityId"]?.ToString();
                    var productId = item["id"]?.ToString();
                    var available = item["available"]?.ToObject<bool>() ?? false;

                    if (!string.IsNullOrEmpty(entityId) && !string.IsNullOrEmpty(productId) && available)
                        result.Add((entityId, productId));
                }
            }

            return result;
        }

        public async Task<string?> CreateDownloadRequestAsync(List<(string entityId, string productId)> downloads)
        {
            if (string.IsNullOrEmpty(TokenJWT))
                throw new InvalidOperationException("API key (TokenJWT) is not set.");

            var url = "https://m2m.cr.usgs.gov/api/api/json/stable/download-request";

            var body = new
            {
                downloads = downloads.Select(d => new { d.entityId, d.productId }).ToArray(),
                label = "LandsatBatch",
                returnAvailable = true
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")
            };

            request.Headers.Add("X-Auth-Token", TokenJWT);

            var json = await GetJsonAsync(request);
            return json["data"]?["downloadRequestId"]?.ToString();
        }

        public async Task<List<string>> RetrieveDownloadLinksAsync(string downloadRequestId)
        {
            if (string.IsNullOrEmpty(TokenJWT))
                throw new InvalidOperationException("API key (TokenJWT) is not set.");

            var url = "https://m2m.cr.usgs.gov/api/api/json/stable/download-retrieve";

            var body = new
            {
                downloadRequestId
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")
            };

            request.Headers.Add("X-Auth-Token", TokenJWT);

            var json = await GetJsonAsync(request);

            var links = new List<string>();
            var data = json["data"]?["availableDownloads"] as JArray;
            if (data != null)
            {
                foreach (var item in data)
                {
                    var link = item["url"]?.ToString();
                    if (!string.IsNullOrEmpty(link))
                        links.Add(link);
                }
            }

            return links;
        }

        public async Task<Stream?> DownloadProductByIdAsync(string productId)
        {
            if (string.IsNullOrEmpty(TokenJWT))
                throw new InvalidOperationException("API key (TokenJWT) is not set.");

            var datasetName = "landsat_ot_c2_l2";

            // 1. Получаем download options
            var options = await GetDownloadOptionsAsync(datasetName, new List<string> { productId });
            if (!options.Any())
            {
                Logger.LogWarning("No download options available for product {ProductId}", productId);
                return null;
            }

            // 2. Создаём запрос на загрузку
            var downloadRequestId = await CreateDownloadRequestAsync(options);
            if (string.IsNullOrEmpty(downloadRequestId))
            {
                Logger.LogWarning("Failed to create download request for product {ProductId}", productId);
                return null;
            }

            // 3. Получаем download-ссылку
            var links = await RetrieveDownloadLinksAsync(downloadRequestId);
            var url = links.FirstOrDefault();
            if (string.IsNullOrEmpty(url))
            {
                Logger.LogWarning("No download link returned for product {ProductId}", productId);
                return null;
            }

            // 4. Загружаем файл как Stream
            Logger.LogInformation("Downloading product {ProductId} from {Url}", productId, url);
            return await HttpClient.GetStreamAsync(url);
        }
    }
}
