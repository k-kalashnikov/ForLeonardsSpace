using Masofa.Common.Models.Ugm;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data;

namespace Masofa.BusinessLogic.Ugm
{
    public class UgmWeatherDataUpsertCommand : IRequest<Guid>
    {
        public int RegionId { get; set; }
        public DateOnly? Date { get; set; }
        public DateTime? DateTime { get; set; }
        public DayPart? DayPart { get; set; }
        public string? Icon { get; set; }
        public double? AirTMin { get; set; }
        public double? AirTMax { get; set; }
        public int? WindDirection { get; set; }
        public int? WindDirectionChange { get; set; }
        public int? WindSpeedMin { get; set; }
        public int? WindSpeedMax { get; set; }
        public int? WindSpeedMinAfterChange { get; set; }
        public int? WindSpeedMaxAfterChange { get; set; }
        public string? CloudAmount { get; set; }
        public string? TimePeriod { get; set; }
        public string? Precipitation { get; set; }
        public int? IsOccasional { get; set; }
        public int? IsPossible { get; set; }
        public int? Thunderstorm { get; set; }
        public string? Location { get; set; }
        public string? WeatherCode { get; set; }
    }

    public class UgmWeatherDataUpsertCommandHandler : IRequestHandler<UgmWeatherDataUpsertCommand, Guid>
    {
        private ILogger Logger { get; set; }
        private MasofaUgmDbContext UgmDbContext { get; set; }

        public UgmWeatherDataUpsertCommandHandler(ILogger<UgmWeatherDataUpsertCommandHandler> logger, MasofaUgmDbContext ugmDbContext)
        {
            Logger = logger;
            UgmDbContext = ugmDbContext;
        }

