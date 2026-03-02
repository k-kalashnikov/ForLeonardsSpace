using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Masofa.DataAccess.Migrations.CropMonitoring
{
    /// <inheritdoc />
    public partial class AddFieldAddictionalInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FieldEncumbrances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EncumbranceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldEncumbrances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FieldFinancialConditions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    FinancialConditionId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Purpose = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldFinancialConditions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FieldInsuranceCases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PaymentAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    InsuranceCaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsInsuranceCase = table.Column<bool>(type: "boolean", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldInsuranceCases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FieldIrrigationDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnnualLevelPerArea = table.Column<double>(type: "double precision", nullable: true),
                    Requirement = table.Column<double>(type: "double precision", nullable: true),
                    Actual = table.Column<double>(type: "double precision", nullable: true),
                    WaterCharacteristics = table.Column<double>(type: "double precision", nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldIrrigationDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FieldSoilProfileAnalyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnalysisDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Coordinates = table.Column<Point>(type: "geometry", nullable: true),
                    AnalysisResults = table.Column<string>(type: "text", nullable: true),
                    FertileLayerDepth = table.Column<double>(type: "double precision", nullable: true),
                    Humus = table.Column<double>(type: "double precision", nullable: true),
                    Ph = table.Column<decimal>(type: "numeric", nullable: true),
                    SoilSalinity = table.Column<double>(type: "double precision", nullable: true),
                    ChlorideSalinity = table.Column<double>(type: "double precision", nullable: true),
                    SulfateSalinity = table.Column<double>(type: "double precision", nullable: true),
                    CarbonateSalinity = table.Column<double>(type: "double precision", nullable: true),
                    Macronutrients = table.Column<double>(type: "double precision", nullable: true),
                    Nitrogen = table.Column<double>(type: "double precision", nullable: true),
                    Phosphorus = table.Column<double>(type: "double precision", nullable: true),
                    Potassium = table.Column<double>(type: "double precision", nullable: true),
                    Sulfur = table.Column<double>(type: "double precision", nullable: true),
                    Micronutrients = table.Column<double>(type: "double precision", nullable: true),
                    SoilProfileAnalysisId = table.Column<Guid>(type: "uuid", nullable: true),
                    SoilClassification = table.Column<double>(type: "double precision", nullable: true),
                    BonitScore = table.Column<double>(type: "double precision", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldSoilProfileAnalyses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_field_encumbrance_dates",
                table: "FieldEncumbrances",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "idx_field_encumbrance_field_id",
                table: "FieldEncumbrances",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "idx_field_financial_condition_dates",
                table: "FieldFinancialConditions",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "idx_field_financial_condition_field_id",
                table: "FieldFinancialConditions",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "idx_field_insurance_case_field_id",
                table: "FieldInsuranceCases",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "idx_field_insurance_case_payment_date",
                table: "FieldInsuranceCases",
                column: "PaymentDate");

            migrationBuilder.CreateIndex(
                name: "idx_field_irrigation_data_field_id",
                table: "FieldIrrigationDatas",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "idx_field_irrigation_data_year",
                table: "FieldIrrigationDatas",
                column: "Year");

            migrationBuilder.CreateIndex(
                name: "idx_field_soil_profile_analysis_date",
                table: "FieldSoilProfileAnalyses",
                column: "AnalysisDate");

            migrationBuilder.CreateIndex(
                name: "idx_field_soil_profile_field_id",
                table: "FieldSoilProfileAnalyses",
                column: "FieldId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldEncumbrances");

            migrationBuilder.DropTable(
                name: "FieldFinancialConditions");

            migrationBuilder.DropTable(
                name: "FieldInsuranceCases");

            migrationBuilder.DropTable(
                name: "FieldIrrigationDatas");

            migrationBuilder.DropTable(
                name: "FieldSoilProfileAnalyses");
        }
    }
}
