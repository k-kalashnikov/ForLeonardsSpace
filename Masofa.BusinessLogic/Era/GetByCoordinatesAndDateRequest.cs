using Masofa.Common.Models.Era;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Masofa.BusinessLogic.Era
{
    public class GetByCoordinatesAndDateRequest : IRequest<Era5HourWeatherForecast>
    {
        /// <summary>
        /// Широта
        /// </summary>
        [Required]
        public required double Latitude { get; set; }

        /// <summary>
        /// Долгота
        /// </summary>
        [Required]
        public required double Longitude { get; set; }

        /// <summary>
        /// Время
        /// </summary>
        [Required]
        public required DateTime InputDate { get; set; }
    }

    public class GetByCoordinatesAndDateRequestHandler : IRequestHandler<GetByCoordinatesAndDateRequest, Era5HourWeatherForecast>
    {
        public GetByCoordinatesAndDateRequestHandler(MasofaEraDbContext eraDbContext)
        {
            EraDbContext = eraDbContext;
        }

        public MasofaEraDbContext EraDbContext { get; set; }

        public async Task<Era5HourWeatherForecast> Handle(GetByCoordinatesAndDateRequest request, CancellationToken cancellationToken)
        {

            var targetPoint = new Point(request.Longitude, request.Latitude)
            {
                SRID = 4326
            };

            var targetDateTime = new DateTime(request.InputDate.Year, request.InputDate.Month, request.InputDate.Day, request.InputDate.Hour, request.InputDate.Minute, request.InputDate.Second, DateTimeKind.Utc);

            var closestStation = await EraDbContext.EraWeatherStations
                .OrderBy(s => s.Point.Distance(targetPoint))
                .FirstOrDefaultAsync();

            if (closestStation == null) 
            {
                return null;
            }

            var partitionDate = request.InputDate.ToString("yyyy_MM_dd");
            var modelName = "EraWeatherData";
            var tableName = $"\"{modelName}_{partitionDate}\"";

            var sqlSelect = $"SELECT\n\r" +
                        $"\"Id\",\n\r" +
                        $"\"OriginalDateTimeUtc\",\n\r" +
                        $"\"EraWeatherStationId\",\n\r" +
                        $"\"Temperature\",\n\r" +
                        $"\"RelativeHumidity\",\n\r" +
                        $"\"DewPoint\",\n\r" +
                        $"\"Precipitation\",\n\r" +
                        $"\"CloudCover\",\n\r" +
                        $"\"WindSpeed\",\n\r" +
                        $"\"WindDirection\",\n\r" +
                        $"\"GroundTemperature\",\n\r" +
                        $"\"SoilTemperature\",\n\r" +
                        $"\"ConditionIds\",\n\r" +
                        $"\"SoilHumidity50cm\",\n\r" +
                        $"\"SoilHumidity2m\",\n\r" +
                        $"\"SolarRadiation\"\n\r" +
                    $"FROM {tableName}\n\r" +
                    $"WHERE\n\r" +
                    $"\"EraWeatherStationId\" = '{closestStation.Id}';";

            var command = new NpgsqlCommand(sqlSelect, (NpgsqlConnection)EraDbContext.Database.GetDbConnection())
            {
                CommandTimeout = 60
            };
            var scalar = (IEnumerable<Masofa.Common.Models.Era.EraWeatherData>)(await command.ExecuteScalarAsync());
            if ((scalar == null) || (!scalar.Any()))
            {
                return null;
            }
            scalar = scalar.Where(x => x.OriginalDateTimeUtc.HasValue).ToList();
            var result = scalar.OrderBy(x => Math.Abs((x.OriginalDateTimeUtc.Value - targetDateTime).TotalSeconds))
                .FirstOrDefault();

            if (result == null)
            {
                return null;
            }

            return new Era5HourWeatherForecast()
            {
                Fallout = result.Precipitation ?? 0,
                Date = new DateOnly(targetDateTime.Year, targetDateTime.Month, targetDateTime.Day),
                Id = result.Id,
                SolarRadiationInfluence = result.SolarRadiation ?? 0,
                Hour = targetDateTime.Hour,
                Humidity = result.RelativeHumidity ?? 0,
                TemperatureMax = result.Temperature ?? 0,
                TemperatureMaxTotal = result.Temperature ?? 0,
                TemperatureMin = result.Temperature ?? 0,
                TemperatureMinTotal = result.Temperature ?? 0,
                WeatherStation = closestStation.Id,
                WindDerection = result.WindDirection ?? 0,
                WindSpeed = result.WindSpeed ?? 0
            };
        }
    }
}
