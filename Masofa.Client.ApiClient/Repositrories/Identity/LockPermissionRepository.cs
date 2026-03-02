using Masofa.Common.Models.Identity;

namespace Masofa.Client.ApiClient.Repositrories.Identity
{
    public class LockPermissionRepository : BaseCrudRepository<LockPermission>
    {
        public LockPermissionRepository(HttpClient httpClient, string baseUrl) : base(httpClient, baseUrl)
        {
        }
    }
}
