using Masofa.Common.Models;
using Masofa.Common.Models.Identity;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using Masofa.Web.Monolith.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Masofa.BusinessLogic.Cli.DbSeeders
{
    public class SystemBackgroundTaskDbSeeder
    {
        private MasofaCommonDbContext CommonDbContext { get; set; }
        private MasofaIdentityDbContext IdentityDbContext { get; set; }

        public SystemBackgroundTaskDbSeeder(MasofaIdentityDbContext identityDbContext, MasofaCommonDbContext commonDbContext)
        {
            IdentityDbContext = identityDbContext;
            CommonDbContext = commonDbContext;
        }

        public async Task SeedAsync()
        {
            await SeedTasks();
        }

        private async Task SeedTasks()
        {
            var lastUpdateUser = await IdentityDbContext.Set<User>().FirstAsync(m => m.UserName.ToLower().Equals("admin"));

            var defaultDelay = 30;
            var defaultFrequency = 24;

            List<(string executeTypeName, int delay, string group, int frequency)> jobs = [];
            jobs.Add(("System.CreatePartitionJob", defaultDelay, "system", defaultFrequency));
            jobs.Add(("System.HealthCheckJob", defaultDelay, "system", defaultFrequency));

            //jobs.Add(("SatelliteIndices.Db.EviDbJob", defaultDelay, "indices", defaultFrequency));
            //jobs.Add(("SatelliteIndices.Db.GndviDbJob", defaultDelay, "indices", defaultFrequency));
            //jobs.Add(("SatelliteIndices.Db.MndwiDbJob", defaultDelay, "indices", defaultFrequency));
            //jobs.Add(("SatelliteIndices.Db.NdmiDbJob", defaultDelay, "indices", defaultFrequency));
            //jobs.Add(("SatelliteIndices.Db.NdviDbJob", defaultDelay, "indices", defaultFrequency));
            //jobs.Add(("SatelliteIndices.Db.OrviDbJob", defaultDelay, "indices", defaultFrequency));
            //jobs.Add(("SatelliteIndices.Db.OsaviDbJob", defaultDelay, "indices", defaultFrequency));

            //jobs.Add(("SatelliteIndices.Tiff.EVIJob", defaultDelay, "indices", defaultFrequency));
            //jobs.Add(("SatelliteIndices.Tiff.GNDVIJob", defaultDelay, "indices", defaultFrequency));
            //jobs.Add(("SatelliteIndices.Tiff.MNDWIJob", defaultDelay, "indices", defaultFrequency));
            //jobs.Add(("SatelliteIndices.Tiff.NDMIJob", defaultDelay, "indices", defaultFrequency));
            //jobs.Add(("SatelliteIndices.Tiff.NDVIJob", defaultDelay, "indices", defaultFrequency));
            //jobs.Add(("SatelliteIndices.Tiff.ORVIJob", defaultDelay, "indices", defaultFrequency));
            //jobs.Add(("SatelliteIndices.Tiff.OSAVIJob", defaultDelay, "indices", defaultFrequency));

            jobs.Add(("Sentinel2.Sentinel2SearchProductJob", defaultDelay, "sentinel", defaultFrequency));
            jobs.Add(("Sentinel2.Sentinel2MetadataLoaderJob", defaultDelay, "sentinel", defaultFrequency));
            jobs.Add(("Sentinel2.Sentinel2MediaLoaderJob", defaultDelay, "sentinel", defaultFrequency));
            jobs.Add(("Sentinel2.Sentinel2ArchiveParsingJob", defaultDelay, "sentinel", defaultFrequency));
            jobs.Add(("SatelliteIndices.ParallelesPointAndTiffFetchJob", defaultDelay, "sentinel", defaultFrequency));
            jobs.Add(("Sentinel2.AnomaliesPointAndTiffJob", 1800, "sentinel", defaultFrequency));
            jobs.Add(("Sentinel2.Sentinel2PreviewImageJob", 1800, "sentinel", defaultFrequency));
            jobs.Add(("Sentinel2.Sentinel2ConvertTilesJob", defaultDelay, "sentinel", defaultFrequency));

            jobs.Add(("Weather.EraWeatherDataLoaderJob", defaultDelay, "weatherreport", defaultFrequency));
            jobs.Add(("WeatherReport.Era5WeatherReportsCalculationJob", 3600, "weatherreport", defaultFrequency));
            jobs.Add(("WeatherReport.Era5WeatherReportsTilesGenerationJob", 7200, "weatherreport", defaultFrequency));
            jobs.Add(("Weather.UgmWeatherCurrentDataLoaderJob", 300, "weatherreport", defaultFrequency));
            jobs.Add(("WeatherReport.UgmWeatherCurrentDataLoaderJob", 300, "weatherreport", defaultFrequency));
            jobs.Add(("WeatherReport.UgmCurrentWeatherReportsTilesGenerationJob", 600, "weatherreport", defaultFrequency));
            jobs.Add(("Weather.UgmWeatherForecastDataLoaderJob", defaultDelay, "weatherreport", defaultFrequency));
            jobs.Add(("Weather.UgmWeatherForecastDataLoaderJob", defaultDelay, "weatherreport", defaultFrequency));
            jobs.Add(("WeatherReport.UgmForecastWeatherReportsTilesGenerationJob", 1200, "weatherreport", defaultFrequency));

            jobs.Add(("IBMWeather.LoadForecastDataJob", defaultDelay, "weatherreport", defaultFrequency));
            jobs.Add(("WeatherReport.IbmWeatherReportsCalculationJob", 3600, "weatherreport", defaultFrequency));
            jobs.Add(("WeatherReport.IbmWeatherReportsTilesGenerationJob", 7200, "weatherreport", defaultFrequency));
            jobs.Add(("AnaliticReport.CollectQwenFullAnalysisToReportJob", 7200, "analiticreport", defaultFrequency));
            jobs.Add(("AnaliticReport.GenerateAnaliticReportJob", 7200, "analiticreport", defaultFrequency));
            jobs.Add(("QwenAnalysis.BidSentToQwenJob", 7200, "qwenanalysis", defaultFrequency));
            jobs.Add(("QwenAnalysis.FetchQwenResultsJob", 7200, "qwenanalysis", defaultFrequency));

            var allJobNames = await CommonDbContext.SystemBackgroundTasks.Select(t => t.ExecuteTypeName).ToHashSetAsync();
            List<SystemBackgroundTask> jobsToAdd = [];
            foreach (var (executeTypeName, delay, group, frequency) in jobs)
            {
                var jobName = $"Masofa.Web.Monolith.Jobs.{executeTypeName}";
                if (allJobNames.Contains(jobName)) continue;

                var newSystemBackgroundTask = new SystemBackgroundTask()
                {
                    CreateAt = DateTime.UtcNow,
                    CreateUser = lastUpdateUser.Id,
                    LastUpdateUser = lastUpdateUser.Id,
                    LastUpdateAt = DateTime.UtcNow,
                    Status = StatusType.Active,
                    CurrentRetryCount = 0,
                    IsActive = true,
                    ExecuteTypeName = jobName,
                    IsRetryable = true,
                    TaskType = SystemBackgroundTaskType.Schedule,
                    ParametrsJson = Newtonsoft.Json.JsonConvert.SerializeObject(new ScheduleTaskOptions()
                    {
                        StartDelaySeconds = delay,
                        GroupName = group,
                        Type = ScheduleType.Interval,
                        Interval = ScheduleInterval.Hours,
                        Frequency = frequency
                    }),
                    Names = BackgroundTaskLocalization.GetLocalization(executeTypeName.Split('.')[^1]),
                };

                jobsToAdd.Add(newSystemBackgroundTask);
            }

            if (jobsToAdd.Count > 0)
            {
                await CommonDbContext.SystemBackgroundTasks.AddRangeAsync(jobsToAdd);
                await CommonDbContext.SaveChangesAsync();
            }
        }
    }
}
