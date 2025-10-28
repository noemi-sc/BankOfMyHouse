using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankOfMyHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchasePriceToInvestment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PurchasePrice",
                table: "Investments",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m,
                comment: "Stock price per share at the time of purchase");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchasePrice",
                table: "Investments");
        }
    }
}
