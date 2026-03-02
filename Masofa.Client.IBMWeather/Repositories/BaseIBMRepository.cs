using Microsoft.Extensions.Logging;
using System.Net;
using System.Web;

namespace Masofa.Client.IBMWeather.Repositories
{
    public abstract class BaseIBMRepository
    {
        protected HttpClient HttpClient { get; }
        protected ILogger Logger { get; }
        protected IBMWeatherServiceOptions? Options { get; private set; }

        public BaseIBMRepository(IHttpClientFactory httpClientFactory, ILogger logger)
        {
            HttpClient = httpClientFactory.CreateClient();
            Logger = logger;
        }

        public void Configure(IBMWeatherServiceOptions options)
        {
            Options = options;
        }

        protected async Task<TResult> SendRequestAsync<TResult>(HttpRequestMessage message)
        {
            Logger.LogInformation($"Sending request to {message.RequestUri}");

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
                        throw new HttpRequestException("Weather data not found for the specified location");

                    case HttpStatusCode.Unauthorized:
                        throw new HttpRequestException("Invalid API key or unauthorized access");

                    case HttpStatusCode.BadRequest:
                        var errorContent = await response.Content.ReadAsStringAsync();
                        throw new HttpRequestException($"Bad request: {errorContent}");

                    case HttpStatusCode.Forbidden:
                        throw new HttpRequestException("Access forbidden - check your subscription level");

                    case HttpStatusCode.InternalServerError:
                        throw new HttpRequestException("IBM Weather API internal server error");

                    default:
                        var defaultErrorContent = await response.Content.ReadAsStringAsync();
                        throw new HttpRequestException($"Unexpected status code {response.StatusCode}: {defaultErrorContent}");
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (!(ex is HttpRequestException))
            {
                throw new InvalidOperationException("Failed to send request to IBM Weather API", ex);
            }
        }

        protected string BuildQueryString(Dictionary<string, string> parameters)
        {
            if (Options == null)
            {
                throw new InvalidOperationException("Repository not configured. Call Configure() first.");
            }

            // Добавляем обязательные параметры
            parameters["apiKey"] = Options.ApiKey;
            parameters["language"] = Options.Language;
            parameters["format"] = Options.Format;
            parameters["units"] = Options.Units;

            var queryParams = parameters
                .Where(kvp => !string.IsNullOrEmpty(kvp.Value))
                .Select(kvp => $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}");

            return string.Join("&", queryParams);
        }

        protected HttpRequestMessage CreateGetRequest(string endpoint, Dictionary<string, string> parameters)
        {
            var queryString = BuildQueryString(parameters);
            var uri = $"{Options!.BaseUrl}{endpoint}?{queryString}";

            return new HttpRequestMessage(HttpMethod.Get, uri);
        }
    }
}
