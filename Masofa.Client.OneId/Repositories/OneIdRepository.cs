using Masofa.Client.OneId.Models;
using Masofa.Common.Models.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;

namespace Masofa.Client.OneId.Repositories
{
    public class OneIdRepository
    {
        private HttpClient HttpClient { get; }
        private ILogger Logger { get; }
        private OneIdOptions Options { get; }

        public OneIdRepository(HttpClient httpClient, ILogger logger, IOptions<OneIdOptions> options)
        {
            HttpClient = httpClient;
            Logger = logger;
            Options = options.Value;
        }

        public async Task<OneIdRedirectUrlResponse> GetRedirectUrlAsync()
        {
            var query = $"{Options.BaseUrl}/get-redirect-url" +
                $"?client_id={Options.ClientId}" +
                $"&redirect_url={Options.RedirectUrl}";

            HttpRequestMessage httpRequestMessage = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(query)
            };

            return await SendRequestAsync<OneIdRedirectUrlResponse>(httpRequestMessage);
        }

        public async Task<OneIdTokenResponse> GetUserTokenAsync(string code)
        {
            var query = $"{Options.BaseUrl}/user-token" +
                $"?client_id={Options.ClientId}" +
                $"&client_secret={Options.ClientSecret}" +
                $"&code={Uri.EscapeDataString(code)}";

            HttpRequestMessage httpRequestMessage = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(query)
            };

            return await SendRequestAsync<OneIdTokenResponse>(httpRequestMessage);
        }

        public async Task<OneIdUser> GetUserDataAsync(string accessToken)
        {
            var query = $"{Options.BaseUrl}/user-data?access_token={Uri.EscapeDataString(accessToken)}";

            HttpRequestMessage httpRequestMessage = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(query)
            };

            return await SendRequestAsync<OneIdUser>(httpRequestMessage);
        }

        public async Task<OneIdTokenResponse> RefreshTokenAsync(string refreshToken)
        {
            var query = $"{Options.BaseUrl}/refresh?refresh_token={Uri.EscapeDataString(refreshToken)}";

            HttpRequestMessage httpRequestMessage = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(query)
            };

            return await SendRequestAsync<OneIdTokenResponse>(httpRequestMessage);
        }

        public async Task<TResult> SendRequestAsync<TResult>(HttpRequestMessage message)
        {
            Logger.LogInformation($"Try send request to {message.RequestUri}");
            try
            {
                var response = await HttpClient.SendAsync(message);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var responseString = await response.Content.ReadAsStringAsync();
                        object result = typeof(string) == typeof(TResult) ? responseString : Newtonsoft.Json.JsonConvert.DeserializeObject<TResult>(responseString);
                        return (TResult)result;
                    case HttpStatusCode.NotFound:
                        throw new HttpRequestException("Not Found");
                    case HttpStatusCode.Unauthorized:
                        throw new HttpRequestException("Unauthorized user");
                    case HttpStatusCode.BadRequest:
                        throw new HttpRequestException($"Model not valid: {response.Content}");
                    case HttpStatusCode.InternalServerError:
                        throw new HttpRequestException($"Internal Server Error: {response.Content}");
                    default:
                        throw new Exception($"Unhadled code: {response.StatusCode}. With message: {response.Content.ToString()}");
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to send request.", ex);
            }
        }
    }
}
