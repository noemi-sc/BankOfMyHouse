using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BankOfMyHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentCategoryEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentCategory",
                table: "Transactions");

            migrationBuilder.AddColumn<int>(
                name: "PaymentCategoryId",
                table: "Transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                comment: "Foreign key to payment category");

            migrationBuilder.CreateTable(
                name: "payment_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_categories", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PaymentCategoryId",
                table: "Transactions",
                column: "PaymentCategoryId");

            migrationBuilder.CreateIndex(
                name: "ix_payment_categories_code",
                table: "payment_categories",
                column: "code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_payment_categories_PaymentCategoryId",
                table: "Transactions",
                column: "PaymentCategoryId",
                principalTable: "payment_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_payment_categories_PaymentCategoryId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "payment_categories");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_PaymentCategoryId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "PaymentCategoryId",
                table: "Transactions");

            migrationBuilder.AddColumn<string>(
                name: "PaymentCategory",
                table: "Transactions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
