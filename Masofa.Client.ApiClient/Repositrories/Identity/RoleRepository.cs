using Masofa.Common.Models;
using Masofa.Common.Models.Identity;
using Masofa.Common.ViewModels.Account;

namespace Masofa.Client.ApiClient.Repositories.Identity
{
    public class RoleRepository : BaseRepository
    {
        public RoleRepository(HttpClient httpClient, string baseUrl) : base(httpClient, baseUrl) { }

        public async Task<List<ProfileInfoViewModel>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            HttpRequestMessage httpRequestMessage = new();
            httpRequestMessage.Method = HttpMethod.Get;
            httpRequestMessage.RequestUri = new Uri($"{BaseUrl}/GetUsersInRole?roleName={roleName}");
            return await SendRequestAsync<List<ProfileInfoViewModel>>(AddAuthenticationJWT(httpRequestMessage), "role-get-users", cancellationToken);
        }

        public async Task<List<Role>> GetRolesAsync(CancellationToken cancellationToken)
        {
            HttpRequestMessage httpRequestMessage = new();
            httpRequestMessage.Method = HttpMethod.Get;
            httpRequestMessage.RequestUri = new Uri($"{BaseUrl}/GetRoles");
            return await SendRequestAsync<List<Role>>(AddAuthenticationJWT(httpRequestMessage), "role-get-all", cancellationToken);
        }
    }
}