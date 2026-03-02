using System.Net.Http.Headers;
using System.Net;
using Microsoft.Extensions.Logging;

namespace Masofa.Client.Copernicus.Repositories
{
    public abstract class BaseRepository
    {
        protected HttpClient HttpClient { get; }
        protected ILogger Logger { get; }
        public string TokenJWT { get; set; }

        public BaseRepository(ILogger logger, HttpClient httpClient)
        {
            Logger = logger;
            HttpClient = httpClient;
        }

        public async Task<Stream> GetStreamFromRequestAsync(HttpRequestMessage message)
        {
            var response = await HttpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);
            
            // Проверяем статус ответа
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"401 Unauthorized: {errorContent}");
            }
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"HTTP request failed with status {response.StatusCode}: {errorContent}");
            }
            
            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<HttpResponseMessage> GetStreamFromRequestAsMessageAsync(HttpRequestMessage message)
        {
            var response = await HttpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);

            // Проверяем статус ответа
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"401 Unauthorized: {errorContent}");
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"HTTP request failed with status {response.StatusCode}: {errorContent}");
            }

            return response;
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
                // Можно вернуть значение по умолчанию или выбросить исключение
                throw; // Переброс исключения OperationCanceledException
            }
            catch (Exception ex)
            {
                // Можно бросить исключение или вернуть значение по умолчанию
                throw new InvalidOperationException("Failed to send request.", ex);
            }
        }

        public virtual HttpRequestMessage AddAuthenticationJWT(HttpRequestMessage message)
        {
            if (string.IsNullOrEmpty(TokenJWT))
            {
                throw new HttpRequestException("Unauthorized user");
            }
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenJWT);
            return message;
        }
    }
}
