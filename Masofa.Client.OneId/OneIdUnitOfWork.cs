using Masofa.Client.OneId.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Masofa.Client.OneId
{
    public class OneIdUnitOfWork
    {
        public OneIdRepository OneIdRepository { get; }
        private HttpClient HttpClient { get; set; }

        public OneIdUnitOfWork(ILogger<OneIdUnitOfWork> logger, IOptions<OneIdOptions> options)
        {
            HttpClient = new HttpClient();
            HttpClient.Timeout = TimeSpan.FromMinutes(5);
            OneIdRepository = new OneIdRepository(HttpClient, logger, options);
        }
    }

    public class OneIdOptions
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string RedirectUrl { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
    }
}