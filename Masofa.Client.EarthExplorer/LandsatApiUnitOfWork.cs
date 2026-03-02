using Masofa.Client.EarthExplorer.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Masofa.Client.EarthExplorer
{
    public class LandsatApiUnitOfWork
    {
        public LandsatProductRepository ProductRepository { get; }
        public LandsatAccountRepository AccountRepository { get; }
        private string? TokenJWT { get; set; }
        public bool IsAuthed
        {
            get
            {
                return !string.IsNullOrEmpty(TokenJWT);
            }
        }

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<LandsatApiUnitOfWork> _logger;

        public LandsatApiUnitOfWork(
            IHttpClientFactory httpClientFactory, 
            ILogger<LandsatApiUnitOfWork> logger, 
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            ProductRepository = new LandsatProductRepository(httpClientFactory, logger);
            AccountRepository = new LandsatAccountRepository(httpClientFactory, logger, configuration);
        }

        public async Task LoginAsync(LandsatServiceOptions options)
        {
            if (IsAuthed)
            {
                return;
            }
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
}
