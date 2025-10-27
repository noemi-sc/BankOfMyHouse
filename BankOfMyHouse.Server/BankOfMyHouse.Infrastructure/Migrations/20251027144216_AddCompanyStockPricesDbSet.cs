using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankOfMyHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyStockPricesDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyStockPrice_Companies_CompanyId",
                table: "CompanyStockPrice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyStockPrice",
                table: "CompanyStockPrice");

            migrationBuilder.RenameTable(
                name: "CompanyStockPrice",
                newName: "CompanyStockPrices");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyStockPrice_CompanyId",
                table: "CompanyStockPrices",
                newName: "IX_CompanyStockPrices_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyStockPrices",
                table: "CompanyStockPrices",
                columns: new[] { "TimeOfPriceChange", "CompanyId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyStockPrices_Companies_CompanyId",
                table: "CompanyStockPrices",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyStockPrices_Companies_CompanyId",
                table: "CompanyStockPrices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyStockPrices",
                table: "CompanyStockPrices");

            migrationBuilder.RenameTable(
                name: "CompanyStockPrices",
                newName: "CompanyStockPrice");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyStockPrices_CompanyId",
                table: "CompanyStockPrice",
                newName: "IX_CompanyStockPrice_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyStockPrice",
                table: "CompanyStockPrice",
                columns: new[] { "TimeOfPriceChange", "CompanyId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyStockPrice_Companies_CompanyId",
                table: "CompanyStockPrice",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
