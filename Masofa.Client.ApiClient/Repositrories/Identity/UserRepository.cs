using System.Net.Http.Json;
using Masofa.Common.Models;
using Masofa.Common.Models.Identity;
using Masofa.Common.ViewModels.Account;

namespace Masofa.Client.ApiClient.Repositories.Identity
{
    public class UserRepository : BaseRepository
    {
        public UserRepository(HttpClient httpClient, string baseUrl) : base(httpClient, baseUrl) { }

        //public async Task<UserCreateCommandResult> CreateAsync(CreateViewModel model, CancellationToken cancellationToken)
        //{
        //    HttpRequestMessage httpRequestMessage = new();
        //    httpRequestMessage.Method = HttpMethod.Post;
        //    httpRequestMessage.Content = JsonContent.Create(model);
        //    httpRequestMessage.RequestUri = new Uri($"{BaseUrl}/Create");
        //    return await SendRequestAsync<UserCreateCommandResult>(AddAuthenticationJWT(httpRequestMessage), "user-create", cancellationToken);
        //}

        public async Task ResendNewAccountLetterAsync(Guid id, CancellationToken cancellationToken)
        {
            HttpRequestMessage httpRequestMessage = new();
            httpRequestMessage.Method = HttpMethod.Post;
            httpRequestMessage.RequestUri = new Uri($"{BaseUrl}/ResendNewAccountLetter/{id}");
            await SendRequestAsync<object>(AddAuthenticationJWT(httpRequestMessage), "user-resend-letter", cancellationToken);
        }

        public async Task<List<ProfileInfoViewModel>> GetByQueryAsync(UserGetQuery query, CancellationToken cancellationToken)
        {
            HttpRequestMessage httpRequestMessage = new();
            httpRequestMessage.Method = HttpMethod.Post;
            httpRequestMessage.Content = JsonContent.Create(query);
            httpRequestMessage.RequestUri = new Uri($"{BaseUrl}/GetByQuery");
            return await SendRequestAsync<List<ProfileInfoViewModel>>(AddAuthenticationJWT(httpRequestMessage), "user-get-by-query", cancellationToken);
        }

        public async Task<ProfileInfoViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            HttpRequestMessage httpRequestMessage = new();
            httpRequestMessage.Method = HttpMethod.Get;
            httpRequestMessage.RequestUri = new Uri($"{BaseUrl}/GetById/{id}");
            return await SendRequestAsync<ProfileInfoViewModel?>(AddAuthenticationJWT(httpRequestMessage), "user-get-by-id", cancellationToken);
        }

        public async Task<List<ProfileInfoViewModel>> GetChildUsersAsync(CancellationToken cancellationToken)
        {
            HttpRequestMessage httpRequestMessage = new();
            httpRequestMessage.Method = HttpMethod.Get;
            httpRequestMessage.RequestUri = new Uri($"{BaseUrl}/GetChildUsers");
            return await SendRequestAsync<List<ProfileInfoViewModel>>(AddAuthenticationJWT(httpRequestMessage), "user-get-children", cancellationToken);
        }

        //public async Task<BulkUserCreateResult> ImportUserPersonAsync(MultipartFormDataContent content, CancellationToken cancellationToken)
        //{
        //    HttpRequestMessage httpRequestMessage = new();
        //    httpRequestMessage.Method = HttpMethod.Post;
        //    httpRequestMessage.Content = content;
        //    httpRequestMessage.RequestUri = new Uri($"{BaseUrl}/ImportUserPerson");
        //    return await SendRequestAsync<BulkUserCreateResult>(AddAuthenticationJWT(httpRequestMessage), "user-import-person", cancellationToken);
        //}

        //public async Task<BulkUserCreateResult> ImportUserFirmAsync(MultipartFormDataContent content, CancellationToken cancellationToken)
        //{
        //    HttpRequestMessage httpRequestMessage = new();
        //    httpRequestMessage.Method = HttpMethod.Post;
        //    httpRequestMessage.Content = content;
        //    httpRequestMessage.RequestUri = new Uri($"{BaseUrl}/ImportUserFirm");
        //    return await SendRequestAsync<BulkUserCreateResult>(AddAuthenticationJWT(httpRequestMessage), "user-import-firm", cancellationToken);
        //}
    }
}