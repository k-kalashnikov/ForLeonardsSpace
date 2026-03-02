using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Common
{
    /// <inheritdoc />
    public partial class Add_UserTicket_UserTicketMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserTicketMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserTicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmailId = table.Column<Guid>(type: "uuid", nullable: true),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AttachmentIdsJson = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTicketMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserTickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateUserName = table.Column<string>(type: "text", nullable: true),
                    CreateUserEmail = table.Column<string>(type: "text", nullable: true),
                    ExceptionType = table.Column<string>(type: "text", nullable: false),
                    ExceptionJson = table.Column<string>(type: "text", nullable: false),
                    ModuleName = table.Column<string>(type: "text", nullable: false),
                    UserDeviceId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserDeviceType = table.Column<string>(type: "text", nullable: true),
                    UserDeviceJson = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTickets", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTicketMessages");

            migrationBuilder.DropTable(
                name: "UserTickets");
        }
    }
}
