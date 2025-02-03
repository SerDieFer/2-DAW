using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vestigio.Migrations
{
    /// <inheritdoc />
    public partial class unlocks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserUnlockedProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    UnlockedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserUnlockedProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserUnlockedProduct_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserUnlockedProduct_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserUnlockedProductByLevel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    UnlockedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserUnlockedProductByLevel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserUnlockedProductByLevel_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserUnlockedProduct_ProductId",
                table: "UserUnlockedProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_UserUnlockedProduct_UserId",
                table: "UserUnlockedProduct",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserUnlockedProductByLevel_UserId",
                table: "UserUnlockedProductByLevel",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserUnlockedProduct");

            migrationBuilder.DropTable(
                name: "UserUnlockedProductByLevel");
        }
    }
}
