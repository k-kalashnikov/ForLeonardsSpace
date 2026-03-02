using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.CropMonitoring
{
    /// <inheritdoc />
    public partial class Update_Database_SoilData_1_6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "SoilDatas");

            migrationBuilder.Sql("CREATE TABLE \"SoilDatas\" " +
                "(\r\n    " +
                    "\"Id\" uuid NOT NULL,\r\n    " +
                    "\"Point\" geometry(Point, 4326) NOT NULL,  " +
                    "\r\n    " +
                    "\"Parameter\" text NOT NULL," +
                    "\r\n    " +
                    "\"DepthRange\" text NOT NULL,\r\n    " +
                    "\"Value\" double precision NULL,\r\n    " +
                    "\"Unit\" text NOT NULL,\r\n    " +
                    "\"Source\" text NOT NULL,\r\n    " +
                    "\"CreateAt\" timestamp with time zone NOT NULL,\r\n    " +
                    "\"LastUpdateAt\" timestamp with time zone NOT NULL,\r\n    " +
                    "\"Status\" integer NOT NULL,\r\n    " +
                    "\"CreateUser\" uuid NOT NULL,\r\n    " +
                    "\"LastUpdateUser\" uuid NOT NULL,\r\n    " +
                    "\"TileKey\" text NOT NULL,\r\n" +
                    "CONSTRAINT \"PK_SoilDatas\" PRIMARY KEY (\"Id\", \"TileKey\")\r\n" +
                ") PARTITION BY LIST (\"TileKey\");");

            var latitudes = GenerateRange(37.00, 45.5, 0.25);
            var longitudes = GenerateRange(56.00, 73.00, 0.25);

            foreach (var latitude in latitudes)
            {
                foreach (var longitude in longitudes)
                {
                    migrationBuilder.Sql($@"
                    CREATE TABLE ""SoilDatas_{longitude}_{latitude}"" PARTITION OF ""SoilDatas""
                        FOR VALUES IN ('{$"x_{longitude}_y_{latitude}"}');
                    ");
                }
            }
        }

        private static List<double> GenerateRange(double min, double max, double step)
        {
            var values = new List<double>();
            for (double value = min; value <= max; value = Math.Round(value, 2))
            {
                values.Add(Math.Round(value, 2));
                value += step;
            }
            return values;
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            var latitudes = GenerateRange(37.00, 45.5, 0.25);
            var longitudes = GenerateRange(56.00, 73.00, 0.25);

            foreach (var latitude in latitudes)
            {
                foreach (var longitude in longitudes)
                {
                    migrationBuilder.DropTable(name: $"SoilDatas_{longitude}_{latitude}");
                }
            }
        }
    }
}
