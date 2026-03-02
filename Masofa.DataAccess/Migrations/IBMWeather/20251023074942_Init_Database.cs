using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Masofa.DataAccess.Migrations.IBMWeather
{
    /// <inheritdoc />
    public partial class Init_Database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "IBMMeteoStations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Point = table.Column<Point>(type: "geometry", nullable: false),
                    City = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    CountryCode = table.Column<string>(type: "text", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    AdminDistrict = table.Column<string>(type: "text", nullable: true),
                    AdminDistrictCode = table.Column<string>(type: "text", nullable: true),
                    IataCode = table.Column<string>(type: "text", nullable: true),
                    IcaoCode = table.Column<string>(type: "text", nullable: true),
                    PwsId = table.Column<string>(type: "text", nullable: true),
                    LocId = table.Column<string>(type: "text", nullable: true),
                    PlaceId = table.Column<string>(type: "text", nullable: true),
                    PostalKey = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Names = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IBMMeteoStations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IBMWeatherAlertFloodInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WeatherAlertId = table.Column<Guid>(type: "uuid", nullable: false),
                    FloodCrestTimeLocal = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FloodCrestTimeLocalTimeZone = table.Column<string>(type: "text", nullable: true),
                    FloodEndTimeLocal = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FloodEndTimeLocalTimeZone = table.Column<string>(type: "text", nullable: true),
                    FloodImmediateCause = table.Column<string>(type: "text", nullable: true),
                    FloodImmediateCauseCode = table.Column<string>(type: "text", nullable: true),
                    FloodLocationId = table.Column<string>(type: "text", nullable: true),
                    FloodLocationName = table.Column<string>(type: "text", nullable: true),
                    FloodRecordStatus = table.Column<string>(type: "text", nullable: true),
                    FloodRecordStatusCode = table.Column<string>(type: "text", nullable: true),
                    FloodSeverity = table.Column<string>(type: "text", nullable: true),
                    FloodSeverityCode = table.Column<string>(type: "text", nullable: true),
                    FloodStartTimeLocal = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FloodStartTimeLocalTimeZone = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IBMWeatherAlertFloodInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IBMWeatherAlerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IBMMeteoStationId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminDistrict = table.Column<string>(type: "text", nullable: true),
                    AdminDistrictCode = table.Column<string>(type: "text", nullable: true),
                    AreaId = table.Column<string>(type: "text", nullable: false),
                    AreaName = table.Column<string>(type: "text", nullable: false),
                    AreaTypeCode = table.Column<string>(type: "text", nullable: false),
                    Certainty = table.Column<string>(type: "text", nullable: false),
                    CertaintyCode = table.Column<string>(type: "text", nullable: false),
                    CountryCode = table.Column<string>(type: "text", nullable: false),
                    CountryName = table.Column<string>(type: "text", nullable: false),
                    DetailKey = table.Column<string>(type: "text", nullable: false),
                    Disclaimer = table.Column<string>(type: "text", nullable: true),
                    DisplayRank = table.Column<int>(type: "integer", nullable: false),
                    EffectiveTimeLocal = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EffectiveTimeLocalTimeZone = table.Column<string>(type: "text", nullable: true),
                    EventDescription = table.Column<string>(type: "text", nullable: false),
                    EventTrackingNumber = table.Column<string>(type: "text", nullable: false),
                    ExpireTimeLocal = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpireTimeLocalTimeZone = table.Column<string>(type: "text", nullable: false),
                    ExpireTimeUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HeadlineText = table.Column<string>(type: "text", nullable: false),
                    IanaTimeZone = table.Column<string>(type: "text", nullable: true),
                    Identifier = table.Column<string>(type: "text", nullable: false),
                    IssueTimeLocal = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IssueTimeLocalTimeZone = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    MessageType = table.Column<string>(type: "text", nullable: false),
                    MessageTypeCode = table.Column<int>(type: "integer", nullable: false),
                    OfficeAdminDistrict = table.Column<string>(type: "text", nullable: true),
                    OfficeAdminDistrictCode = table.Column<string>(type: "text", nullable: true),
                    OfficeCode = table.Column<string>(type: "text", nullable: false),
                    OfficeCountryCode = table.Column<string>(type: "text", nullable: true),
                    OfficeName = table.Column<string>(type: "text", nullable: false),
                    OnsetTimeLocal = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OnsetTimeLocalTimeZone = table.Column<string>(type: "text", nullable: true),
                    Phenomena = table.Column<string>(type: "text", nullable: false),
                    ProcessTimeUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductIdentifier = table.Column<string>(type: "text", nullable: false),
                    Severity = table.Column<string>(type: "text", nullable: false),
                    SeverityCode = table.Column<int>(type: "integer", nullable: false),
                    Significance = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    Urgency = table.Column<string>(type: "text", nullable: false),
                    UrgencyCode = table.Column<int>(type: "integer", nullable: false),
                    EndTimeLocal = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndTimeLocalTimeZone = table.Column<string>(type: "text", nullable: true),
                    EndTimeUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WeatherAlertResponseType = table.Column<int>(type: "integer", nullable: false),
                    WeatherAlertCategory = table.Column<int>(type: "integer", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Names = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IBMWeatherAlerts", x => x.Id);
                });

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""IBMWeatherData"" (
                    ""Id"" uuid NOT NULL,
                    ""IBMMeteoStationId"" uuid NOT NULL,
                    ""ValidTimeUtc"" timestamp with time zone NOT NULL,
                    ""Temperature"" integer,
                    ""Humidity"" integer,
                    ""WindSpeed"" integer,
                    ""WindDirection"" integer,
                    ""Precipitation"" double precision,
                    ""UvIndex"" integer,
                    ""TemperatureMax"" integer,
                    ""TemperatureMin"" integer,
                    ""DayOrNight"" text,
                    ""PrecipChance"" integer,
                    ""Qpf"" double precision,
                    ""QpfSnow"" double precision,
                    ""RelativeHumidity"" integer,
                    ""DayOfWeek"" text,
                    ""RequestedLatitude"" double precision,
                    ""RequestedLongitude"" double precision,
                    ""GridpointId"" text,
                    ""CreateAt"" timestamp with time zone NOT NULL,
                    ""Status"" integer NOT NULL,
                    ""LastUpdateAt"" timestamp with time zone NOT NULL,
                    ""CreateUser"" uuid NOT NULL,
                    ""LastUpdateUser"" uuid NOT NULL,
                    CONSTRAINT ""PK_IBMWeatherData"" PRIMARY KEY (""Id"", ""ValidTimeUtc"")
                ) PARTITION BY RANGE (""ValidTimeUtc"");
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE ""IBMWeatherData""
                ADD CONSTRAINT ""uq_IBMWeatherData_datetime_station_dayPart""
                UNIQUE (""ValidTimeUtc"", ""IBMMeteoStationId"", ""DayOrNight"");
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IBMMeteoStations");

            migrationBuilder.DropTable(
                name: "IBMWeatherAlertFloodInfos");

            migrationBuilder.DropTable(
                name: "IBMWeatherAlerts");

            migrationBuilder.DropTable(
                name: "IBMWeatherData");
        }
    }
}
