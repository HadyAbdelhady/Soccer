using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddAssisterIdToMatchGoal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssisterId",
                table: "MatchGoals",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatchGoals_AssisterId",
                table: "MatchGoals",
                column: "AssisterId");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchGoals_Players_AssisterId",
                table: "MatchGoals",
                column: "AssisterId",
                principalTable: "Players",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchGoals_Players_AssisterId",
                table: "MatchGoals");

            migrationBuilder.DropIndex(
                name: "IX_MatchGoals_AssisterId",
                table: "MatchGoals");

            migrationBuilder.DropColumn(
                name: "AssisterId",
                table: "MatchGoals");
        }
    }
}
