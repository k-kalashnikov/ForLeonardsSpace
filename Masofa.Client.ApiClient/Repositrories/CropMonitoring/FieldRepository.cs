using System.Net;
using System.Net.Http.Json;
using Masofa.BusinessLogic.CropMonitoring.Fields;
using Masofa.Client.ApiClient.Repositrories.Common;
using Masofa.Common.Models.CropMonitoring;

namespace Masofa.Client.ApiClient.Repositrories.CropMonitoring
{
    public class FieldRepository : BaseCrudRepository<Field>
    {
        public FieldRepository(HttpClient httpClient, string baseUrl) : base(httpClient, baseUrl)
        {
        }

        // Специфичные методы для Field

        public async Task<FieldImportResult> ImportFieldsAsync(FieldExportType exportType, MultipartFormDataContent content, CancellationToken cancellationToken)
        {
            HttpRequestMessage httpRequestMessage = new();
            httpRequestMessage.Method = HttpMethod.Post;
            httpRequestMessage.Content = content;
            httpRequestMessage.RequestUri = new Uri($"{BaseUrl}/ImportFields?exportType={(int)exportType}");
            return await SendRequestAsync<FieldImportResult>(AddAuthenticationJWT(httpRequestMessage), nameof(ImportFieldsAsync), cancellationToken);
        }

        public async Task<byte[]> ExportFieldsAsync(FieldExportRequest request, CancellationToken cancellationToken)
        {
            HttpRequestMessage httpRequestMessage = new();
            httpRequestMessage.Method = HttpMethod.Post;
            httpRequestMessage.Content = JsonContent.Create(request);
            httpRequestMessage.RequestUri = new Uri($"{BaseUrl}/ExportFields");

            var response = await SendRequestAndGetResponseAsync(AddAuthenticationJWT(httpRequestMessage), nameof(ExportFieldsAsync), cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync(cancellationToken);
            }

            // Обработка ошибок, как в вашем BaseRepository
            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    throw new HttpRequestException("Page not found", null, HttpStatusCode.NotFound);
                case HttpStatusCode.Unauthorized:
                    throw new HttpRequestException("Unauthorized user", null, HttpStatusCode.Unauthorized);
                case HttpStatusCode.BadRequest:
                    var badRequestContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Model not valid: {badRequestContent}", null, HttpStatusCode.BadRequest);
                case HttpStatusCode.InternalServerError:
                    var serverErrorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Internal Server Error: {serverErrorContent}", null, HttpStatusCode.InternalServerError);
                default:
                    var defaultContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Unhandled code: {response.StatusCode}. With message: {defaultContent}", null, response.StatusCode);
            }
        }

        public async Task<Field?> GetByPointsAsync(double x, double y, int srid = 4326, CancellationToken cancellationToken = default)
        {
            HttpRequestMessage httpRequestMessage = new();
            httpRequestMessage.Method = HttpMethod.Get;
            httpRequestMessage.RequestUri = new Uri($"{BaseUrl}/GetByPoints?x={x}&y={y}&srid={srid}");
            return await SendRequestAsync<Field?>(AddAuthenticationJWT(httpRequestMessage), nameof(GetByPointsAsync), cancellationToken);
        }

        public async Task<bool> ImportFromCSVAsync(MultipartFormDataContent content, CancellationToken cancellationToken)
        {
            HttpRequestMessage httpRequestMessage = new();
            httpRequestMessage.Method = HttpMethod.Post;
            httpRequestMessage.Content = content;
            httpRequestMessage.RequestUri = new Uri($"{BaseUrl}/ImportFromCSV");
            return await SendRequestAsync<bool>(AddAuthenticationJWT(httpRequestMessage), nameof(ImportFromCSVAsync), cancellationToken);
        }

        // Вспомогательный метод для получения полного HttpResponseMessage
        private async Task<HttpResponseMessage> SendRequestAndGetResponseAsync(HttpRequestMessage request, string methodName, CancellationToken cancellationToken)
        {
            try
            {
                var response = await HttpClient.SendAsync(request, cancellationToken);
                return response;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to send request.", ex);
            }
        }
    }

    // Модель для результата импорта полей
    public class FieldImportResult
    {
        public int Created { get; set; }
        public List<Guid> Ids { get; set; } = new();
    }
}