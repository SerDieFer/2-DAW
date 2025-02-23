using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vestigio.Migrations
{
    /// <inheritdoc />
    public partial class decimals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Challenge_Product_ProductId",
                table: "Challenge");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSize_Product_ProductId",
                table: "ProductSize");

            migrationBuilder.AddColumn<string>(
                name: "PriceInput",
                table: "Product",
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

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSize_Product_ProductId",
                table: "ProductSize",
                column: "ProductId",
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
                name: "FK_ProductSize_Product_ProductId",
                table: "ProductSize");

            migrationBuilder.DropColumn(
                name: "PriceInput",
                table: "Product");

            migrationBuilder.AddForeignKey(
                name: "FK_Challenge_Product_ProductId",
                table: "Challenge",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSize_Product_ProductId",
                table: "ProductSize",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
