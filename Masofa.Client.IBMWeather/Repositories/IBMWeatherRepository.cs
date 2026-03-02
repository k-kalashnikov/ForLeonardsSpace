using Masofa.Client.IBMWeather.Models;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Masofa.Client.IBMWeather.Repositories
{
    /// <summary>
    /// Unified repository for all IBM Weather API endpoints
    /// </summary>
    public class IBMWeatherRepository : BaseIBMRepository
    {
        public IBMWeatherRepository(IHttpClientFactory httpClientFactory, ILogger logger)
            : base(httpClientFactory, logger)
        {
        }

        #region Location API

        /// <summary>
        /// Get location information by geocode coordinates
        /// Endpoint: /v3/location/point
        /// </summary>
        /// <param name="latitude">Latitude coordinate</param>
        /// <param name="longitude">Longitude coordinate</param>
        /// <returns>Location information</returns>
        public async Task<LocationPointResponse> GetLocationByGeocodeAsync(double latitude, double longitude)
        {
            var geocode = $"{latitude},{longitude}";
            return await GetLocationByGeocodeAsync(geocode);
        }

        /// <summary>
        /// Get location information by geocode string
        /// Endpoint: /v3/location/point
        /// </summary>
        /// <param name="geocode">Geocode string in format "lat,lon"</param>
        /// <returns>Location information</returns>
        public async Task<LocationPointResponse> GetLocationByGeocodeAsync(string geocode)
        {
            if (string.IsNullOrEmpty(geocode))
            {
                throw new ArgumentException("Geocode is required", nameof(geocode));
            }

            var parameters = new Dictionary<string, string>
            {
                ["geocode"] = geocode
            };

            var request = CreateGetRequest("/v3/location/point", parameters);
            return await SendRequestAsync<LocationPointResponse>(request);
        }

        /// <summary>
        /// Search for locations by query
        /// Endpoint: /v3/location/search
        /// </summary>
        /// <param name="query">Search query (e.g., "Uzbekistan", "Tashkent")</param>
        /// <param name="countryCode">Country code (e.g., "UZ")</param>
        /// <param name="locationType">Location types to search (e.g., "airport,metar,pws")</param>
        /// <returns>Location search results</returns>
        public async Task<LocationSearchResponse> SearchLocationsAsync(NetTopologySuite.Geometries.Point point, string? countryCode = null, string? locationType = null)
        {
            if (point is null)
            {
                throw new ArgumentException("Point is required", nameof(point));
            }

            var parameters = new Dictionary<string, string>
            {
                ["proximity"] = $"{point.Y},{point.X}"
            };

            if (!string.IsNullOrEmpty(countryCode))
            {
                parameters["countryCode"] = countryCode;
            }

            if (!string.IsNullOrEmpty(locationType))
            {
                parameters["locationType"] = locationType;
            }

            var request = CreateGetRequest("/v3/location/search", parameters);
            return await SendRequestAsync<LocationSearchResponse>(request);
        }

        #endregion

        #region Weather Alerts API

        /// <summary>
        /// Get weather alerts and warnings for a specific location
        /// Endpoint: /v3/alerts/headlines
        /// </summary>
        /// <param name="latitude">Latitude coordinate</param>
        /// <param name="longitude">Longitude coordinate</param>
        /// <returns>Weather alerts and warnings</returns>
        public async Task<WeatherAlertsResponse> GetWeatherAlertsAsync(NetTopologySuite.Geometries.Point point)
        {
            var geocode = $"{point.Y},{point.X}";
            return await GetWeatherAlertsAsync(geocode);
        }

        /// <summary>
        /// Get weather alerts and warnings for a specific location
        /// Endpoint: /v3/alerts/headlines
        /// </summary>
        /// <param name="geocode">Geocode string in format "lat,lon"</param>
        /// <returns>Weather alerts and warnings</returns>
        public async Task<WeatherAlertsResponse> GetWeatherAlertsAsync(string geocode)
        {
            if (string.IsNullOrEmpty(geocode))
            {
                throw new ArgumentException("Geocode is required", nameof(geocode));
            }

            var parameters = new Dictionary<string, string>
            {
                ["geocode"] = geocode
            };

            var request = CreateGetRequest("/v3/alerts/headlines", parameters);
            return await SendRequestAsync<WeatherAlertsResponse>(request);
        }

        #endregion

        #region Historical Data API

        /// <summary>
        /// Get 30-day historical daily weather summary
        /// Endpoint: /v3/wx/conditions/historical/dailysummary/30day
        /// </summary>
        /// <param name="latitude">Latitude coordinate</param>
        /// <param name="longitude">Longitude coordinate</param>
        /// <returns>30-day historical daily summary data</returns>
        public async Task<HistoricalDailySummaryResponse> GetHistoricalDailySummaryAsync(double latitude, double longitude)
        {
            var geocode = $"{latitude},{longitude}";
            return await GetHistoricalDailySummaryAsync(geocode);
        }

        /// <summary>
        /// Get 30-day historical daily weather summary
        /// Endpoint: /v3/wx/conditions/historical/dailysummary/30day
        /// </summary>
        /// <param name="geocode">Geocode string in format "lat,lon"</param>
        /// <returns>30-day historical daily summary data</returns>
        public async Task<HistoricalDailySummaryResponse> GetHistoricalDailySummaryAsync(string geocode)
        {
            if (string.IsNullOrEmpty(geocode))
            {
                throw new ArgumentException("Geocode is required", nameof(geocode));
            }

            var parameters = new Dictionary<string, string>
            {
                ["geocode"] = geocode
            };

            var request = CreateGetRequest("/v3/wx/conditions/historical/dailysummary/30day", parameters);
            return await SendRequestAsync<HistoricalDailySummaryResponse>(request);
        }

        /// <summary>
        /// Get 1-day historical hourly weather data
        /// Endpoint: /v3/wx/conditions/historical/hourly/1day
        /// </summary>
        /// <param name="latitude">Latitude coordinate</param>
        /// <param name="longitude">Longitude coordinate</param>
        /// <returns>1-day historical hourly data</returns>
        public async Task<HistoricalHourlyResponse> GetHistoricalHourlyAsync(double latitude, double longitude)
        {
            var geocode = $"{latitude},{longitude}";
            return await GetHistoricalHourlyAsync(geocode);
        }

        /// <summary>
        /// Get 1-day historical hourly weather data
        /// Endpoint: /v3/wx/conditions/historical/hourly/1day
        /// </summary>
        /// <param name="geocode">Geocode string in format "lat,lon"</param>
        /// <returns>1-day historical hourly data</returns>
        public async Task<HistoricalHourlyResponse> GetHistoricalHourlyAsync(string geocode)
        {
            if (string.IsNullOrEmpty(geocode))
            {
                throw new ArgumentException("Geocode is required", nameof(geocode));
            }

            var parameters = new Dictionary<string, string>
            {
                ["geocode"] = geocode
            };

            var request = CreateGetRequest("/v3/wx/conditions/historical/hourly/1day", parameters);
            return await SendRequestAsync<HistoricalHourlyResponse>(request);
        }

        #endregion

        #region Current Observations API

        /// <summary>
        /// Get current weather observations by coordinates
        /// Endpoint: /v1/geocode/{lat}/{lon}/observations.json
        /// </summary>
        /// <param name="latitude">Latitude coordinate</param>
        /// <param name="longitude">Longitude coordinate</param>
        /// <returns>Current weather observations</returns>
        public async Task<GeocodeObservationsResponse> GetObservationsAsync(NetTopologySuite.Geometries.Point point)
        {
            var latitude = point.Y;
            var longitude = point.X;

            if (latitude < -90 || latitude > 90)
            {
                throw new ArgumentException("Latitude must be between -90 and 90", nameof(latitude));
            }

            if (longitude < -180 || longitude > 180)
            {
                throw new ArgumentException("Longitude must be between -180 and 180", nameof(longitude));
            }

            var parameters = new Dictionary<string, string>();

            var endpoint = $"/v1/geocode/{latitude}/{longitude}/observations.json";
            var request = CreateGetRequest(endpoint, parameters);
            return await SendRequestAsync<GeocodeObservationsResponse>(request);
        }

        #endregion

        #region Forecast API

        /// <summary>
        /// Get 7-day daily weather forecast
        /// Endpoint: /v3/wx/forecast/daily/7day
        /// </summary>
        /// <param name="latitude">Latitude coordinate</param>
        /// <param name="longitude">Longitude coordinate</param>
        /// <returns>7-day daily forecast data</returns>
        public async Task<ForecastDailyResponse> GetDailyForecastAsync(NetTopologySuite.Geometries.Point point)
        {
            var geocode = $"{point.X.ToString(CultureInfo.InvariantCulture)},{point.Y.ToString(CultureInfo.InvariantCulture)}";
            return await GetDailyForecastAsync(geocode);
        }

        /// <summary>
        /// Get 10-day daily weather forecast
        /// Endpoint: /v3/wx/forecast/daily/7day
        /// </summary>
        /// <param name="geocode">Geocode string in format "lat,lon"</param>
        /// <returns>7-day daily forecast data</returns>
        public async Task<ForecastDailyResponse> GetDailyForecastAsync(string geocode)
        {
            if (string.IsNullOrEmpty(geocode))
            {
                throw new ArgumentException("Geocode is required", nameof(geocode));
            }

            var parameters = new Dictionary<string, string>
            {
                ["geocode"] = geocode
            };

            var request = CreateGetRequest("/v3/wx/forecast/daily/7day", parameters);
            return await SendRequestAsync<ForecastDailyResponse>(request);
        }

        #endregion

        #region HOD (Hour of Day) API

        /// <summary>
        /// Get HOD (Hour of Day) weather data
        /// Endpoint: /v3/wx/hod/r1/direct
        /// </summary>
        /// <param name="latitude">Latitude coordinate</param>
        /// <param name="longitude">Longitude coordinate</param>
        /// <param name="products">Comma-separated list of weather products to retrieve</param>
        /// <returns>HOD weather data</returns>
        public async Task<HodDirectResponse> GetHodDirectAsync(double latitude, double longitude, string? products = null)
        {
            var geocode = $"{latitude},{longitude}";
            return await GetHodDirectAsync(geocode, products);
        }

        /// <summary>
        /// Get HOD (Hour of Day) weather data
        /// Endpoint: /v3/wx/hod/r1/direct
        /// </summary>
        /// <param name="geocode">Geocode string in format "lat,lon"</param>
        /// <param name="products">Comma-separated list of weather products to retrieve</param>
        /// <returns>HOD weather data</returns>
        public async Task<HodDirectResponse> GetHodDirectAsync(string geocode, string? products = null)
        {
            if (string.IsNullOrEmpty(geocode))
            {
                throw new ArgumentException("Geocode is required", nameof(geocode));
            }

            var parameters = new Dictionary<string, string>
            {
                ["geocode"] = geocode
            };

            if (!string.IsNullOrEmpty(products))
            {
                parameters["products"] = products;
            }

            var request = CreateGetRequest("/v3/wx/hod/r1/direct", parameters);
            return await SendRequestAsync<HodDirectResponse>(request);
        }

        /// <summary>
        /// Get HOD weather data with default products
        /// Endpoint: /v3/wx/hod/r1/direct
        /// </summary>
        /// <param name="latitude">Latitude coordinate</param>
        /// <param name="longitude">Longitude coordinate</param>
        /// <returns>HOD weather data with default products</returns>
        public async Task<HodDirectResponse> GetHodDirectWithDefaultProductsAsync(NetTopologySuite.Geometries.Point point)
        {
            var defaultProducts = "temperature,temperatureDewPoint,evapotranspiration,uvIndex,precip1Hour,windSpeed,windDirection";
            return await GetHodDirectAsync(point.X, point.Y, defaultProducts);
        }

        #endregion
    }
}
