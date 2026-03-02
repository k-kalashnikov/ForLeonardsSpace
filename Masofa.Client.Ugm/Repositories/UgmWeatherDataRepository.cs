using Masofa.Client.Ugm.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Masofa.Client.Ugm.Repositories
{
    public class UgmWeatherDataRepository : UgmBaseRepository
    {
        private UgmOptions UgmOptions { get; }

        public UgmWeatherDataRepository(HttpClient httpClient, ILogger logger, IOptions<UgmOptions> options) : base(httpClient, logger)
        {
            UgmOptions = options.Value;
        }

        public async Task<List<UgmForecastContainer>> GetAllForecastWeatherDataAsync()
        {
            HttpRequestMessage httpRequestMessage = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(UgmOptions.UrlForecast)
            };

            return await SendRequestAsync<List<UgmForecastContainer>>(httpRequestMessage);
        }

        public async Task<List<UgmCurrentWeather>> GetAllCurrentWeatherDataAsync()
        {
            HttpRequestMessage httpRequestMessage = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(UgmOptions.UrlCurrent)
            };

            return await SendRequestAsync<List<UgmCurrentWeather>>(httpRequestMessage);
        }

        public async Task<List<UgmForecastItem>> GetForecastWeatherDataByUgmRegionIdAsync(int regionId)
        {
            HttpRequestMessage httpRequestMessage = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{UgmOptions.UrlForecast}/{regionId}")
            };

            return await SendRequestAsync<List<UgmForecastItem>>(httpRequestMessage);
        }

        public async Task<UgmCurrentWeather> GetCurrentWeatherDataByUgmRegionIdAsync(int regionId)
        {
            HttpRequestMessage httpRequestMessage = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{UgmOptions.UrlCurrent}/{regionId}")
            };

            return await SendRequestAsync<UgmCurrentWeather>(httpRequestMessage);
        }
    }
}
