using Masofa.Common.Models.CropMonitoring;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Masofa.DataAccess.Migrations.CropMonitoring
{
    /// <inheritdoc />
    public partial class Update_Database_SoilData_1_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "SoilDatas");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""SoilDatas"" (
                    ""Id"" uuid NOT NULL,
                    ""Point"" geometry NOT NULL,
                    ""Parameter"" text NOT NULL,
                    ""DepthRange"" text NOT NULL,
                    ""Value"" double precision NULL,
                    ""Unit"" text NOT NULL,
                    ""Source"" text NOT NULL,
                    ""CreateAt"" timestamp with time zone NOT NULL,
                    ""LastUpdateAt"" timestamp with time zone NOT NULL,
                    ""Status"" integer NOT NULL,
                    ""CreateUser"" uuid NOT NULL,
                    ""LastUpdateUser"" uuid NOT NULL,
                    CONSTRAINT ""PK_SoilDatas"" PRIMARY KEY (""Id"", ""Parameter"")
                ) PARTITION BY RANGE (""Parameter"");
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "SoilDatas");

            migrationBuilder.CreateTable(
                name: "SoilDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Point = table.Column<Point>(type: "geometry", nullable: false),
                    Parameter = table.Column<string>(type: "text", nullable: false),
                    DepthRange = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: true),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    PointJson = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoilDatas", x => x.Id);
                });
        }
    }
}
