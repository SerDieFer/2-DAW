using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vestigio.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Challenge_Product_ProductId",
                table: "Challenge");

            migrationBuilder.DropColumn(
                name: "Solution",
                table: "Challenge");

            migrationBuilder.RenameColumn(
                name: "Active",
                table: "Challenge",
                newName: "IsActive");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Product",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Challenge",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseDate",
                table: "Challenge",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SolutionMode",
                table: "Challenge",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Challenge_ProductLevel",
                table: "Challenge",
                column: "ProductLevel");

            migrationBuilder.AddForeignKey(
                name: "FK_Challenge_Product_ProductId",
                table: "Challenge",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Challenge_Product_ProductLevel",
                table: "Challenge",
                column: "ProductLevel",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Challenge_Product_ProductId",
                table: "Challenge");

            migrationBuilder.DropForeignKey(
                name: "FK_Challenge_Product_ProductLevel",
                table: "Challenge");

            migrationBuilder.DropIndex(
                name: "IX_Challenge_ProductLevel",
                table: "Challenge");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Challenge");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "Challenge");

            migrationBuilder.DropColumn(
                name: "SolutionMode",
                table: "Challenge");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Challenge",
                newName: "Active");

            migrationBuilder.AddColumn<string>(
                name: "Solution",
                table: "Challenge",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Challenge_Product_ProductId",
                table: "Challenge",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
