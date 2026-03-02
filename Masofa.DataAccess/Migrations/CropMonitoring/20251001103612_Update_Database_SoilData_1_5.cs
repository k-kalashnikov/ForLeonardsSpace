using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.CropMonitoring
{
    /// <inheritdoc />
    public partial class Update_Database_SoilData_1_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "SoilDatas");

            migrationBuilder.Sql(@"
                CREATE TABLE ""SoilDatas"" (
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
                ) PARTITION BY LIST (""Parameter"");
            ");

            var parameters = new[]
            {
                "sand", "silt", "clay", "phh2o", "cec", "soc",
                "bdod", "cfvo", "nitrogen", "humus", "phosphorus", "salinity"
            };

            foreach (var param in parameters)
            {
                migrationBuilder.Sql($@"
                    CREATE TABLE ""SoilDatas_{param}"" PARTITION OF ""SoilDatas""
                        FOR VALUES IN ('{param}');
                ");
            }
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
                    Point = table.Column<NetTopologySuite.Geometries.Point>(type: "geometry", nullable: false),
                    Parameter = table.Column<string>(type: "text", nullable: false),
                    DepthRange = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: true),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
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
