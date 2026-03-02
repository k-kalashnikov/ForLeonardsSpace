using Masofa.BusinessLogic.CropMonitoring.Fields;
using Masofa.Common.Models.CropMonitoring;

namespace Masofa.Client.ApiClient.Repositrories.CropMonitoring
{
    public class BidTemplateRepository : BaseCrudRepository<BidTemplate>
    {
        public BidTemplateRepository(HttpClient httpClient, string baseUrl) : base(httpClient, baseUrl)
        {
        }

        // Специфичные методы для BidTemplate

        public async Task<List<string>> ImportFromFilesAsync(FieldExportType exportType, MultipartFormDataContent content, CancellationToken cancellationToken)
        {
            HttpRequestMessage httpRequestMessage = new();
            httpRequestMessage.Method = HttpMethod.Post;
            httpRequestMessage.Content = content;
            // Важно: exportType передается как query параметр
            httpRequestMessage.RequestUri = new Uri($"{BaseUrl}/ImportFromFiles?exportType={(int)exportType}");
            return await SendRequestAsync<List<string>>(AddAuthenticationJWT(httpRequestMessage), nameof(ImportFromFilesAsync), cancellationToken);
        }

        public async Task<string> GetSchemaAsync(CancellationToken cancellationToken)
        {
            HttpRequestMessage httpRequestMessage = new();
            httpRequestMessage.Method = HttpMethod.Get;
            httpRequestMessage.RequestUri = new Uri($"{BaseUrl}/GetSchema");
            return await SendRequestAsync<string>(AddAuthenticationJWT(httpRequestMessage), nameof(GetSchemaAsync), cancellationToken);
        }

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