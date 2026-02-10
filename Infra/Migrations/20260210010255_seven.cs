using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class seven : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoalTime",
                table: "MatchGoals");

            migrationBuilder.DropColumn(
                name: "IncidentTime",
                table: "MatchCards");

            migrationBuilder.AddColumn<int>(
                name: "Minute",
                table: "MatchGoals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "MatchGoals",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "StageType",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Venue",
                table: "Matches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Minute",
                table: "MatchCards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "MatchCards",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_MatchGoals_TeamId",
                table: "MatchGoals",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchCards_TeamId",
                table: "MatchCards",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchCards_Teams_TeamId",
                table: "MatchCards",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchGoals_Teams_TeamId",
                table: "MatchGoals",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchCards_Teams_TeamId",
                table: "MatchCards");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchGoals_Teams_TeamId",
                table: "MatchGoals");

            migrationBuilder.DropIndex(
                name: "IX_MatchGoals_TeamId",
                table: "MatchGoals");

            migrationBuilder.DropIndex(
                name: "IX_MatchCards_TeamId",
                table: "MatchCards");

            migrationBuilder.DropColumn(
                name: "Minute",
                table: "MatchGoals");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "MatchGoals");

            migrationBuilder.DropColumn(
                name: "StageType",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Venue",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Minute",
                table: "MatchCards");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "MatchCards");

            migrationBuilder.AddColumn<DateTime>(
                name: "GoalTime",
                table: "MatchGoals",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "IncidentTime",
                table: "MatchCards",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
