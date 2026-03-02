using Masofa.Client.Copernicus.Repositories;
using Masofa.Common.Models.Satellite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Masofa.Client.Copernicus
{
    public class CopernicusApiUnitOfWork
    {
        public ProductRepository ProductRepository { get; }
        public AccountRepository AccountRepository { get; }
        private HttpClient HttpClient { get; set; }
        private string? TokenJWT { get; set; }

        public bool IsAuthed 
        {
            get
            {
                return !string.IsNullOrEmpty(TokenJWT);
            }
        }

        public CopernicusApiUnitOfWork(ILogger<CopernicusApiUnitOfWork> logger, IConfiguration configuration)
        {
            HttpClient = new HttpClient();
            HttpClient.Timeout = TimeSpan.FromSeconds(60);
            ProductRepository = new ProductRepository(logger, HttpClient, configuration);
            AccountRepository = new AccountRepository(logger, HttpClient);
        }

        public async Task LoginAsync(SentinelServiceOptions options)
        {
            var token = await AccountRepository.GetAccessTokenAsync(options);
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            TokenJWT = token;
            ProductRepository.TokenJWT = token;
            AccountRepository.TokenJWT = token;
        }
    }

    public class SentinelServiceOptions
    {
        public string ApiUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string TokenApiUrl { get; set; }
        public string ProductSearchApiUrl { get; set; }
        public string ProductDownloadApiUrl { get; set; }
        public SatelliteSearchConfig SatelliteSearchConfig { get; set; }
    }
}
