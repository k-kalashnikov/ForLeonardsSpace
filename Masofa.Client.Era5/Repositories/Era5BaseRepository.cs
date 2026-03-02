using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Masofa.Client.Era5.Repositories
{
    public abstract class Era5BaseRepository
    {
        protected HttpClient HttpClient { get; }
        protected ILogger Logger { get; }

        public Era5BaseRepository(HttpClient httpClient, ILogger logger)
        {
            HttpClient = httpClient;
            Logger = logger;
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
