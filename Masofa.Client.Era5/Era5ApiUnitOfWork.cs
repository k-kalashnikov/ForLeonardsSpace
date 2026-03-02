using Masofa.Client.Era5.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Masofa.Client.Era5
{
    public class Era5ApiUnitOfWork
    {
        public Era5WeatherDataRepository Era5WeatherDataRepository { get; }
        private HttpClient HttpClient { get; set; }

        public Era5ApiUnitOfWork(ILogger<Era5ApiUnitOfWork> logger, IOptions<Era5Options> options)
        {
            HttpClient = new HttpClient();
            HttpClient.Timeout = TimeSpan.FromMinutes(10);
            Era5WeatherDataRepository = new Era5WeatherDataRepository(HttpClient, logger, options);
        }
    }

    public class Era5Options
    {
        public string ArchiveUrl { get; set; }
        public string ForecastUrl { get; set; }
        public string RequestParams { get; set; }
        public int NumRetries { get; set; }
        public int RetryDelayMs { get; set; }
        public int RateLimit { get; set; }
    }
}