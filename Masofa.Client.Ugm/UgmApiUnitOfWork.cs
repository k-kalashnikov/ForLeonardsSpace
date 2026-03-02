using Masofa.Client.Ugm.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Masofa.Client.Ugm
{
    public class UgmApiUnitOfWork
    {
        public UgmWeatherDataRepository UgmWeatherDataRepository { get; }
        private HttpClient HttpClient { get; set; }

        public UgmApiUnitOfWork(ILogger<UgmApiUnitOfWork> logger, IOptions<UgmOptions> options)
        {
            HttpClient = new HttpClient();
            HttpClient.Timeout = TimeSpan.FromMinutes(5);
            UgmWeatherDataRepository = new UgmWeatherDataRepository(HttpClient, logger, options);
        }
    }

    public class UgmOptions
    {
        public string UrlCurrent { get; set; }
        public string UrlForecast { get; set; }
    }
}