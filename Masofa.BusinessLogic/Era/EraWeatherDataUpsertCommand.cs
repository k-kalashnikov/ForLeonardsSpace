using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data;

namespace Masofa.BusinessLogic.Era
{
    public class EraWeatherDataUpsertCommand : IRequest<Guid>
    {
        public DateTime OriginalDateTimeUtc { get; set; }
        public double? Temperature { get; set; }
        public double? RelativeHumidity { get; set; }
        public double? DewPoint { get; set; }
        public double? Precipitation { get; set; }
        public double? CloudCover { get; set; }
        public double? WindSpeed { get; set; }
        public double? WindDirection { get; set; }
        public double? GroundTemperature { get; set; }
        public double? SoilTemperature { get; set; }
        public int? ConditionIds { get; set; }
        public double? SoilHumidity50cm { get; set; }
        public double? SoilHumidity2m { get; set; }
        public double? SolarRadiation { get; set; }
        public Guid EraWeatherStationId { get; set; }
    }

    public class EraWeatherDataUpsertCommandHandler : IRequestHandler<EraWeatherDataUpsertCommand, Guid>
    {
        private ILogger Logger { get; set; }
        private MasofaEraDbContext EraDbContext { get; set; }

        public EraWeatherDataUpsertCommandHandler(ILogger<EraWeatherDataUpsertCommandHandler> logger, MasofaEraDbContext eraDbContext)
        {
            Logger = logger;
            EraDbContext = eraDbContext;
        }

        public async Task<Guid> Handle(EraWeatherDataUpsertCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                var partitionDate = request.OriginalDateTimeUtc.Date;
                var modelName = "EraWeatherData";
                var tableName = $"\"{modelName}_{partitionDate:yyyy_MM_dd}\"";

                var sql = $@"
                    INSERT INTO {tableName} (
                        ""Id"", ""OriginalDateTimeUtc"", ""EraWeatherStationId"",
                        ""Temperature"", ""RelativeHumidity"", ""DewPoint"", ""Precipitation"",
                        ""CloudCover"", ""WindSpeed"", ""WindDirection"", ""GroundTemperature"",
                        ""SoilTemperature"", ""ConditionIds"", ""SoilHumidity50cm"", ""SoilHumidity2m"",
                        ""SolarRadiation""
                    )
                    VALUES (
                        @id, @originalDateTimeUtc, @eraWeatherStationId,
                        @temperature, @relativeHumidity, @dewPoint, @precipitation,
                        @cloudCover, @windSpeed, @windDirection, @groundTemperature,
                        @soilTemperature, @conditionIds, @soilHumidity50cm, @soilHumidity2m,
                        @solarRadiation
                    )
                    ON CONFLICT (""OriginalDateTimeUtc"", ""EraWeatherStationId"")
                    DO UPDATE SET
                        ""Temperature"" = EXCLUDED.""Temperature"",
                        ""RelativeHumidity"" = EXCLUDED.""RelativeHumidity"",
                        ""DewPoint"" = EXCLUDED.""DewPoint"",
                        ""Precipitation"" = EXCLUDED.""Precipitation"",
                        ""CloudCover"" = EXCLUDED.""CloudCover"",
                        ""WindSpeed"" = EXCLUDED.""WindSpeed"",
                        ""WindDirection"" = EXCLUDED.""WindDirection"",
                        ""GroundTemperature"" = EXCLUDED.""GroundTemperature"",
                        ""SoilTemperature"" = EXCLUDED.""SoilTemperature"",
                        ""ConditionIds"" = EXCLUDED.""ConditionIds"",
                        ""SoilHumidity50cm"" = EXCLUDED.""SoilHumidity50cm"",
                        ""SoilHumidity2m"" = EXCLUDED.""SoilHumidity2m"",
                        ""SolarRadiation"" = EXCLUDED.""SolarRadiation""
                    RETURNING ""Id""";

                var parameters = new[]
                {
                    new NpgsqlParameter("@id", Guid.NewGuid()),
                    new NpgsqlParameter("@originalDateTimeUtc", request.OriginalDateTimeUtc),
                    new NpgsqlParameter("@eraWeatherStationId", request.EraWeatherStationId),
                    new NpgsqlParameter("@temperature", (object)request.Temperature ?? DBNull.Value),
                    new NpgsqlParameter("@relativeHumidity", (object)request.RelativeHumidity ?? DBNull.Value),
                    new NpgsqlParameter("@dewPoint", (object)request.DewPoint ?? DBNull.Value),
                    new NpgsqlParameter("@precipitation", (object)request.Precipitation ?? DBNull.Value),
                    new NpgsqlParameter("@cloudCover", (object)request.CloudCover ?? DBNull.Value),
                    new NpgsqlParameter("@windSpeed", (object)request.WindSpeed ?? DBNull.Value),
                    new NpgsqlParameter("@windDirection", (object)request.WindDirection ?? DBNull.Value),
                    new NpgsqlParameter("@groundTemperature", (object)request.GroundTemperature ?? DBNull.Value),
                    new NpgsqlParameter("@soilTemperature", (object)request.SoilTemperature ?? DBNull.Value),
                    new NpgsqlParameter("@conditionIds", (object)request.ConditionIds ?? DBNull.Value),
                    new NpgsqlParameter("@soilHumidity50cm", (object)request.SoilHumidity50cm ?? DBNull.Value),
                    new NpgsqlParameter("@soilHumidity2m", (object)request.SoilHumidity2m ?? DBNull.Value),
                    new NpgsqlParameter("@solarRadiation", (object)request.SolarRadiation ?? DBNull.Value)
                };

                var connection = EraDbContext.Database.GetDbConnection();
                if (connection.State == ConnectionState.Closed)
                {
                    await connection.OpenAsync(cancellationToken);
                }

                if (!(await DoesTableExistAsync((NpgsqlConnection)connection, tableName, cancellationToken)))
                {
                    await CreatePartitionForDateAsync((NpgsqlConnection)connection, modelName, DateOnly.FromDateTime(partitionDate), cancellationToken);
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