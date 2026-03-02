using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Masofa.DataAccess.Migrations.Weather
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
                name: "AgroClimaticZoneMonthNorms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uuid", nullable: true),
                    AgroClimaticZoneId = table.Column<Guid>(type: "uuid", nullable: true),
                    M = table.Column<int>(type: "integer", nullable: true),
                    TemperatureAvgNorm = table.Column<double>(type: "double precision", nullable: true),
                    PrecipitationAvgNorm = table.Column<double>(type: "double precision", nullable: true),
                    SolarRadiationAvgNorm = table.Column<double>(type: "double precision", nullable: true),
                    TemperatureMedNorm = table.Column<double>(type: "double precision", nullable: true),
                    PrecipitationMedNorm = table.Column<double>(type: "double precision", nullable: true),
                    SolarRadiationMedNorm = table.Column<double>(type: "double precision", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgroClimaticZoneMonthNorms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgroClimaticZoneNorms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uuid", nullable: true),
                    AgroClimaticZoneId = table.Column<Guid>(type: "uuid", nullable: true),
                    M = table.Column<int>(type: "integer", nullable: true),
                    D = table.Column<int>(type: "integer", nullable: true),
                    TemperatureAvgNorm = table.Column<double>(type: "double precision", nullable: true),
                    PrecipitationAvgNorm = table.Column<double>(type: "double precision", nullable: true),
                    TemperatureMedNorm = table.Column<double>(type: "double precision", nullable: true),
                    PrecipitationMedNorm = table.Column<double>(type: "double precision", nullable: true),
                    SolarRadiationAvgNorm = table.Column<double>(type: "double precision", nullable: true),
                    SolarRadiationMedNorm = table.Column<double>(type: "double precision", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgroClimaticZoneNorms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgroClimaticZones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Polygon = table.Column<string>(type: "text", nullable: false),
                    PolygonGeom = table.Column<Geometry>(type: "geometry", nullable: true),
                    Active = table.Column<bool>(type: "boolean", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameUz = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgroClimaticZones", x => x.Id);
                });

            migrationBuilder.Sql("CREATE TABLE IF NOT EXISTS \"AgroClimaticZonesWeatherRates\" (" +
                "\"Id\" uuid NOT NULL, " +
                "\"Date\" timestamp with time zone NOT NULL, " +
                "\"TempRate\" double precision, " +
                "\"PrecRate\" double precision, " +
                "CONSTRAINT \"PK_AgroclimaticZonesWeatherRates\" PRIMARY KEY (\"Id\", \"Date\")" +
                ") PARTITION BY RANGE (\"Date\");");

            migrationBuilder.Sql("CREATE TABLE IF NOT EXISTS \"Alerts\" (" +
                "\"Id\" uuid NOT NULL, " +
                "\"RegionId\" uuid NOT NULL, " +
                "\"TypeId\" integer, " +
                "\"ProviderId\" uuid NOT NULL, " +
                "\"Date\" timestamp with time zone NOT NULL, " +
                "\"Value\" double precision, " +
                "\"AgroClimaticZonesId\" uuid, " +
                "CONSTRAINT \"PK_Alerts\" PRIMARY KEY (\"Id\", \"Date\")" +
                ") PARTITION BY RANGE (\"Date\");");

            migrationBuilder.CreateTable(
                name: "ApplicationPropertys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Application = table.Column<string>(type: "text", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationPropertys", x => x.Id);
                });

            migrationBuilder.Sql("CREATE TABLE IF NOT EXISTS \"Jobs\" (" +
                "\"Id\" uuid NOT NULL, " +
                "\"Date\" timestamp with time zone NOT NULL, " +
                "\"Action\" text, " +
                "\"ProviderId\" uuid, " +
                "\"JobStatusId\" uuid, " +
                "\"Application\" text, " +
                "\"Result\" text, " +
                "\"Path\" text, " +
                "CONSTRAINT \"PK_Jobs\" PRIMARY KEY (\"Id\", \"Date\")" +
                ") PARTITION BY RANGE (\"Date\");");

            migrationBuilder.Sql("CREATE TABLE IF NOT EXISTS \"Logs\" (" +
                "\"Id\" uuid NOT NULL, " +
                "\"Date\" timestamp with time zone NOT NULL, " +
                "\"JobId\" uuid, " +
                "\"JobStatus\" text, " +
                "\"ProviderId\" uuid, " +
                "\"Details\" text, " +
                "\"ContentSize\" double precision, " +
                "\"UserInfo\" text, " +
                "CONSTRAINT \"PK_Logs\" PRIMARY KEY (\"Id\", \"Date\")" +
                ") PARTITION BY RANGE (\"Date\");");

            migrationBuilder.Sql("CREATE TABLE IF NOT EXISTS \"Regions\" (" +
                "\"Id\" uuid NOT NULL, " +
                "\"RegionName\" text, " +
                "\"Iso\" text, " +
                "\"RegionLevel\" integer, " +
                "\"Lat\" double precision, " +
                "\"Lon\" double precision, " +
                "\"RowX\" integer, " +
                "\"ColumnY\" integer, " +
                "\"Polygon\" text NOT NULL, " +
                "\"PolygonGeom\" geometry, " +
                "\"ParentId\" uuid, " +
                "\"Active\" boolean, " +
                "\"RegionNameEn\" text, " +
                "\"RegionNameUz\" text, " +
                "\"Mhobt\" text, " +
                "\"UpdateDate\" timestamp with time zone NOT NULL, " +
                "CONSTRAINT \"PK_Regions\" PRIMARY KEY (\"Id\", \"UpdateDate\")" +
                ") PARTITION BY RANGE (\"UpdateDate\");");

            migrationBuilder.CreateTable(
                name: "RegionsAgroClimaticZones",
                columns: table => new
                {
                    RegionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AgroClimaticZonesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionsAgroClimaticZones", x => x.RegionId);
                });

            migrationBuilder.CreateTable(
                name: "RegionsDumps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionName = table.Column<string>(type: "text", nullable: true),
                    Iso = table.Column<string>(type: "text", nullable: true),
                    RegionLevel = table.Column<int>(type: "integer", nullable: true),
                    Lat = table.Column<double>(type: "double precision", nullable: true),
                    Lon = table.Column<double>(type: "double precision", nullable: true),
                    RowX = table.Column<int>(type: "integer", nullable: true),
                    ColumnY = table.Column<int>(type: "integer", nullable: true),
                    Polygon = table.Column<string>(type: "text", nullable: false),
                    PolygonGeom = table.Column<Geometry>(type: "geometry", nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Active = table.Column<bool>(type: "boolean", nullable: true),
                    Mhobt = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionsDumps", x => x.Id);
                });

            migrationBuilder.Sql("CREATE TABLE IF NOT EXISTS \"RegionsWeathers\" (" +
                "\"Id\" uuid NOT NULL, " +
                "\"RegionId\" uuid NOT NULL, " +
                "\"ProviderId\" uuid NOT NULL, " +
                "\"Date\" timestamp with time zone NOT NULL, " +
                "\"Temp\" double precision, " +
                "\"Precipitation\" double precision, " +
                "\"TempDev\" double precision, " +
                "\"PrecDev\" double precision, " +
                "CONSTRAINT \"PK_RegionsWeathers\" PRIMARY KEY (\"Id\", \"Date\")" +
                ") PARTITION BY RANGE (\"Date\");");

            migrationBuilder.Sql("CREATE TABLE IF NOT EXISTS \"Reports\" (" +
                "\"Id\" uuid NOT NULL, " +
                "\"ReportType\" uuid, " +
                "\"Link\" text, " +
                "\"Name\" text, " +
                "\"UpdateDate\" timestamp with time zone, " +
                "\"Description\" text, " +
                "\"SourceQuery\" text, " +
                "CONSTRAINT \"PK_Reports\" PRIMARY KEY (\"Id\", \"UpdateDate\")" +
                ") PARTITION BY RANGE (\"UpdateDate\");");

            migrationBuilder.CreateTable(
                name: "WeatherStationAgroClimaticZones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WeatherStationId = table.Column<Guid>(type: "uuid", nullable: false),
                    AgroClimaticZonesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherStationAgroClimaticZones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherStations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ProviderId = table.Column<Guid>(type: "uuid", nullable: true),
                    Lat = table.Column<double>(type: "double precision", nullable: true),
                    Lon = table.Column<double>(type: "double precision", nullable: true),
                    X = table.Column<int>(type: "integer", nullable: true),
                    Y = table.Column<int>(type: "integer", nullable: true),
                    Application = table.Column<string>(type: "text", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherStations", x => x.Id);
                });

            migrationBuilder.Sql("CREATE TABLE IF NOT EXISTS \"WeatherStationsDataEx\" (" +
                "\"Id\" uuid NOT NULL, " +
                "\"TemperatureSoil\" double precision, " +
                "\"TemperatureGroundLevel\" double precision, " +
                "\"Temperature1mAbove\" double precision, " +
                "\"Temperature2mUnder\" double precision, " +
                "\"HumiditySoil50cm\" double precision, " +
                "\"HumiditySoil2m\" double precision, " +
                "\"Temp10cmUnder\" double precision, " +
                "\"Temp30100cm\" double precision, " +
                "\"Temp1030cm\" double precision, " +
                "CONSTRAINT \"PK_WeatherStationsDataEx\" PRIMARY KEY (\"Id\")" +
                ");");

            migrationBuilder.Sql("CREATE TABLE IF NOT EXISTS \"WeatherStationsDatum\" (" +
                "\"Id\" uuid NOT NULL, " +
                "\"WeatherStationId\" uuid, " +
                "\"Date\" timestamp with time zone NOT NULL, " +
                "\"Temperature\" double precision, " +
                "\"TemperatureMax\" double precision, " +
                "\"TemperatureMin\" double precision, " +
                "\"Precipitation\" double precision, " +
                "\"WindSpeed\" double precision, " +
                "\"WindSpeedMin\" double precision, " +
                "\"WindSpeedMax\" double precision, " +
                "\"Windchill\" double precision, " +
                "\"CloudCover\" double precision, " +
                "\"RelativeHumidity\" double precision, " +
                "\"ConditionCode\" integer, " +
                "\"SolarRadiation\" double precision, " +
                "\"DewPoint\" double precision, " +
                "\"HumidityMin\" double precision, " +
                "\"HumidityMax\" double precision, " +
                "\"WindDirection\" double precision, " +
                "CONSTRAINT \"PK_WeatherStationsDatum\" PRIMARY KEY (\"Id\", \"Date\")" +
                ") PARTITION BY RANGE (\"Date\");");

            migrationBuilder.CreateTable(
                name: "XslsUzUnputColumns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    XlsColumnName = table.Column<string>(type: "text", nullable: true),
                    DbTableName = table.Column<string>(type: "text", nullable: true),
                    DbColumnName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XslsUzUnputColumns", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "AgroClimaticZoneMonthNorms");
            migrationBuilder.DropTable(name: "AgroClimaticZoneNorms");
            migrationBuilder.DropTable(name: "AgroClimaticZones");
            migrationBuilder.DropTable(name: "AgroclimaticZonesWeatherRates");
            migrationBuilder.DropTable(name: "Alerts");
            migrationBuilder.DropTable(name: "ApplicationPropertys");
            migrationBuilder.DropTable(name: "Jobs");
            migrationBuilder.DropTable(name: "Logs");
            migrationBuilder.DropTable(name: "Regions");
            migrationBuilder.DropTable(name: "RegionsAgroClimaticZones");
            migrationBuilder.DropTable(name: "RegionsDumps");
            migrationBuilder.DropTable(name: "RegionsWeathers");
            migrationBuilder.DropTable(name: "Reports");
            migrationBuilder.DropTable(name: "WeatherStationAgroClimaticZones");
            migrationBuilder.DropTable(name: "WeatherStations");
            migrationBuilder.DropTable(name: "WeatherStationsDataEx");
            migrationBuilder.DropTable(name: "WeatherStationsDatum");
            migrationBuilder.DropTable(name: "XslsUzUnputColumns");
        }
    }
}