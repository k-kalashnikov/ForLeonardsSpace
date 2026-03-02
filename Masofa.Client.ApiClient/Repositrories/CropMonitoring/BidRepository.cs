using System.Net.Http.Json;
using Masofa.Client.ApiClient.Repositrories.Common;
using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;

namespace Masofa.Client.ApiClient.Repositrories.CropMonitoring
{
    public class BidRepository : BaseCrudRepository<Bid>
    {
        public BidRepository(HttpClient httpClient, string baseUrl) : base(httpClient, baseUrl)
        {
        }

        // Специфичные методы для Bid

        public async Task<bool> SaveResultAsync(Guid bidId, MultipartFormDataContent content, CancellationToken cancellationToken)
        {
            HttpRequestMessage httpRequestMessage = new();
            httpRequestMessage.Method = HttpMethod.Post;
            httpRequestMessage.Content = content;
            httpRequestMessage.RequestUri = new Uri($"{BaseUrl}/SaveResult/{bidId}");
            return await SendRequestAsync<bool>(AddAuthenticationJWT(httpRequestMessage), nameof(SaveResultAsync), cancellationToken);
        }

        //public async Task<List<BidGetViewModel>> CustomGetByQueryAsync(BaseGetQuery<BidGetViewModel> query, CancellationToken cancellationToken)
        //{
        //    HttpRequestMessage httpRequestMessage = new();
        //    httpRequestMessage.Method = HttpMethod.Post;
        //    httpRequestMessage.Content = JsonContent.Create(query);
        //    httpRequestMessage.RequestUri = new Uri($"{BaseUrl}/CustomGetByQuery");
        //    return await SendRequestAsync<List<BidGetViewModel>>(AddAuthenticationJWT(httpRequestMessage), nameof(CustomGetByQueryAsync), cancellationToken);
        //}

        //public async Task<BidGetViewModel?> CustomGetByIdAsync(Guid id, CancellationToken cancellationToken)
        //{
        //    HttpRequestMessage httpRequestMessage = new();
        //    httpRequestMessage.Method = HttpMethod.Get;
        //    httpRequestMessage.RequestUri = new Uri($"{BaseUrl}/CustomGetById/{id}");
        //    return await SendRequestAsync<BidGetViewModel?>(AddAuthenticationJWT(httpRequestMessage), nameof(CustomGetByIdAsync), cancellationToken);
        //}

        //public async Task<Guid> CustomCreateAsync(BidCreateViewModel model, CancellationToken cancellationToken)
        //{
        //    HttpRequestMessage httpRequestMessage = new();
        //    httpRequestMessage.Method = HttpMethod.Post;
        //    httpRequestMessage.Content = JsonContent.Create(model);
        //    httpRequestMessage.RequestUri = new Uri($"{BaseUrl}/CustomCreate");
        //    return await SendRequestAsync<Guid>(AddAuthenticationJWT(httpRequestMessage), nameof(CustomCreateAsync), cancellationToken);
        //}

        //public async Task<Bid?> CustomUpdateAsync(BidUpdateViewModel model, CancellationToken cancellationToken)
        //{
        //    HttpRequestMessage httpRequestMessage = new();
        //    httpRequestMessage.Method = HttpMethod.Put;
        //    httpRequestMessage.Content = JsonContent.Create(model);
        //    httpRequestMessage.RequestUri = new Uri($"{BaseUrl}/CustomUpdate");
        //    return await SendRequestAsync<Bid?>(AddAuthenticationJWT(httpRequestMessage), nameof(CustomUpdateAsync), cancellationToken);
        //}

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