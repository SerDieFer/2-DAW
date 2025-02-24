using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vestigio.Migrations
{
    /// <inheritdoc />
    public partial class refix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChallengeResolution_Challenge_ChallengeId",
                table: "ChallengeResolution");

            migrationBuilder.DropForeignKey(
                name: "FK_Image_Challenge_ChallengeId",
                table: "Image");

            migrationBuilder.AddForeignKey(
                name: "FK_ChallengeResolution_Challenge_ChallengeId",
                table: "ChallengeResolution",
                column: "ChallengeId",
                principalTable: "Challenge",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Image_Challenge_ChallengeId",
                table: "Image",
                column: "ChallengeId",
                principalTable: "Challenge",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChallengeResolution_Challenge_ChallengeId",
                table: "ChallengeResolution");

            migrationBuilder.DropForeignKey(
                name: "FK_Image_Challenge_ChallengeId",
                table: "Image");

            migrationBuilder.AddForeignKey(
                name: "FK_ChallengeResolution_Challenge_ChallengeId",
                table: "ChallengeResolution",
                column: "ChallengeId",
                principalTable: "Challenge",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Image_Challenge_ChallengeId",
                table: "Image",
                column: "ChallengeId",
                principalTable: "Challenge",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
