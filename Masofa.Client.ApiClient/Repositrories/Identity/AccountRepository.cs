using Masofa.Common.ViewModels.Account;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Masofa.Client.ApiClient.Repositrories.Identity
{
    public class AccountRepository : BaseRepository
    {
        public AccountRepository(HttpClient httpClient, string baseUrl) : base(httpClient, baseUrl)
        {

        }

        public async Task<string> LoginAsync(LoginAndPasswordViewModel viewModel, CancellationToken cancellationToken)
        {
            HttpRequestMessage httpRequestMessage = new()
            {
                Method = HttpMethod.Post,
                Content = JsonContent.Create<LoginAndPasswordViewModel>(viewModel),
                RequestUri = new Uri($"{BaseUrl}/LoginByLoginPassword")
            };
            return await SendRequestAsync<string>(httpRequestMessage, "login", cancellationToken);
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordViewModel viewModel, CancellationToken cancellationToken)
        {
            HttpRequestMessage httpRequestMessage = new()
            {
                Method = HttpMethod.Post,
                Content = JsonContent.Create<ChangePasswordViewModel>(viewModel),
                RequestUri = new Uri($"{BaseUrl}/ChangePassword")
            };
            return await SendRequestAsync<bool>(AddAuthenticationJWT(httpRequestMessage), "change-password", cancellationToken);
        }

        public async Task<ProfileInfoViewModel?> GetProfileInfoAsync(CancellationToken cancellationToken)
        {
            HttpRequestMessage httpRequestMessage = new()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{BaseUrl}/GetProfileInfo")
            };
            return await SendRequestAsync<ProfileInfoViewModel?>(AddAuthenticationJWT(httpRequestMessage), "get-profile", cancellationToken);
        }

        public async Task ForgotPasswordAsync(ForgotPasswordViewModel viewModel, CancellationToken cancellationToken)
        {
            HttpRequestMessage httpRequestMessage = new()
            {
                Method = HttpMethod.Post,
                Content = JsonContent.Create(viewModel),
                RequestUri = new Uri($"{BaseUrl}/ForgotPassword")
            };
            await SendRequestAsync<object>(httpRequestMessage, "forgot-password", cancellationToken);
        }

        public async Task ResetPasswordAsync(ResetPasswordViewModel viewModel, CancellationToken cancellationToken)
        {
            HttpRequestMessage httpRequestMessage = new()
            {
                Method = HttpMethod.Post,
                Content = JsonContent.Create(viewModel),
                RequestUri = new Uri($"{BaseUrl}/ResetPassword")
            };
            await SendRequestAsync<object>(httpRequestMessage, "reset-password", cancellationToken);
        }

        //public async Task<Masofa.Common.Models.Identity.User?> UpdateAsync(UpdateViewModel viewModel, CancellationToken cancellationToken)
        //{
        //    HttpRequestMessage httpRequestMessage = new()
        //    {
        //        Method = HttpMethod.Put,
        //        Content = JsonContent.Create(viewModel),
        //        RequestUri = new Uri($"{BaseUrl}/Update")
        //    };
        //    return await SendRequestAsync<Masofa.Common.Models.Identity.User?>(httpRequestMessage, "update", cancellationToken);
        //}
    }
}
