using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Ugm
{
    /// <inheritdoc />
    public partial class Init_Database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""UgmWeatherData"" (
                    ""Id"" uuid NOT NULL,
                    ""RegionId"" integer NOT NULL,
                    ""Date"" date,
                    ""DateTime"" timestamp with time zone,
                    ""DayPart"" integer,
                    ""Icon"" text,
                    ""AirTMin"" integer,
                    ""AirTMax"" integer,
                    ""WindDirection"" integer,
                    ""WindDirectionChange"" integer,
                    ""WindSpeedMin"" integer,
                    ""WindSpeedMax"" integer,
                    ""WindSpeedMinAfterChange"" integer,
                    ""WindSpeedMaxAfterChange"" integer,
                    ""CloudAmount"" text,
                    ""TimePeriod"" text,
                    ""Precipitation"" text,
                    ""IsOccasional"" integer,
                    ""IsPossible"" integer,
                    ""Thunderstorm"" integer,
                    ""Location"" text,
                    ""WeatherCode"" text,
                    CONSTRAINT ""PK_UgmWeatherData"" PRIMARY KEY (""Id"", ""Date"")
                ) PARTITION BY RANGE (""Date"");
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE ""UgmWeatherData""
                ADD CONSTRAINT ""uq_UgmWeatherData_date_station_dayPart""
                UNIQUE (""Date"", ""RegionId"", ""DayPart"");
            ");

            migrationBuilder.CreateTable(
                name: "UgmWeatherStations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UgmRegionId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    IsRegionalCenter = table.Column<bool>(type: "boolean", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UgmWeatherStations", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UgmWeatherData");

            migrationBuilder.DropTable(
                name: "UgmWeatherStations");
        }
    }
}
