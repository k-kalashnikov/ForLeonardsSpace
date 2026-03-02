using NLog.Extensions.Logging;
using NLog.Web;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebApplicationBuilderExtention
    {
        public static WebApplicationBuilder ConfigureCommonSettings(this WebApplicationBuilder builder)
        {
            builder.Configuration
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json");

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = 1024 * 1024 * 1024;
                options.Limits.MaxRequestBufferSize = 1024 * 1024 * 1024;
            });

            builder.Logging.ClearProviders();
            NLog.LogManager.Configuration = new NLogLoggingConfiguration(builder.Configuration.GetSection("NLog"));
            builder.Host.UseNLog();

            return builder;
        }
    }
}
