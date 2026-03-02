using Masofa.Common.Models.IBMWeather;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Masofa.BusinessLogic.IBMWeather
{
    public class IbmWeatherDataUpsertCommand : IRequest<Guid>
    {
        [Required]
        public required IBMWeatherData Data { get; set; }
    }

    public class IbmWeatherDataUpsertCommandHandler : IRequestHandler<IbmWeatherDataUpsertCommand, Guid>
    {
        private ILogger Logger { get; set; }
        public MasofaIBMWeatherDbContext IBMWeatherDbContext { get; set; }

        public IbmWeatherDataUpsertCommandHandler(MasofaIBMWeatherDbContext iBMWeatherDbContext, ILogger<IbmWeatherDataUpsertCommandHandler> logger)
        {
            IBMWeatherDbContext = iBMWeatherDbContext;
            Logger = logger;
        }

        public async Task<Guid> Handle(IbmWeatherDataUpsertCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                var partitionDate = request.Data.ValidTimeUtc.Date;
                var modelName = "IBMWeatherData";
                var tableName = $"\"{modelName}_{partitionDate:yyyy_MM_dd}\"";

                var sql = $@"
                    INSERT INTO {tableName} (
                        ""Id"", ""IBMMeteoStationId"", ""ValidTimeUtc"", ""Temperature"", ""Humidity"", ""WindSpeed"",
                        ""WindDirection"", ""Precipitation"", ""UvIndex"", ""TemperatureMax"", ""TemperatureMin"",
                        ""DayOrNight"", ""PrecipChance"", ""Qpf"", ""QpfSnow"", ""RelativeHumidity"", ""DayOfWeek"",
                        ""RequestedLatitude"", ""RequestedLongitude"", ""GridpointId"", ""CreateAt"", ""Status"",
                        ""LastUpdateAt"", ""CreateUser"", ""LastUpdateUser""
                    )
                    VALUES (
                        @id, @iBMMeteoStationId, @validTimeUtc, @temperature, @humidity, @windSpeed, @windDirection,
                        @precipitation, @uvIndex, @temperatureMax, @temperatureMin, @dayOrNight, @precipChance, @qpf,
                        @qpfSnow, @relativeHumidity, @dayOfWeek, @requestedLatitude, @requestedLongitude, @gridpointId,
                        @createAt, @status, @lastUpdateAt, @createUser, @lastUpdateUser
                    )
                    ON CONFLICT (""ValidTimeUtc"", ""IBMMeteoStationId"", ""DayOrNight"")
                    DO UPDATE SET
                        ""Temperature"" = EXCLUDED.""Temperature"",
                        ""Humidity"" = EXCLUDED.""Humidity"",
                        ""WindSpeed"" = EXCLUDED.""WindSpeed"",
                        ""WindDirection"" = EXCLUDED.""WindDirection"",
                        ""Precipitation"" = EXCLUDED.""Precipitation"",
                        ""UvIndex"" = EXCLUDED.""UvIndex"",
                        ""TemperatureMax"" = EXCLUDED.""TemperatureMax"",
                        ""TemperatureMin"" = EXCLUDED.""TemperatureMin"",
                        ""DayOrNight"" = EXCLUDED.""DayOrNight"",
                        ""PrecipChance"" = EXCLUDED.""PrecipChance"",
                        ""Qpf"" = EXCLUDED.""Qpf"",
                        ""QpfSnow"" = EXCLUDED.""QpfSnow"",
                        ""RelativeHumidity"" = EXCLUDED.""RelativeHumidity"",
                        ""DayOfWeek"" = EXCLUDED.""DayOfWeek"",
                        ""RequestedLatitude"" = EXCLUDED.""RequestedLatitude"",
                        ""RequestedLongitude"" = EXCLUDED.""RequestedLongitude"",
                        ""GridpointId"" = EXCLUDED.""GridpointId"",
                        ""CreateAt"" = EXCLUDED.""CreateAt"", 
                        ""Status"" = EXCLUDED.""Status"", 
                        ""LastUpdateAt"" = EXCLUDED.""LastUpdateAt"", 
                        ""CreateUser"" = EXCLUDED.""CreateUser"", 
                        ""LastUpdateUser"" = EXCLUDED.""LastUpdateUser""
                    RETURNING ""Id""";

                var parameters = new[]
                {
                    new NpgsqlParameter("@id", Guid.NewGuid()),
                    new NpgsqlParameter("@validTimeUtc", request.Data.ValidTimeUtc),
                    new NpgsqlParameter("@iBMMeteoStationId", request.Data.IBMMeteoStationId),
                    new NpgsqlParameter("@temperature", (object)request.Data.Temperature ?? DBNull.Value),
                    new NpgsqlParameter("@humidity", (object)request.Data.Humidity ?? DBNull.Value),
                    new NpgsqlParameter("@windSpeed", (object)request.Data.WindSpeed ?? DBNull.Value),
                    new NpgsqlParameter("@windDirection", (object)request.Data.WindDirection ?? DBNull.Value),
                    new NpgsqlParameter("@precipitation", (object)request.Data.Precipitation ?? DBNull.Value),
                    new NpgsqlParameter("@uvIndex", (object)request.Data.UvIndex ?? DBNull.Value),
                    new NpgsqlParameter("@temperatureMax", (object)request.Data.TemperatureMax ?? DBNull.Value),
                    new NpgsqlParameter("@temperatureMin", (object)request.Data.TemperatureMin ?? DBNull.Value),
                    new NpgsqlParameter("@dayOrNight", (object)request.Data.DayOrNight ?? DBNull.Value),
                    new NpgsqlParameter("@precipChance", (object)request.Data.PrecipChance ?? DBNull.Value),
                    new NpgsqlParameter("@qpf", (object)request.Data.Qpf ?? DBNull.Value),
                    new NpgsqlParameter("@qpfSnow", (object)request.Data.QpfSnow ?? DBNull.Value),
                    new NpgsqlParameter("@relativeHumidity", (object)request.Data.RelativeHumidity ?? DBNull.Value),
                    new NpgsqlParameter("@dayOfWeek", (object)request.Data.DayOfWeek ?? DBNull.Value),
                    new NpgsqlParameter("@requestedLatitude", (object)request.Data.RequestedLatitude ?? DBNull.Value),
                    new NpgsqlParameter("@requestedLongitude", (object)request.Data.RequestedLongitude ?? DBNull.Value),
                    new NpgsqlParameter("@gridpointId", (object)request.Data.GridpointId ?? DBNull.Value),
                    new NpgsqlParameter("@createAt", (object)request.Data.CreateAt ?? DBNull.Value),
                    new NpgsqlParameter("@status", (int)request.Data.Status),
                    new NpgsqlParameter("@lastUpdateAt", (object)request.Data.LastUpdateAt ?? DBNull.Value),
                    new NpgsqlParameter("@createUser", (object)request.Data.CreateUser ?? DBNull.Value),
                    new NpgsqlParameter("@lastUpdateUser", (object)request.Data.LastUpdateUser ?? DBNull.Value)
                };

                var connection = IBMWeatherDbContext.Database.GetDbConnection();
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