        public async Task<Guid> Handle(UgmWeatherDataUpsertCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                var partitionDate = request.Date;
                if (partitionDate == null)
                {
                    return Guid.Empty;
                }

                var modelName = "UgmWeatherData";
                var tableName = $"\"{modelName}_{partitionDate:yyyy_MM_dd}\"";

                var sql = $@"
                    INSERT INTO {tableName} (
                        ""Id"", ""RegionId"", ""Date"", ""DayPart"", ""Icon"", ""AirTMin"", ""AirTMax"", 
                        ""WindDirection"", ""WindDirectionChange"", ""WindSpeedMin"", ""WindSpeedMax"", 
                        ""WindSpeedMinAfterChange"", ""WindSpeedMaxAfterChange"", ""CloudAmount"",
                        ""TimePeriod"", ""Precipitation"", ""IsOccasional"", ""IsPossible"", 
                        ""Thunderstorm"", ""Location"", ""DateTime"", ""WeatherCode""
                    )
                    VALUES (
                        @id, @regionId, @date, @dayPart, @icon, @airTMin, @airTMax, @windDirection, 
                        @windDirectionChange, @windSpeedMin, @windSpeedMax, @windSpeedMinAfterChange, 
                        @windSpeedMaxAfterChange, @cloudAmount, @timePeriod, @precipitation, @isOccasional,
                        @isPossible, @thunderstorm, @location, @dateTime, @weatherCode
                    )
                    ON CONFLICT (""RegionId"", ""Date"", ""DayPart"")
                    DO UPDATE SET
                        ""Icon"" = EXCLUDED.""Icon"",
                        ""AirTMin"" = EXCLUDED.""AirTMin"",
                        ""AirTMax"" = EXCLUDED.""AirTMax"",
                        ""WindDirection"" = EXCLUDED.""WindDirection"",
                        ""WindDirectionChange"" = EXCLUDED.""WindDirectionChange"",
                        ""WindSpeedMin"" = EXCLUDED.""WindSpeedMin"",
                        ""WindSpeedMax"" = EXCLUDED.""WindSpeedMax"",
                        ""WindSpeedMinAfterChange"" = EXCLUDED.""WindSpeedMinAfterChange"",
                        ""WindSpeedMaxAfterChange"" = EXCLUDED.""WindSpeedMaxAfterChange"",
                        ""CloudAmount"" = EXCLUDED.""CloudAmount"",
                        ""TimePeriod"" = EXCLUDED.""TimePeriod"",
                        ""Precipitation"" = EXCLUDED.""Precipitation"",
                        ""IsOccasional"" = EXCLUDED.""IsOccasional"",
                        ""IsPossible"" = EXCLUDED.""IsPossible"",
                        ""Thunderstorm"" = EXCLUDED.""Thunderstorm"",
                        ""Location"" = EXCLUDED.""Location"",
                        ""DateTime"" = EXCLUDED.""DateTime"",
                        ""WeatherCode"" = EXCLUDED.""WeatherCode""
                    RETURNING ""Id""";

                var parameters = new[]
                {
                    new NpgsqlParameter("@id", Guid.NewGuid()),
                    new NpgsqlParameter("@regionId", request.RegionId),
                    new NpgsqlParameter("@date", request.Date),
                    new NpgsqlParameter("@dayPart", (int)request.DayPart),
                    new NpgsqlParameter("@icon", (object)request.Icon ?? DBNull.Value),
                    new NpgsqlParameter("@airTMin", (object)request.AirTMin ?? DBNull.Value),
                    new NpgsqlParameter("@airTMax", (object)request.AirTMax ?? DBNull.Value),
                    new NpgsqlParameter("@windDirection", (object)request.WindDirection ?? DBNull.Value),
                    new NpgsqlParameter("@windDirectionChange", (object)request.WindDirectionChange ?? DBNull.Value),
                    new NpgsqlParameter("@windSpeedMin", (object)request.WindSpeedMin ?? DBNull.Value),
                    new NpgsqlParameter("@windSpeedMax", (object)request.WindSpeedMax ?? DBNull.Value),
                    new NpgsqlParameter("@windSpeedMinAfterChange", (object)request.WindSpeedMinAfterChange ?? DBNull.Value),
                    new NpgsqlParameter("@windSpeedMaxAfterChange", (object)request.WindSpeedMaxAfterChange ?? DBNull.Value),
                    new NpgsqlParameter("@cloudAmount", (object)request.CloudAmount ?? DBNull.Value),
                    new NpgsqlParameter("@timePeriod", (object)request.TimePeriod ?? DBNull.Value),
                    new NpgsqlParameter("@precipitation", (object)request.Precipitation ?? DBNull.Value),
                    new NpgsqlParameter("@isOccasional", (object)request.IsOccasional ?? DBNull.Value),
                    new NpgsqlParameter("@isPossible", (object)request.IsPossible ?? DBNull.Value),
                    new NpgsqlParameter("@thunderstorm", (object)request.Thunderstorm ?? DBNull.Value),
                    new NpgsqlParameter("@location", (object)request.Location ?? DBNull.Value),
                    new NpgsqlParameter("@dateTime", (object)request.DateTime ?? DBNull.Value),
                    new NpgsqlParameter("@weatherCode", (object)request.WeatherCode ?? DBNull.Value)
                };

                var connection = UgmDbContext.Database.GetDbConnection();
                if (connection.State == ConnectionState.Closed)
                {
                    await connection.OpenAsync(cancellationToken);
                }

                if (!(await DoesTableExistAsync((NpgsqlConnection)connection, tableName, cancellationToken)))
                {
                    await CreatePartitionForDateAsync((NpgsqlConnection)connection, modelName, partitionDate.Value, cancellationToken);
                }

                var command = new NpgsqlCommand(sql, (NpgsqlConnection)connection)
                {
                    CommandTimeout = 30
                };
                command.Parameters.AddRange(parameters);

                try
                {
                    var scalar = await command.ExecuteScalarAsync(cancellationToken);
                    return (Guid)scalar;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        await connection.CloseAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {requestPath}. {ex.Message}";
                Logger.LogCritical(ex, msg);
                throw ex;
            }
        }

        private async Task<bool> DoesTableExistAsync(NpgsqlConnection? connection, string tableName, CancellationToken ct)
        {
            using var cmd = new NpgsqlCommand(@"
                SELECT EXISTS (
                    SELECT FROM pg_tables 
                    WHERE schemaname = 'public' 
                        AND tablename = @tableName
                );", connection);

            cmd.Parameters.AddWithValue("@tableName", tableName.Replace("\"", ""));

            var result = await cmd.ExecuteScalarAsync(ct);
            return (bool)result;
        }

        private async Task CreatePartitionForDateAsync(NpgsqlConnection connection, string modelName, DateOnly date, CancellationToken ct)
        {
            var tableName = $"{modelName}_{date:yyyy_MM_dd}";
            var nextDay = date.AddDays(1);

            var sql = $"CREATE TABLE \"{tableName}\" PARTITION OF \"{modelName}\" FOR VALUES FROM ('{date:yyyy-MM-dd}') TO ('{nextDay:yyyy-MM-dd}');";

            using var cmd = new NpgsqlCommand(sql, connection);
            await cmd.ExecuteNonQueryAsync(ct);
        }
    }
}
