using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BankOfMyHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedIdToCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Investments_Companies_CompanyName",
                table: "Investments");

            migrationBuilder.DropIndex(
                name: "IX_Investments_CompanyName",
                table: "Investments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Companies",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "Investments");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Investments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Companies",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Companies",
                table: "Companies",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Investments_CompanyId",
                table: "Investments",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Investments_Companies_CompanyId",
                table: "Investments",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Investments_Companies_CompanyId",
                table: "Investments");

            migrationBuilder.DropIndex(
                name: "IX_Investments_CompanyId",
                table: "Investments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Companies",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Investments");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Companies");

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "Investments",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Companies",
                table: "Companies",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Investments_CompanyName",
                table: "Investments",
                column: "CompanyName");

            migrationBuilder.AddForeignKey(
                name: "FK_Investments_Companies_CompanyName",
                table: "Investments",
                column: "CompanyName",
                principalTable: "Companies",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
