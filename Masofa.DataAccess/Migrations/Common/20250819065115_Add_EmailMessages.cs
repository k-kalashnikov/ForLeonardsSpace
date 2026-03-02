using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Common
{
    /// <inheritdoc />
    public partial class Add_EmailMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""EmailMessages"" (
                    ""Id"" uuid NOT NULL,
                    ""Sender"" text NOT NULL,
                    ""Recipients"" text[] NOT NULL,
                    ""CarbonCopy"" text[],
                    ""Body"" text,
                    ""Subject"" text,
                    ""CreateAt"" timestamp with time zone NOT NULL,
                    ""Status"" integer NOT NULL,
                    ""LastUpdateAt"" timestamp with time zone NOT NULL,
                    ""CreateUser"" uuid NOT NULL,
                    ""LastUpdateUser"" uuid NOT NULL,
                    CONSTRAINT ""PK_EmailMessages"" PRIMARY KEY (""Id"", ""CreateAt"")
                ) PARTITION BY RANGE (""CreateAt"");
            ");

            var today = DateTime.Today;

            var tableName = $"EmailMessages_{today:yyyy_MM_dd}";
            var startDate = today.ToString("yyyy-MM-dd");
            var endDate = today.AddDays(1).ToString("yyyy-MM-dd");

            var sql = $@"
            DO $$
            BEGIN
                IF NOT EXISTS (
                    SELECT FROM pg_tables
                    WHERE tablename = '{tableName}'
                ) THEN
                    EXECUTE '
                        CREATE TABLE ""{tableName}"" PARTITION OF ""EmailMessages""
                        FOR VALUES FROM (''{startDate}'') TO (''{endDate}'')
                    ';
                    RAISE NOTICE 'Partition created %', '{tableName}';
                END IF;
            END $$;";

            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailMessages");
        }
    }
}
