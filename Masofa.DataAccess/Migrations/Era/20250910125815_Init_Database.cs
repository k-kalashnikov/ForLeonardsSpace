using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Masofa.DataAccess.Migrations.Era
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
                CREATE TABLE IF NOT EXISTS ""EraWeatherData"" (
                    ""Id"" uuid NOT NULL,
                    ""OriginalDateTimeUtc"" timestamp with time zone,
                    ""Temperature"" double precision,
                    ""RelativeHumidity"" double precision,
                    ""DewPoint"" double precision,
                    ""Precipitation"" double precision,
                    ""CloudCover"" double precision,
                    ""WindSpeed"" double precision,
                    ""WindDirection"" double precision,
                    ""GroundTemperature"" double precision,
                    ""SoilTemperature"" double precision,
                    ""ConditionIds"" integer,
                    ""SoilHumidity50cm"" double precision,
                    ""SoilHumidity2m"" double precision,
                    ""EraWeatherStationId"" uuid NOT NULL,
                    CONSTRAINT ""PK_EraWeatherData"" PRIMARY KEY (""Id"", ""OriginalDateTimeUtc"")
                ) PARTITION BY RANGE (""OriginalDateTimeUtc"");
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE ""EraWeatherData""
                ADD CONSTRAINT ""uq_EraWeatherData_datetime_station""
                UNIQUE (""OriginalDateTimeUtc"", ""EraWeatherStationId"");
            ");

            migrationBuilder.CreateTable(
                name: "EraWeatherStations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Point = table.Column<Point>(type: "geometry", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EraWeatherStations", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EraWeatherData");

            migrationBuilder.DropTable(
                name: "EraWeatherStations");
        }
    }
}
