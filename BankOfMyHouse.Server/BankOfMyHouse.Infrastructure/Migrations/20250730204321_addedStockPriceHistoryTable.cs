using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankOfMyHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedStockPriceHistoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SharePrice",
                table: "Companies");

            migrationBuilder.CreateTable(
                name: "CompanyStockPrice",
                columns: table => new
                {
                    TimeOfPriceChange = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StockPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyStockPrice", x => x.TimeOfPriceChange);
                    table.ForeignKey(
                        name: "FK_CompanyStockPrice_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyStockPrice_CompanyId",
                table: "CompanyStockPrice",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyStockPrice");

            migrationBuilder.AddColumn<decimal>(
                name: "SharePrice",
                table: "Companies",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m,
                comment: "Current price per share");
        }
    }
}
