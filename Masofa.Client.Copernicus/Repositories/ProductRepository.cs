using Masofa.Common.Models.Satellite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Masofa.Client.Copernicus.Repositories
{
    public class ProductRepository : BaseRepository
    {
        private readonly double LatMin, LatMax, LonMin, LonMax;

        public ProductRepository(
            ILogger logger, 
            HttpClient httpClient, 
            IConfiguration configuration) 
            : base(logger, httpClient)
        {
            LatMin = configuration.GetValue<double>("CountryBoundaries:LatMin");
            LatMax = configuration.GetValue<double>("CountryBoundaries:LatMax");
            LonMin = configuration.GetValue<double>("CountryBoundaries:LonMin");
            LonMax = configuration.GetValue<double>("CountryBoundaries:LonMax");
        }

        public async Task<Sentinel2ProductMetadata> GetProductMetadataAsync(SentinelServiceOptions options, Guid productId)
        {
            var url = $"{options.ProductSearchApiUrl}({productId})";

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Get;
            httpRequestMessage.RequestUri = new Uri(url);

            var result = await SendRequestAsync<Sentinel2ProductMetadata>(AddAuthenticationJWT(httpRequestMessage));

            return result;
        }

        public async Task<List<Guid>> SearchProductAsync(SentinelServiceOptions options, DateTime startAt, DateTime endAt)
        {
            try
            {
                string fromIso = startAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                string toIso = endAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                string defPolygon = $"POLYGON(({LonMin.ToString(CultureInfo.InvariantCulture)} {LatMin.ToString(CultureInfo.InvariantCulture)}, " +
                    $"{LonMax.ToString(CultureInfo.InvariantCulture)} {LatMin.ToString(CultureInfo.InvariantCulture)}, " +
                    $"{LonMax.ToString(CultureInfo.InvariantCulture)} {LatMax.ToString(CultureInfo.InvariantCulture)}, " +
                    $"{LonMin.ToString(CultureInfo.InvariantCulture)} {LatMax.ToString(CultureInfo.InvariantCulture)}, " +
                    $"{LonMin.ToString(CultureInfo.InvariantCulture)} {LatMin.ToString(CultureInfo.InvariantCulture)}))";
                //string wkt = options.SatelliteSearchConfig.SentinelPolygon?.AsText() ?? "POLYGON((56 37, 73 37, 73 45, 56 45, 56 37))";
                string wkt = options.SatelliteSearchConfig.SentinelPolygon?.AsText() ?? defPolygon;
                string formattedWkt = $"geography'SRID=4326;{wkt}'";

                var url = $"{options.ProductSearchApiUrl}?$format=json&$filter=" +
                    "Collection/Name eq 'SENTINEL-2'" +
                    " and Attributes/OData.CSC.StringAttribute/any(att: att/Name eq 'productType'" +
                    " and att/OData.CSC.StringAttribute/Value eq 'S2MSI1C')" +
                    $" and OData.CSC.Intersects(area={formattedWkt})" +
                    $" and ContentDate/Start gt {fromIso}" +
                    $" and ContentDate/Start lt {toIso}" +
                    " and Attributes/OData.CSC.DoubleAttribute/any(att: att/Name eq 'cloudCover'" +
                    " and att/OData.CSC.DoubleAttribute/Value le 20)" +
                    "&$orderby=ContentDate/Start desc";

                HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage.Method = HttpMethod.Get;
                httpRequestMessage.RequestUri = new Uri(url);

                var result = await SendRequestAsync<SearchProductViewModel>(AddAuthenticationJWT(httpRequestMessage));
                return result.value.Select(m => Guid.Parse(m.Id)).ToList();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception while getting access token");
                return null;
            }
        }

        public async Task<Stream> GetProductMediadataAsync(SentinelServiceOptions options, Guid productId)
        {

            var url = $"{options.ProductDownloadApiUrl}({productId})/$value";

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Get;
            httpRequestMessage.RequestUri = new Uri(url);

            return await GetStreamFromRequestAsync(AddAuthenticationJWT(httpRequestMessage));
        }

        public async Task<HttpResponseMessage> GetProductMediadataAsMessageAsync(SentinelServiceOptions options, Guid productId)
        {

            var url = $"{options.ProductDownloadApiUrl}({productId})/$value";

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Get;
            httpRequestMessage.RequestUri = new Uri(url);

            return await GetStreamFromRequestAsMessageAsync(AddAuthenticationJWT(httpRequestMessage));
        }
    }

    public class SearchProductViewModel
    {
        public List<SearchProductViewModelItem> value { get; set; }
    }

    public class SearchProductViewModelItem
    {
        public string Id { get; set; }
    }
}
