using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Indices
{
    /// <inheritdoc />
    public partial class Update_Database_Repots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArviSeasonReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOnly = table.Column<DateOnly>(type: "date", nullable: false),
                    Average = table.Column<double>(type: "double precision", nullable: false),
                    TotalMax = table.Column<double>(type: "double precision", nullable: false),
                    TotalMin = table.Column<double>(type: "double precision", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArviSeasonReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArviSharedReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOnly = table.Column<DateOnly>(type: "date", nullable: false),
                    Average = table.Column<double>(type: "double precision", nullable: false),
                    TotalMax = table.Column<double>(type: "double precision", nullable: false),
                    TotalMin = table.Column<double>(type: "double precision", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CropId = table.Column<Guid>(type: "uuid", nullable: true),
                    AverageMax = table.Column<double>(type: "double precision", nullable: false),
                    AverageMin = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArviSharedReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EviSeasonReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOnly = table.Column<DateOnly>(type: "date", nullable: false),
                    Average = table.Column<double>(type: "double precision", nullable: false),
                    TotalMax = table.Column<double>(type: "double precision", nullable: false),
                    TotalMin = table.Column<double>(type: "double precision", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EviSeasonReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EviSharedReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOnly = table.Column<DateOnly>(type: "date", nullable: false),
                    Average = table.Column<double>(type: "double precision", nullable: false),
                    TotalMax = table.Column<double>(type: "double precision", nullable: false),
                    TotalMin = table.Column<double>(type: "double precision", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CropId = table.Column<Guid>(type: "uuid", nullable: true),
                    AverageMax = table.Column<double>(type: "double precision", nullable: false),
                    AverageMin = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EviSharedReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GndviSeasonReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOnly = table.Column<DateOnly>(type: "date", nullable: false),
                    Average = table.Column<double>(type: "double precision", nullable: false),
                    TotalMax = table.Column<double>(type: "double precision", nullable: false),
                    TotalMin = table.Column<double>(type: "double precision", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GndviSeasonReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GndviSharedReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOnly = table.Column<DateOnly>(type: "date", nullable: false),
                    Average = table.Column<double>(type: "double precision", nullable: false),
                    TotalMax = table.Column<double>(type: "double precision", nullable: false),
                    TotalMin = table.Column<double>(type: "double precision", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CropId = table.Column<Guid>(type: "uuid", nullable: true),
                    AverageMax = table.Column<double>(type: "double precision", nullable: false),
                    AverageMin = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GndviSharedReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MndwiSeasonReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOnly = table.Column<DateOnly>(type: "date", nullable: false),
                    Average = table.Column<double>(type: "double precision", nullable: false),
                    TotalMax = table.Column<double>(type: "double precision", nullable: false),
                    TotalMin = table.Column<double>(type: "double precision", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MndwiSeasonReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MndwiSharedReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOnly = table.Column<DateOnly>(type: "date", nullable: false),
                    Average = table.Column<double>(type: "double precision", nullable: false),
                    TotalMax = table.Column<double>(type: "double precision", nullable: false),
                    TotalMin = table.Column<double>(type: "double precision", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CropId = table.Column<Guid>(type: "uuid", nullable: true),
                    AverageMax = table.Column<double>(type: "double precision", nullable: false),
                    AverageMin = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MndwiSharedReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NdmiSeasonReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOnly = table.Column<DateOnly>(type: "date", nullable: false),
                    Average = table.Column<double>(type: "double precision", nullable: false),
                    TotalMax = table.Column<double>(type: "double precision", nullable: false),
                    TotalMin = table.Column<double>(type: "double precision", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdmiSeasonReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NdmiSharedReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOnly = table.Column<DateOnly>(type: "date", nullable: false),
                    Average = table.Column<double>(type: "double precision", nullable: false),
                    TotalMax = table.Column<double>(type: "double precision", nullable: false),
                    TotalMin = table.Column<double>(type: "double precision", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CropId = table.Column<Guid>(type: "uuid", nullable: true),
                    AverageMax = table.Column<double>(type: "double precision", nullable: false),
                    AverageMin = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdmiSharedReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NdviSeasonReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOnly = table.Column<DateOnly>(type: "date", nullable: false),
                    Average = table.Column<double>(type: "double precision", nullable: false),
                    TotalMax = table.Column<double>(type: "double precision", nullable: false),
                    TotalMin = table.Column<double>(type: "double precision", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdviSeasonReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NdviSharedReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOnly = table.Column<DateOnly>(type: "date", nullable: false),
                    Average = table.Column<double>(type: "double precision", nullable: false),
                    TotalMax = table.Column<double>(type: "double precision", nullable: false),
                    TotalMin = table.Column<double>(type: "double precision", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CropId = table.Column<Guid>(type: "uuid", nullable: true),
                    AverageMax = table.Column<double>(type: "double precision", nullable: false),
                    AverageMin = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdviSharedReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrviSeasonReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOnly = table.Column<DateOnly>(type: "date", nullable: false),
                    Average = table.Column<double>(type: "double precision", nullable: false),
                    TotalMax = table.Column<double>(type: "double precision", nullable: false),
                    TotalMin = table.Column<double>(type: "double precision", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrviSeasonReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrviSharedReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOnly = table.Column<DateOnly>(type: "date", nullable: false),
                    Average = table.Column<double>(type: "double precision", nullable: false),
                    TotalMax = table.Column<double>(type: "double precision", nullable: false),
                    TotalMin = table.Column<double>(type: "double precision", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CropId = table.Column<Guid>(type: "uuid", nullable: true),
                    AverageMax = table.Column<double>(type: "double precision", nullable: false),
                    AverageMin = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrviSharedReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OsaviSeasonReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOnly = table.Column<DateOnly>(type: "date", nullable: false),
                    Average = table.Column<double>(type: "double precision", nullable: false),
                    TotalMax = table.Column<double>(type: "double precision", nullable: false),
                    TotalMin = table.Column<double>(type: "double precision", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OsaviSeasonReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OsaviSharedReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOnly = table.Column<DateOnly>(type: "date", nullable: false),
                    Average = table.Column<double>(type: "double precision", nullable: false),
                    TotalMax = table.Column<double>(type: "double precision", nullable: false),
                    TotalMin = table.Column<double>(type: "double precision", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CropId = table.Column<Guid>(type: "uuid", nullable: true),
                    AverageMax = table.Column<double>(type: "double precision", nullable: false),
                    AverageMin = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OsaviSharedReports", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArviSeasonReports");

            migrationBuilder.DropTable(
                name: "ArviSharedReports");

            migrationBuilder.DropTable(
                name: "EviSeasonReports");

            migrationBuilder.DropTable(
                name: "EviSharedReports");

            migrationBuilder.DropTable(
                name: "GndviSeasonReports");

            migrationBuilder.DropTable(
                name: "GndviSharedReports");

            migrationBuilder.DropTable(
                name: "MndwiSeasonReports");

            migrationBuilder.DropTable(
                name: "MndwiSharedReports");

            migrationBuilder.DropTable(
                name: "NdmiSeasonReports");

            migrationBuilder.DropTable(
                name: "NdmiSharedReports");

            migrationBuilder.DropTable(
                name: "NdviSeasonReports");

            migrationBuilder.DropTable(
                name: "NdviSharedReports");

            migrationBuilder.DropTable(
                name: "OrviSeasonReports");

            migrationBuilder.DropTable(
                name: "OrviSharedReports");

            migrationBuilder.DropTable(
                name: "OsaviSeasonReports");

            migrationBuilder.DropTable(
                name: "OsaviSharedReports");
        }
    }
}
