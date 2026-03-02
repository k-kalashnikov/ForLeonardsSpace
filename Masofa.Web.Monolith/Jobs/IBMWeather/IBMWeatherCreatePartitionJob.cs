using Masofa.Common.Models;
using Masofa.Common.Models.IBMWeather;
using Masofa.Common.Services;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Masofa.Web.Monolith.Jobs.IBMWeather
{
    /// <summary>
    /// Job для создания партиций таблицы WeatherData для IBM Weather API
    /// </summary>
    public class IBMWeatherCreatePartitionJob : IJob
    {
        private ILogger<IBMWeatherCreatePartitionJob> _logger;
        private MasofaIBMWeatherDbContext MasofaIBMWeatherDbContext { get; set; }

        public IBMWeatherCreatePartitionJob(ILogger<IBMWeatherCreatePartitionJob> logger, MasofaIBMWeatherDbContext masofaIBMWeatherDbContext)
        {
            _logger = logger;
            MasofaIBMWeatherDbContext = masofaIBMWeatherDbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var tomorrow = DateTime.Today.AddDays(7);
            
            // Получаем имя таблицы через рефлексию из атрибута Table
            var tableName = typeof(IBMWeatherData).GetCustomAttribute<TableAttribute>()?.Name ?? typeof(IBMWeatherData).Name;

            await CreatePartitionForWeatherData(tableName, tomorrow);
        }

        private async Task CreatePartitionForWeatherData(string modelName, DateTime date)
        {
            var tableName = $"{modelName}_{date:yyyy_MM_dd}";
            var startDate = date.ToString("yyyy-MM-dd");
            var endDate = date.AddDays(1).ToString("yyyy-MM-dd");

            if (MasofaIBMWeatherDbContext.IsTableExists(tableName))
            {
                _logger.LogInformation("Партиция {TableName} уже существует", tableName);
                return;
            }

            var sql = $"CREATE TABLE \"{tableName}\" PARTITION OF \"{modelName}\" FOR VALUES FROM ('{startDate}') TO ('{endDate}');";

            try
            {
                await MasofaIBMWeatherDbContext.Database.ExecuteSqlRawAsync(sql);
                _logger.LogInformation("Создана партиция для IBM Weather Data: {TableName}", tableName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании партиции {TableName}", tableName);
            }
        }
    }
}
