using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;

namespace Masofa.Client.EarthExplorer.Repositories
{
    public abstract class LandsatBaseRepository
    {
        protected HttpClient HttpClient { get; }
        protected ILogger Logger { get; }
        public string TokenJWT { get; set; }

        public LandsatBaseRepository(IHttpClientFactory httpClientFactory, ILogger logger)
        {
            HttpClient = httpClientFactory.CreateClient();
            Logger = logger;
        }

        public void SetToken(string token)
        {
            TokenJWT = token;
        }

        protected async Task<Stream> GetStreamAsync(HttpRequestMessage request)
        {
            var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }

        protected async Task<string> GetStringAsync(HttpRequestMessage request)
        {
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<HttpStatusCode> SendRequestAsync(HttpRequestMessage message)
        {
            var response = await HttpClient.SendAsync(message);
            return response.StatusCode;
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
                        Console.WriteLine(responseString);
                        object result = typeof(string) == typeof(TResult) ? responseString : Newtonsoft.Json.JsonConvert.DeserializeObject<TResult>(responseString);
                        Console.WriteLine($"TResult == {typeof(TResult)}");
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
                // Можно вернуть значение по умолчанию или выбросить исключение
                throw; // Переброс исключения OperationCanceledException
            }
            catch (Exception ex)
            {
                // Можно бросить исключение или вернуть значение по умолчанию
                throw new InvalidOperationException("Failed to send request.", ex);
            }
        }

        public virtual HttpRequestMessage AddAuthToken(HttpRequestMessage message)
        {
            if (string.IsNullOrEmpty(TokenJWT))
            {
                throw new HttpRequestException("Unauthorized user");
            }
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenJWT);
            return message;
        }

        protected async Task<JObject> GetJsonAsync(HttpRequestMessage request)
        {
            var content = await GetStringAsync(request);
            var json = JObject.Parse(content);

            var errorCode = json["errorCode"]?.ToString();
            if (!string.IsNullOrWhiteSpace(errorCode))
            {
                var errorMessage = json["errorMessage"]?.ToString() ?? "No message";
                throw new Exception($"USGS API error: {errorCode} - {errorMessage}");
            }

            return json;
        }
    }
}
