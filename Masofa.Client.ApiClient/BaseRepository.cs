using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Masofa.Client.ApiClient
{
    public class BaseRepository
    {
        protected HttpClient HttpClient { get; set; }
        protected string BaseUrl { get; set; }
        public string Token { get; set; }
        public string RepositoryBaseUrl => BaseUrl; // <-- Добавлено

        public BaseRepository(HttpClient httpClient, string baseUrl)
        {
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            BaseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
        }

        public async Task<TResult> SendRequestAsync<TResult>(HttpRequestMessage message, string methodName, CancellationToken cancellationToken)
        {
            try
            {
                // Добавим проверку на null
                if (HttpClient == null)
                {
                    throw new InvalidOperationException("HttpClient is null in BaseRepository");
                }

                var response = await HttpClient.SendAsync(message, cancellationToken);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var responseString = await response.Content.ReadAsStringAsync();
                        object result = (typeof(string) == typeof(TResult)) ? responseString : Newtonsoft.Json.JsonConvert.DeserializeObject<TResult>(responseString);
                        return (TResult)result;
                    case HttpStatusCode.NotFound:
                        throw new HttpRequestException("Page not found", null, HttpStatusCode.NotFound);
                    case HttpStatusCode.Unauthorized:
                        throw new HttpRequestException("Unauthorized user", null, HttpStatusCode.Unauthorized);
                    case HttpStatusCode.BadRequest:
                        var badRequestContent = await response.Content.ReadAsStringAsync();
                        throw new HttpRequestException($"Model not valid: {badRequestContent}", null, HttpStatusCode.BadRequest);
                    case HttpStatusCode.InternalServerError:
                        var serverErrorContent = await response.Content.ReadAsStringAsync();
                        throw new HttpRequestException($"Internal Server Error: {serverErrorContent}", null, HttpStatusCode.InternalServerError);
                    default:
                        var defaultContent = await response.Content.ReadAsStringAsync();
                        throw new HttpRequestException($"Unhandled code: {response.StatusCode}. With message: {defaultContent}", null, response.StatusCode);
                }
            }
            //catch (JsonException jsonEx)
            //{
            //    Logger.LogError(
            //        LoggerEventBuilder.GetEventId(Configuration, GetType(), methodName),
            //        jsonEx,
            //        $"Failed to deserialize JSON response in {GetType().FullName}=>{nameof(SendRequest)}. Response content: {await response.Content.ReadAsStringAsync(cancellationToken)}"
            //    );

            //    // Можно бросить исключение или вернуть значение по умолчанию
            //    throw new InvalidDataException("Failed to deserialize JSON response.", jsonEx);
            //}
            catch (OperationCanceledException)
            {

                // Можно вернуть значение по умолчанию или выбросить исключение
                throw; // Переброс исключения OperationCanceledException
            }    
            
            catch (HttpRequestException)
            {
                // Не оборачиваем HttpRequestException в другой тип исключения
                throw; // Пробрасываем как есть
            }
            catch (Exception ex)
            {
                // Можно бросить исключение или вернуть значение по умолчанию
                //Console.WriteLine($"BaseRepository.SendRequestAsync failed: {ex}");
                //throw new InvalidOperationException($"Failed to send request in {methodName}.", ex);
                var errorMessage = $"BaseRepository.SendRequestAsync failed in {methodName}: {ex}";
                Console.WriteLine(errorMessage);
                throw new InvalidOperationException($"Failed to send request in {methodName}. See inner exception.", ex);
            }
        }

        public virtual HttpRequestMessage AddAuthenticationJWT(HttpRequestMessage message)
        {
            if (string.IsNullOrEmpty(Token))
            {
                throw new HttpRequestException("Unauthorized user");
            }
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            return message;
        }
    }
}
