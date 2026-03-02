using Masofa.Client.IBMWeather.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Masofa.Client.IBMWeather
{
    public class IBMWeatherApiUnitOfWork
    {
        public IBMWeatherRepository IBMWeatherRepository { get; }
        
        private string? ApiKey { get; set; }
        
        public bool IsConfigured
        {
            get
            {
                return !string.IsNullOrEmpty(ApiKey);
            }
        }


        public IBMWeatherApiUnitOfWork(
            IHttpClientFactory httpClientFactory, 
            ILogger<IBMWeatherApiUnitOfWork> logger, 
            IConfiguration configuration)
        {
            IBMWeatherRepository = new IBMWeatherRepository(httpClientFactory, logger);
        }

        public void Configure(IBMWeatherServiceOptions options)
        {
            if (string.IsNullOrEmpty(options.ApiKey))
            {
                throw new ArgumentException("API Key is required", nameof(options.ApiKey));
            }

            ApiKey = options.ApiKey;
            IBMWeatherRepository.Configure(options);
        }
    }

    public class IBMWeatherServiceOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://api.weather.com/v3";
        public string Language { get; set; } = "en-US";
        public string Format { get; set; } = "json";
        public string Units { get; set; } = "s"; // s-метрическая, e-английская, h-гибридная, m-метрическая с ветром в км/ч
    }
}
