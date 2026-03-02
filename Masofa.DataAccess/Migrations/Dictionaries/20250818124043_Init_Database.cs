using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Dictionaries
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
                name: "AdministrativeUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdministrativeUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgroclimaticZones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgroclimaticZones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgroMachineTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsSoilSafe = table.Column<bool>(type: "boolean", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgroMachineTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgroOperations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgroOperations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgrotechnicalMeasures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CropId = table.Column<Guid>(type: "uuid", nullable: true),
                    VarietyId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgrotechnicalMeasures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgroTerms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DescrEn = table.Column<string>(type: "text", nullable: true),
                    DescrRu = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgroTerms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BidContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BidContents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BidStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BidStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BidTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BidTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClimaticStandards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Month = table.Column<int>(type: "integer", nullable: true),
                    Day = table.Column<int>(type: "integer", nullable: true),
                    TempAvg = table.Column<decimal>(type: "numeric", nullable: true),
                    TempMin = table.Column<decimal>(type: "numeric", nullable: true),
                    TempMax = table.Column<decimal>(type: "numeric", nullable: true),
                    PrecDayAvg = table.Column<decimal>(type: "numeric", nullable: true),
                    RadDayAvg = table.Column<decimal>(type: "numeric", nullable: true),
                    HumAvg = table.Column<decimal>(type: "numeric", nullable: true),
                    CoefSel = table.Column<decimal>(type: "numeric", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClimaticStandards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CropPeriods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CropId = table.Column<Guid>(type: "uuid", nullable: true),
                    VarietyId = table.Column<Guid>(type: "uuid", nullable: true),
                    DayStart = table.Column<int>(type: "integer", nullable: true),
                    DayEnd = table.Column<int>(type: "integer", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CropPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CropPeriodVegetationIndexes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CropPeriodId = table.Column<Guid>(type: "uuid", nullable: false),
                    VegetationIndexId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<decimal>(type: "numeric", nullable: true),
                    Min = table.Column<decimal>(type: "numeric", nullable: true),
                    Max = table.Column<decimal>(type: "numeric", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CropPeriodVegetationIndexes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Crops",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameLa = table.Column<string>(type: "text", nullable: true),
                    IsMonitoring = table.Column<bool>(type: "boolean", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crops", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DicitonaryTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DicitonaryTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Diseases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CropId = table.Column<Guid>(type: "uuid", nullable: true),
                    VarietyId = table.Column<Guid>(type: "uuid", nullable: true),
                    NameLa = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diseases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntomophageTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameLa = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntomophageTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExperimentalFarmingMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CropId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperimentalFarmingMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fertilizers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsEco = table.Column<bool>(type: "boolean", nullable: true),
                    IsOrganic = table.Column<bool>(type: "boolean", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fertilizers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FertilizerTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FertilizerTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FieldUsageStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldUsageStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Firms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Inn = table.Column<string>(type: "text", nullable: true),
                    Egrpo = table.Column<string>(type: "text", nullable: true),
                    Site = table.Column<string>(type: "text", nullable: true),
                    Phones = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Chief = table.Column<string>(type: "text", nullable: true),
                    MainRegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    PhysicalAddress = table.Column<string>(type: "text", nullable: true),
                    MailingAddress = table.Column<string>(type: "text", nullable: true),
                    ShortName = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    InternationalName = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Firms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlightTargets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightTargets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IrrigationMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsWaterSafe = table.Column<bool>(type: "boolean", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IrrigationMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IrrigationSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IrrigationSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeasurementUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullNameEn = table.Column<string>(type: "text", nullable: true),
                    FullNameRu = table.Column<string>(type: "text", nullable: true),
                    SiUnit = table.Column<string>(type: "text", nullable: true),
                    Factor = table.Column<decimal>(type: "numeric", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeliorativeMeasureTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeliorativeMeasureTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Pinfl = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    Telegram = table.Column<string>(type: "text", nullable: true),
                    Phones = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    MainRegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    PhysicalAddress = table.Column<string>(type: "text", nullable: true),
                    MailingAddress = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pesticides",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PesticideTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    IntCode = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pesticides", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PesticideTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IntCode = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PesticideTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PestTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameLa = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PestTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductQualityStandards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductQualityStandards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProviderWeatherConditions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Lat = table.Column<decimal>(type: "numeric", nullable: true),
                    Lng = table.Column<decimal>(type: "numeric", nullable: true),
                    Radius = table.Column<decimal>(type: "numeric", nullable: true),
                    WeatherStationTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderWeatherConditions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegionMaps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Lat = table.Column<decimal>(type: "numeric", nullable: true),
                    Lng = table.Column<decimal>(type: "numeric", nullable: true),
                    MozaikX = table.Column<int>(type: "integer", nullable: true),
                    MozaikY = table.Column<int>(type: "integer", nullable: true),
                    PolygonJson = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionMaps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: true),
                    NameMhobt = table.Column<string>(type: "text", nullable: true),
                    ShortNameEn = table.Column<string>(type: "text", nullable: true),
                    ShortNameRu = table.Column<string>(type: "text", nullable: true),
                    ActiveFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActiveTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RegionSquare = table.Column<decimal>(type: "numeric", nullable: true),
                    NameAdminUz = table.Column<string>(type: "text", nullable: true),
                    NameAdminEn = table.Column<string>(type: "text", nullable: true),
                    NameAdminRu = table.Column<string>(type: "text", nullable: true),
                    Population = table.Column<decimal>(type: "numeric", nullable: true),
                    AgroclimaticZoneId = table.Column<Guid>(type: "uuid", nullable: true),
                    RegionMapId = table.Column<Guid>(type: "uuid", nullable: true),
                    RegionTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegionTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SoilTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoilTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SolarRadiationInfluences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CropId = table.Column<Guid>(type: "uuid", nullable: true),
                    VarietyId = table.Column<Guid>(type: "uuid", nullable: true),
                    DayStart = table.Column<int>(type: "integer", nullable: true),
                    DayEnd = table.Column<int>(type: "integer", nullable: true),
                    RadNorm = table.Column<decimal>(type: "numeric", nullable: true),
                    VegetationPeriodId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolarRadiationInfluences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemDataSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemDataSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerTypeFullName = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UavCameraTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UavCameraTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UavDataTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UavDataTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Varieties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CropId = table.Column<Guid>(type: "uuid", nullable: false),
                    NameLa = table.Column<string>(type: "text", nullable: true),
                    RipeningPeriod = table.Column<int>(type: "integer", nullable: true),
                    AverageYield = table.Column<decimal>(type: "numeric", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Varieties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VarietyFeatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VarietyFeatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VegetationIndexes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    DescriptionEn = table.Column<string>(type: "text", nullable: true),
                    DescriptionRu = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VegetationIndexes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VegetationPeriods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    VarietyId = table.Column<Guid>(type: "uuid", nullable: true),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: true),
                    DayStart = table.Column<int>(type: "integer", nullable: true),
                    DayEnd = table.Column<int>(type: "integer", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VegetationPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WaterResources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherAlertTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: true),
                    Value = table.Column<double>(type: "double precision", nullable: true),
                    DescriptionEn = table.Column<string>(type: "text", nullable: true),
                    DescriptionRu = table.Column<string>(type: "text", nullable: true),
                    FieldName = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    NameRu = table.Column<string>(type: "text", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherAlertTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherConditions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProviderCode = table.Column<int>(type: "integer", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    NameRu = table.Column<string>(type: "text", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherConditions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherFrequencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    NameRu = table.Column<string>(type: "text", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherFrequencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherImageTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<int>(type: "integer", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    NameRu = table.Column<string>(type: "text", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherImageTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherJobStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    NameRu = table.Column<string>(type: "text", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherJobStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherProviders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Z = table.Column<double>(type: "double precision", nullable: true),
                    FrequencyId = table.Column<int>(type: "integer", nullable: true),
                    Editable = table.Column<bool>(type: "boolean", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    NameRu = table.Column<string>(type: "text", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherReportTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Css = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    NameRu = table.Column<string>(type: "text", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherReportTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherStations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirmId = table.Column<Guid>(type: "uuid", nullable: true),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsAuto = table.Column<bool>(type: "boolean", nullable: true),
                    Lat = table.Column<decimal>(type: "numeric", nullable: true),
                    Lng = table.Column<decimal>(type: "numeric", nullable: true),
                    Radius = table.Column<decimal>(type: "numeric", nullable: true),
                    WeatherStationTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherStations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherStationTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameRu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherStationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<int>(type: "integer", nullable: true),
                    Gis = table.Column<bool>(type: "boolean", nullable: false),
                    TiledMap = table.Column<bool>(type: "boolean", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    NameRu = table.Column<string>(type: "text", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherTypes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdministrativeUnits");

            migrationBuilder.DropTable(
                name: "AgroclimaticZones");

            migrationBuilder.DropTable(
                name: "AgroMachineTypes");

            migrationBuilder.DropTable(
                name: "AgroOperations");

            migrationBuilder.DropTable(
                name: "AgrotechnicalMeasures");

            migrationBuilder.DropTable(
                name: "AgroTerms");

            migrationBuilder.DropTable(
                name: "BidContents");

            migrationBuilder.DropTable(
                name: "BidStates");

            migrationBuilder.DropTable(
                name: "BidTypes");

            migrationBuilder.DropTable(
                name: "BusinessTypes");

            migrationBuilder.DropTable(
                name: "ClimaticStandards");

            migrationBuilder.DropTable(
                name: "CropPeriods");

            migrationBuilder.DropTable(
                name: "CropPeriodVegetationIndexes");

            migrationBuilder.DropTable(
                name: "Crops");

            migrationBuilder.DropTable(
                name: "DicitonaryTypes");

            migrationBuilder.DropTable(
                name: "Diseases");

            migrationBuilder.DropTable(
                name: "EntomophageTypes");

            migrationBuilder.DropTable(
                name: "ExperimentalFarmingMethods");

            migrationBuilder.DropTable(
                name: "Fertilizers");

            migrationBuilder.DropTable(
                name: "FertilizerTypes");

            migrationBuilder.DropTable(
                name: "FieldUsageStatuses");

            migrationBuilder.DropTable(
                name: "Firms");

            migrationBuilder.DropTable(
                name: "FlightTargets");

            migrationBuilder.DropTable(
                name: "IrrigationMethods");

            migrationBuilder.DropTable(
                name: "IrrigationSources");

            migrationBuilder.DropTable(
                name: "MeasurementUnits");

            migrationBuilder.DropTable(
                name: "MeliorativeMeasureTypes");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Pesticides");

            migrationBuilder.DropTable(
                name: "PesticideTypes");

            migrationBuilder.DropTable(
                name: "PestTypes");

            migrationBuilder.DropTable(
                name: "ProductQualityStandards");

            migrationBuilder.DropTable(
                name: "ProviderWeatherConditions");

            migrationBuilder.DropTable(
                name: "RegionMaps");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropTable(
                name: "RegionTypes");

            migrationBuilder.DropTable(
                name: "SoilTypes");

            migrationBuilder.DropTable(
                name: "SolarRadiationInfluences");

            migrationBuilder.DropTable(
                name: "SystemDataSources");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "TaskStatuses");

            migrationBuilder.DropTable(
                name: "UavCameraTypes");

            migrationBuilder.DropTable(
                name: "UavDataTypes");

            migrationBuilder.DropTable(
                name: "Varieties");

            migrationBuilder.DropTable(
                name: "VarietyFeatures");

            migrationBuilder.DropTable(
                name: "VegetationIndexes");

            migrationBuilder.DropTable(
                name: "VegetationPeriods");

            migrationBuilder.DropTable(
                name: "WaterResources");

            migrationBuilder.DropTable(
                name: "WeatherAlertTypes");

            migrationBuilder.DropTable(
                name: "WeatherConditions");

            migrationBuilder.DropTable(
                name: "WeatherFrequencies");

            migrationBuilder.DropTable(
                name: "WeatherImageTypes");

            migrationBuilder.DropTable(
                name: "WeatherJobStatuses");

            migrationBuilder.DropTable(
                name: "WeatherProviders");

            migrationBuilder.DropTable(
                name: "WeatherReportTypes");

            migrationBuilder.DropTable(
                name: "WeatherStations");

            migrationBuilder.DropTable(
                name: "WeatherStationTypes");

            migrationBuilder.DropTable(
                name: "WeatherTypes");
        }
    }
}
