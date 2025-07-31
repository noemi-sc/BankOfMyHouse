using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankOfMyHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedBlanaceToBanKAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "BankAccounts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "BankAccounts");
        }
    }
}
