using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Masofa.Common.Models.Identity;

namespace Masofa.Client.ApiClient.Repositrories.Identity
{
    public class UserDeviceRepository : BaseCrudRepository<UserDevice>
    {
        public UserDeviceRepository(HttpClient httpClient, string baseUrl) : base(httpClient, baseUrl)
        {
        }
    }
}
