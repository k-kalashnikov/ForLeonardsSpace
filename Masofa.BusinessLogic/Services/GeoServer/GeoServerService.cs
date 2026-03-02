using Masofa.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Masofa.BusinessLogic.Services
{
    public class GeoServerService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = string.Empty;
        private readonly string _username = string.Empty;
        private readonly string _password = string.Empty;
        private readonly string _workspace = string.Empty;
        private readonly string _volume = string.Empty;
        private GeoServerOptions GeoServerOptions { get; set; }

        public GeoServerService(IOptions<GeoServerOptions> configuration)
        {
            _baseUrl = (configuration.Value.GeoServerUrl ?? string.Empty) + "/geoserver";
            _username = configuration.Value.UserName ?? string.Empty;
            _password = configuration.Value.Password ?? string.Empty;
            _workspace = configuration.Value.Workspace ?? string.Empty;
            _volume = configuration.Value.Volume ?? string.Empty;

            _httpClient = new HttpClient()
            {
                Timeout = TimeSpan.FromMinutes(5)
            };
            var credentials = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{_username}:{_password}")
            );
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
        }

        private async Task<bool> WorkspaceExistsAsync(string workspaceName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/rest/workspaces/{workspaceName}.json");
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateWorkspaceAsync(string workspaceName)
        {
            if (await WorkspaceExistsAsync(workspaceName))
            {
                return true;
            }

            var payload = new
            {
                workspace = new
                {
                    name = workspaceName
                }
            };
            return await PostAsync($"/rest/workspaces", payload, expectedStatusCodes: [HttpStatusCode.Created, HttpStatusCode.Conflict]);
        }

        private async Task<bool> CoverageStoreExistsAsync(string workspace, string storeName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/rest/workspaces/{workspace}/coveragestores/{storeName}.json");
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateImageMosaicStoreAsync(string storeName, string dataPath, string? workspace = null)
        {
            workspace ??= _workspace;

            if (await CoverageStoreExistsAsync(workspace, storeName))
            {
                return true;
            }

            var url = _volume;
            if (!url.EndsWith("/"))
            {
                url += "/";
            }
            url += dataPath.Replace("\\", "/");

            var payload = new
            {
                coverageStore = new
                {
                    name = storeName,
                    type = "ImageMosaic",
                    enabled = true,
                    workspace = workspace,
                    url = $"file://{url}"
                }
            };
            var result = await PostAsync($"/rest/workspaces/{workspace}/coveragestores", payload, expectedStatusCodes: new[] { HttpStatusCode.Created, HttpStatusCode.Conflict });
            return result;
        }

        public async Task<bool> RecreateImageMosaicStoreAsync(string storeName, string dataPath, string? workspace = null)
        {
            workspace ??= _workspace;

            if (await CoverageStoreExistsAsync(workspace, storeName))
            {
                await DeleteCoverageStoreAsync(workspace, storeName);
            }

            var url = _volume;
            if (!url.EndsWith("/"))
            {
                url += "/";
            }
            url += dataPath.Replace("\\", "/");

            var payload = new
            {
                coverageStore = new
                {
                    name = storeName,
                    type = "ImageMosaic",
                    enabled = true,
                    workspace = workspace,
                    url = $"file://{url}"
                }
            };
            var result = await PostAsync($"/rest/workspaces/{workspace}/coveragestores", payload, expectedStatusCodes: new[] { HttpStatusCode.Created, HttpStatusCode.Conflict });
            return result;
        }

        public async Task<bool> DeleteCoverageStoreAsync(string workspace, string storeName)
        {
            var url = $"{_baseUrl}/rest/workspaces/{workspace}/coveragestores/{storeName}?recurse=true";

            try
            {
                var response = await _httpClient.DeleteAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.OK ||
                    response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка соединения: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> CoverageExistsAsync(string workspace, string layerName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/rest/workspaces/{workspace}/coverages/{layerName}.json");
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> PublishCoverageAsync(string storeName, string layerName, string? workspace = null)
        {
            workspace ??= _workspace;

            if (await CoverageExistsAsync(workspace, layerName))
            {
                return true;
            }

            var payload = new
            {
                coverage = new
                {
                    name = layerName,
                    title = layerName,
                    srs = "EPSG:4326"
                }
            };

            return await PostAsync($"/rest/workspaces/{workspace}/coveragestores/{storeName}/coverages", payload, expectedStatusCodes: new[] { HttpStatusCode.Created, HttpStatusCode.Conflict });
        }

        public async Task<bool> EnableTimeDimensionAsync(string storeName, string layerName, string? workspace = null)
        {
            workspace ??= _workspace;

            var response = await _httpClient.GetAsync($"{_baseUrl}/rest/workspaces/{workspace}/coveragestores/{storeName}/coverages/{layerName}.json");
            if (!response.IsSuccessStatusCode) return false;

            var json = await response.Content.ReadAsStringAsync();
            var jObject = JsonConvert.DeserializeObject<dynamic>(json);

            var dimensions = jObject?.coverage?.domain?.dimensions?.dimension;
            if (dimensions != null)
            {
                foreach (var dim in dimensions)
                {
                    if (dim.name?.ToString() == "time")
                    {
                        Console.WriteLine("Размерность 'time' уже включена.");
                        return true;
                    }
                }
            }

            var payload = new
            {
                coverage = new
                {
                    name = layerName,
                    domain = new
                    {
                        dimensions = new
                        {
                            dimension = new[]
                            {
                                new
                                {
                                    name = "time",
                                    presentation = "CONTINUOUS_INTERVAL",
                                    attribute = "ingestion",
                                    period = "P1D",
                                    units = "ISO8601",
                                    defaultValue = new
                                    {
                                        strategy = "NEAREST",
                                        referenceValue = ""
                                    }
                                }
                            }
                        }
                    }
                }
            };

            return await PutAsync($"/rest/workspaces/{workspace}/coveragestores/{storeName}/coverages/{layerName}", payload);
        }

        public async Task<bool> ReloadConfigurationAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}/rest/reload", null);
                if (response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> PostAsync(string endpoint, object payload, HttpStatusCode[]? expectedStatusCodes = null)
        {
            expectedStatusCodes ??= [HttpStatusCode.Created];

            var json = payload != null ? JsonConvert.SerializeObject(payload) : null;
            var content = json != null ? new StringContent(json, Encoding.UTF8, "application/json") : null;

            try
            {
                var response = await _httpClient.PostAsync(_baseUrl + endpoint, content);

                if (Array.Exists(expectedStatusCodes, code => code == response.StatusCode))
                {
                    return true;
                }

                var error = await response.Content.ReadAsStringAsync();
                return false;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> PutAsync(string endpoint, object payload)
        {
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PutAsync(_baseUrl + endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                var error = await response.Content.ReadAsStringAsync();
                return false;
            }
            catch
            {
                return false;
            }
        }

        public string GetFullDataPath(string relativePath)
        {
            return Path.Combine(_volume, _workspace, relativePath);
        }
    }
}
