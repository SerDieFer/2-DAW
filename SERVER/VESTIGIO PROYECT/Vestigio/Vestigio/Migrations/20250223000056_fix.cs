using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vestigio.Migrations
{
    /// <inheritdoc />
    public partial class fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Challenge_Product_ProductId",
                table: "Challenge");

            migrationBuilder.AddForeignKey(
                name: "FK_Challenge_Product_ProductId",
                table: "Challenge",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Challenge_Product_ProductId",
                table: "Challenge");

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
