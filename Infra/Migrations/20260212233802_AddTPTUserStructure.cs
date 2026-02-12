using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddTPTUserStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchCards_Teams_TeamId",
                table: "MatchCards");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Teams_AwayTeamId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Teams_HomeTeamId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchGoals_Teams_TeamId",
                table: "MatchGoals");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchLineups_Teams_TeamId",
                table: "MatchLineups");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Teams_TeamId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamTournament_Teams_TeamsId",
                table: "TeamTournament");

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "Tournaments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId1",
                table: "Players",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "Matches",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId1",
                table: "Matches",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BaseUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HashedPassword = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminUsers_BaseUsers_Id",
                        column: x => x.Id,
                        principalTable: "BaseUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamUsers_BaseUsers_Id",
                        column: x => x.Id,
                        principalTable: "BaseUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamUsers_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WatcherUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatcherUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WatcherUsers_BaseUsers_Id",
                        column: x => x.Id,
                        principalTable: "BaseUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_TeamId",
                table: "Tournaments",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_TeamId1",
                table: "Players",
                column: "TeamId1");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_TeamId",
                table: "Matches",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_TeamId1",
                table: "Matches",
                column: "TeamId1");

            migrationBuilder.CreateIndex(
                name: "IX_BaseUsers_Username",
                table: "BaseUsers",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamUsers_GroupId",
                table: "TeamUsers",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchCards_TeamUsers_TeamId",
                table: "MatchCards",
                column: "TeamId",
                principalTable: "TeamUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_TeamUsers_AwayTeamId",
                table: "Matches",
                column: "AwayTeamId",
                principalTable: "TeamUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_TeamUsers_HomeTeamId",
                table: "Matches",
                column: "HomeTeamId",
                principalTable: "TeamUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Teams_TeamId",
                table: "Matches",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Teams_TeamId1",
                table: "Matches",
                column: "TeamId1",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchGoals_TeamUsers_TeamId",
                table: "MatchGoals",
                column: "TeamId",
                principalTable: "TeamUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchLineups_TeamUsers_TeamId",
                table: "MatchLineups",
                column: "TeamId",
                principalTable: "TeamUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_TeamUsers_TeamId",
                table: "Players",
                column: "TeamId",
                principalTable: "TeamUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Teams_TeamId1",
                table: "Players",
                column: "TeamId1",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamTournament_TeamUsers_TeamsId",
                table: "TeamTournament",
                column: "TeamsId",
                principalTable: "TeamUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tournaments_Teams_TeamId",
                table: "Tournaments",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchCards_TeamUsers_TeamId",
                table: "MatchCards");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_TeamUsers_AwayTeamId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_TeamUsers_HomeTeamId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Teams_TeamId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Teams_TeamId1",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchGoals_TeamUsers_TeamId",
                table: "MatchGoals");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchLineups_TeamUsers_TeamId",
                table: "MatchLineups");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_TeamUsers_TeamId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Teams_TeamId1",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamTournament_TeamUsers_TeamsId",
                table: "TeamTournament");

            migrationBuilder.DropForeignKey(
                name: "FK_Tournaments_Teams_TeamId",
                table: "Tournaments");

            migrationBuilder.DropTable(
                name: "AdminUsers");

            migrationBuilder.DropTable(
                name: "TeamUsers");

            migrationBuilder.DropTable(
                name: "WatcherUsers");

            migrationBuilder.DropTable(
                name: "BaseUsers");

            migrationBuilder.DropIndex(
                name: "IX_Tournaments_TeamId",
                table: "Tournaments");

            migrationBuilder.DropIndex(
                name: "IX_Players_TeamId1",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Matches_TeamId",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_TeamId1",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "TeamId1",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "TeamId1",
                table: "Matches");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchCards_Teams_TeamId",
                table: "MatchCards",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Teams_AwayTeamId",
                table: "Matches",
                column: "AwayTeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Teams_HomeTeamId",
                table: "Matches",
                column: "HomeTeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchGoals_Teams_TeamId",
                table: "MatchGoals",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchLineups_Teams_TeamId",
                table: "MatchLineups",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Teams_TeamId",
                table: "Players",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamTournament_Teams_TeamsId",
                table: "TeamTournament",
                column: "TeamsId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
