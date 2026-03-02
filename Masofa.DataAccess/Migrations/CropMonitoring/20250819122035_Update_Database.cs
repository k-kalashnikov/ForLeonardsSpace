using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.CropMonitoring
{
    /// <inheritdoc />
    public partial class Update_Database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "BidNumberSequence");

            migrationBuilder.AlterColumn<long>(
                name: "Number",
                table: "Bids",
                type: "bigint",
                nullable: false,
                defaultValueSql: "nextval('\"BidNumberSequence\"')",
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "BidNumberSequence");

            migrationBuilder.AlterColumn<long>(
                name: "Number",
                table: "Bids",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValueSql: "nextval('\"BidNumberSequence\"')");
        }
    }
}
