using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Masofa.Client.EarthExplorer.Repositories
{
    public class LandsatAccountRepository : LandsatBaseRepository
    {
        IHttpClientFactory _httpClientFactory;
        public LandsatAccountRepository(IHttpClientFactory httpClientFactory, ILogger logger, IConfiguration configuration) : base(httpClientFactory, logger)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string?> GetAccessTokenAsync(LandsatServiceOptions options)
        {
            try
            {
                var requestUrl = $"{options.TokenApiUrl}";

                var json = JsonConvert.SerializeObject(new
                {
                    username = options.UserName,
                    token = options.Token
                });

                var content = new StringContent(json, Encoding.UTF8);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl)
                {
                    Content = content
                };

                Logger.LogInformation($"Trying to authenticate Landsat API with username: {options.UserName}");

                var result = await SendRequestAsync<LandsatLoginResponseViewModel>(httpRequestMessage);

                if (result?.data!= null)
                {
                    Logger.LogInformation("Authentication successful.");
                    return result.data;
                }

                throw new Exception($"API login failed: {result?.errorMessage ?? "Unknown error"}");
            }

            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception while getting Landsat API key");
                return null;
            }
        }
    }


}
