using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Masofa.Client.Era5.Repositories
{
    public class Era5WeatherDataRepository : Era5BaseRepository
    {
        private Era5Options Era5Options { get; }

        public Era5WeatherDataRepository(HttpClient httpClient, ILogger logger, IOptions<Era5Options> options) : base(httpClient, logger)
        {
            Era5Options = options.Value;
        }

        public async Task<TResponse> GetForecastWeatherDataAsync<TResponse>(string latitude, string longitude)
        {
            var uri = CreateForecastUri(latitude, longitude);

            HttpRequestMessage httpRequestMessage = new()
            {
                Method = HttpMethod.Get,
                RequestUri = uri
            };

            return await SendRequestAsync<TResponse>(httpRequestMessage);
        }

        public async Task<TResponse> GetHistoricalWeatherDataAsync<TResponse>(string latitude, string longitude, DateOnly startDate, DateOnly endDate)
        {
            var uri = CreateHistoricalUri(latitude, longitude, startDate, endDate);

            HttpRequestMessage httpRequestMessage = new()
            {
                Method = HttpMethod.Get,
                RequestUri = uri
            };

            return await SendRequestAsync<TResponse>(httpRequestMessage);
        }

        private Uri CreateForecastUri(string latitude, string longitude)
        {
            var builder = new UriBuilder(Era5Options.ForecastUrl);
            var queryParams = System.Web.HttpUtility.ParseQueryString(string.Empty);

            queryParams["latitude"] = latitude;
            queryParams["longitude"] = longitude;
            queryParams["hourly"] = Era5Options.RequestParams;
            queryParams["models"] = "ecmwf_ifs025";
            queryParams["wind_speed_unit"] = "ms";
            queryParams["timezone"] = "UTC";

            builder.Query = queryParams.ToString();
            return builder.Uri;
        }

        private Uri CreateHistoricalUri(string latitude, string longitude, DateOnly startDate, DateOnly endDate)
        {
            var url = Era5Options.ArchiveUrl;
            var builder = new UriBuilder(url);
            var queryParams = System.Web.HttpUtility.ParseQueryString(string.Empty);

            queryParams["latitude"] = latitude;
            queryParams["longitude"] = longitude;
            queryParams["start_date"] = startDate.ToString("yyyy-MM-dd");
            queryParams["end_date"] = endDate.ToString("yyyy-MM-dd");
            queryParams["hourly"] = Era5Options.RequestParams;
            queryParams["models"] = "era5";
            queryParams["timezone"] = "UTC";

            builder.Query = queryParams.ToString();
            return builder.Uri;
        }
    }
}
