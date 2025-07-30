using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankOfMyHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changedKeyCompanyStockPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyStockPrice",
                table: "CompanyStockPrice");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyStockPrice",
                table: "CompanyStockPrice",
                columns: new[] { "TimeOfPriceChange", "CompanyId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyStockPrice",
                table: "CompanyStockPrice");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyStockPrice",
                table: "CompanyStockPrice",
                column: "TimeOfPriceChange");
        }
    }
}
