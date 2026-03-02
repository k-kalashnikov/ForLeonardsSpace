using Masofa.Common.Models.CropMonitoring;

namespace Masofa.Client.ApiClient.Repositrories.CropMonitoring
{
    public class FieldAgroProducerHistoryRepository : BaseCrudRepository<FieldAgroProducerHistory>
    {
        public FieldAgroProducerHistoryRepository(HttpClient httpClient, string baseUrl) : base(httpClient, baseUrl)
        {

        }
    }
}
