using Masofa.BusinessLogic.Cli.DbSeeders;
using Masofa.Common.Models.Identity;
using Masofa.Common.Models.SystemCrical;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Logging;

namespace Masofa.Web.Monolith
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IdentityModelEventSource.ShowPII = true;

            var builder = WebApplication.CreateBuilder(args)
                .ConfigureCommonSettings();
            builder.Services.AddCommon(builder.Configuration);

            if (builder.Configuration.GetValue<bool>("MonolithConfiguration:StartSatelliteLandsat"))
            {
                builder.Services.AddSatelliteLandsat(builder.Configuration);
            }

            if (builder.Configuration.GetValue<bool>("MonolithConfiguration:StartSatelliteSentinel"))
            {
                builder.Services.AddSatelliteSentinel(builder.Configuration);
            }

            if (builder.Configuration.GetValue<bool>("MonolithConfiguration:StartEra"))
            {
                builder.Services.AddEra5(builder.Configuration);
            }

            if (builder.Configuration.GetValue<bool>("MonolithConfiguration:StartUgm"))
            {
                builder.Services.AddUgm(builder.Configuration);
            }

            if (builder.Configuration.GetValue<bool>("MonolithConfiguration:StartIBM"))
            {
                builder.Services.AddIBMWeather(builder.Configuration);
            }

            //builder.Services.AddQuartzJobs(builder.Configuration);

            // Добавляем сервисы для управления фоновыми задачами
            //builder.Services.AddSystemBackgroundTaskServices(builder.Configuration);

            // Регистрируем команды DevOps Utils
            //builder.Services.AddDevOpsUtilsCommands(builder.Configuration);
            builder.Services.AddScoped<DbSeeder>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
                await seeder.SeedAllAsync();
            }

            app.UseCors("AllowAll");


            app.UseSwagger().UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/Common/swagger.json", "Common API");
                c.SwaggerEndpoint("/swagger/Notifications/swagger.json", "Notifications API");
                if (builder.Configuration.GetValue<bool>("MonolithConfiguration:StartIdentity"))
                {
                    c.SwaggerEndpoint("/swagger/Identity/swagger.json", "Identity API");
                }

                if (builder.Configuration.GetValue<bool>("MonolithConfiguration:StartDictionaries"))
                {
                    c.SwaggerEndpoint("/swagger/Dictionaries/swagger.json", "Dictionaries API");
                }

                if (builder.Configuration.GetValue<bool>("MonolithConfiguration:StartCropMonitoring"))
                {
                    c.SwaggerEndpoint("/swagger/CropMonitoring/swagger.json", "Crop Monitoring API");
                }

                if (builder.Configuration.GetValue<bool>("MonolithConfiguration:StartSatelliteSentinel"))
                {
                    c.SwaggerEndpoint("/swagger/SatelliteSentinel/swagger.json", "Satellite Sentinel API");
                }

                if (builder.Configuration.GetValue<bool>("MonolithConfiguration:StartSatelliteLandsat"))
                {
                    c.SwaggerEndpoint("/swagger/SatelliteLandsat/swagger.json", "Satellite Landsat API");
                }

                if (builder.Configuration.GetValue<bool>("MonolithConfiguration:StartTiles"))
                {
                    c.SwaggerEndpoint("/swagger/Tiles/swagger.json", "Smart field map API");
                }

                if (builder.Configuration.GetValue<bool>("MonolithConfiguration:StartWeather"))
                {
                    c.SwaggerEndpoint("/swagger/Weather/swagger.json", "Weather API");
                    c.SwaggerEndpoint("/swagger/WeatherReports/swagger.json", "Weather Reports API");
                }

                if (builder.Configuration.GetValue<bool>("MonolithConfiguration:StartIndices"))
                {
                    c.SwaggerEndpoint("/swagger/SatelliteIndices/swagger.json", "Satillite Index API");
                }

                if (builder.Configuration.GetValue<bool>("MonolithConfiguration:StartIBM"))
                {
                    c.SwaggerEndpoint("/swagger/IBMWeather/swagger.json", "IBM Weather API");
                }

                if (builder.Configuration.GetValue<bool>("MonolithConfiguration:StartUav"))
                {
                    c.SwaggerEndpoint("/swagger/Uav/swagger.json", "Uav API");
                }
            });

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
