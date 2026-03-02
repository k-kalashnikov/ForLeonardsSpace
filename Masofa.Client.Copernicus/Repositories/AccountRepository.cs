using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text.Json.Nodes;

namespace Masofa.Client.Copernicus.Repositories
{
    public class AccountRepository : BaseRepository
    {
        public AccountRepository(ILogger logger, HttpClient httpClient) : base(logger, httpClient)
        {
        }

        public async Task<string?> GetAccessTokenAsync(SentinelServiceOptions options)
        {
            try
            {

                HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage.Method = HttpMethod.Post;
                httpRequestMessage.RequestUri = new Uri(options.TokenApiUrl);
                httpRequestMessage.Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new("client_id", "cdse-public"),
                    new("username", options.UserName),
                    new("password", options.Password)

                });

                Logger.LogInformation($"Try to GetAccessTokenAsync with UserName: {options.UserName}, Password: {options.Password}");

                var result = await SendRequestAsync<AccessTokenViewModel>(httpRequestMessage);

                return result.access_token;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception while getting access token");
                return null;
            }
        }
    }

    public class AccessTokenViewModel
    {
        public string access_token { get; set; }
    }
}
