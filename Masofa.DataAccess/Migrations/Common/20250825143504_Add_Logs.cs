using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Common
{
    /// <inheritdoc />
    public partial class Add_Logs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CallStacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateUserName = table.Column<string>(type: "text", nullable: false),
                    CreateUserFullName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CallStacks", x => x.Id);
                });

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""LogMessages"" (
                    ""Id"" uuid NOT NULL,
                    ""CreateAt"" timestamp with time zone NOT NULL,
                    ""Message"" text NOT NULL,
                    ""LogMessageType"" integer NOT NULL,
                    ""CallStackId"" uuid NOT NULL,
                    ""Path"" text NOT NULL,
                    ""Order"" integer NOT NULL,
                    CONSTRAINT ""PK_LogMessages"" PRIMARY KEY (""Id"", ""CreateAt"")
                ) PARTITION BY RANGE (""CreateAt"");
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CallStacks");

            migrationBuilder.DropTable(
                name: "LogMessages");
        }
    }
}
