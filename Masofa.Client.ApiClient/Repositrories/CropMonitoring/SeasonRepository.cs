using Masofa.Client.ApiClient.Repositrories.Common;
using Masofa.Common.Models.CropMonitoring;
using System.Net.Http.Json;

namespace Masofa.Client.ApiClient.Repositrories.CropMonitoring
{
    public class SeasonRepository : BaseCrudRepository<Season>
    {
        public SeasonRepository(HttpClient httpClient, string baseUrl) : base(httpClient, baseUrl)
        {
        }

        // Специфичные методы для Season

        public async Task<bool> ImportFromCSVAsync(MultipartFormDataContent content, CancellationToken cancellationToken)
        {
            HttpRequestMessage httpRequestMessage = new();
            httpRequestMessage.Method = HttpMethod.Post;
            httpRequestMessage.Content = content;
            httpRequestMessage.RequestUri = new Uri($"{BaseUrl}/ImportFromCSV");
            return await SendRequestAsync<bool>(AddAuthenticationJWT(httpRequestMessage), nameof(ImportFromCSVAsync), cancellationToken);
        }
    }
}