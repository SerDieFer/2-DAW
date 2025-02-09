using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vestigio.Migrations
{
    /// <inheritdoc />
    public partial class resolutionFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChallengeResolution_Challenge_ChallengeId",
                table: "ChallengeResolution");

            migrationBuilder.AddForeignKey(
                name: "FK_ChallengeResolution_Challenge_ChallengeId",
                table: "ChallengeResolution",
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

            migrationBuilder.AddForeignKey(
                name: "FK_ChallengeResolution_Challenge_ChallengeId",
                table: "ChallengeResolution",
                column: "ChallengeId",
                principalTable: "Challenge",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
