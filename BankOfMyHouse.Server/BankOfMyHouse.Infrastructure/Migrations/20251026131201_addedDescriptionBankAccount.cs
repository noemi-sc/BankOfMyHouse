﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankOfMyHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedDescriptionBankAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "BankAccounts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "BankAccounts");
        }
    }
}
