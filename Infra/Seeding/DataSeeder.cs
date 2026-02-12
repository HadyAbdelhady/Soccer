using Data.Entities;
using Infra.DBContext;
using Infra.enums;
using Microsoft.EntityFrameworkCore;

namespace Infra.Seeding
{
    /// <summary>
    /// Seeds bogus data for testing when the database is empty.
    /// </summary>
    public static class DataSeeder
    {
        private const string SeedPassword = "Test123!";

        private static string HashPassword(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

        public static async Task SeedIfEmptyAsync(SoccerDbContext db)
        {
            if (db == null) return;

            // If DB already has team users but no users (e.g. after adding User table), seed Admin and Viewer only
            if (await db.TeamUsers.IgnoreQueryFilters().AnyAsync() && !await db.Users.IgnoreQueryFilters().AnyAsync())
            {
                var hashedSeed = HashPassword(SeedPassword);
                var now = DateTimeOffset.UtcNow;
                db.Users.Add(new User { Id = Guid.Parse("e0000001-0001-0001-0001-000000000001"), FullName = "System Admin", Username = "admin", HashedPassword = hashedSeed, Role = UserRole.Admin, IsDeleted = false, CreatedAt = now });
                db.Users.Add(new User { Id = Guid.Parse("e0000001-0001-0001-0001-000000000002"), FullName = "Guest Viewer", Username = "viewer", HashedPassword = hashedSeed, Role = UserRole.Viewer, IsDeleted = false, CreatedAt = now });
                await db.SaveChangesAsync();
                return;
            }

            var strategy = db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await db.Database.BeginTransactionAsync();
                try
                {
                    if (await db.TeamUsers.IgnoreQueryFilters().AnyAsync())
                    {
                        return;
                    }

                    var now = DateTimeOffset.UtcNow;
                    var tournamentStart = DateTime.UtcNow.AddMonths(-1);
                    var tournamentEnd = DateTime.UtcNow.AddMonths(2);

                    // Tournament
                    var tournamentId = Guid.Parse("a0000001-0001-0001-0001-000000000001");
                    var tournament = new Tournament
                    {
                        Id = tournamentId,
                        Name = "Test League 2025",
                        StartDateTime = tournamentStart,
                        EndDateTime = tournamentEnd,
                        IsDeleted = false,
                        CreatedAt = now
                    };
                    db.Tournaments.Add(tournament);

                    // Groups
                    var groupAId = Guid.Parse("b0000001-0001-0001-0001-000000000001");
                    var groupBId = Guid.Parse("b0000001-0001-0001-0001-000000000002");
                    db.Groups.Add(new Group
                    {
                        Id = groupAId,
                        Name = "A",
                        TournamentId = tournamentId,
                        IsDeleted = false,
                        CreatedAt = now
                    });
                    db.Groups.Add(new Group
                    {
                        Id = groupBId,
                        Name = "B",
                        TournamentId = tournamentId,
                        IsDeleted = false,
                        CreatedAt = now
                    });

                    // Teams (4) - using TeamUser entities
                    var team1Id = Guid.Parse("c0000001-0001-0001-0001-000000000001");
                    var team2Id = Guid.Parse("c0000001-0001-0001-0001-000000000002");
                    var team3Id = Guid.Parse("c0000001-0001-0001-0001-000000000003");
                    var team4Id = Guid.Parse("c0000001-0001-0001-0001-000000000004");

                    var hashedSeed = HashPassword(SeedPassword);
                    var teamUsers = new[]
                    {
                        new TeamUser { Id = team1Id, FullName = "Red FC", Username = "red_fc", HashedPassword = hashedSeed, IsDeleted = false, CreatedAt = now, GroupId = groupAId },
                        new TeamUser { Id = team2Id, FullName = "Blue United", Username = "blue_united", HashedPassword = hashedSeed, IsDeleted = false, CreatedAt = now, GroupId = groupAId },
                        new TeamUser { Id = team3Id, FullName = "Green Rangers", Username = "green_rangers", HashedPassword = hashedSeed, IsDeleted = false, CreatedAt = now, GroupId = groupBId },
                        new TeamUser { Id = team4Id, FullName = "Yellow Strikers", Username = "yellow_strikers", HashedPassword = hashedSeed, IsDeleted = false, CreatedAt = now, GroupId = groupBId }
                    };
                    foreach (var t in teamUsers)
                        db.TeamUsers.Add(t);

                    // Admin and Viewer users (same password as seed: Test123!)
                    var adminId = Guid.Parse("e0000001-0001-0001-0001-000000000001");
                    var viewerId = Guid.Parse("e0000001-0001-0001-0001-000000000002");
                    db.Users.Add(new User { Id = adminId, FullName = "System Admin", Username = "admin", HashedPassword = hashedSeed, Role = UserRole.Admin, IsDeleted = false, CreatedAt = now });
                    db.Users.Add(new User { Id = viewerId, FullName = "Guest Viewer", Username = "viewer", HashedPassword = hashedSeed, Role = UserRole.Viewer, IsDeleted = false, CreatedAt = now });

                    // Tournament-Teams many-to-many
                    foreach (var t in teamUsers)
                        t.Tournaments.Add(tournament);

                    // Players (a few per team)
                    var playerIds = new List<Guid>();
                    int jersey = 1;
                    foreach (var team in teamUsers)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            var pid = Guid.CreateVersion7();
                            playerIds.Add(pid);
                            db.Players.Add(new Player
                            {
                                Id = pid,
                                FullName = $"{team.FullName} Player {i + 1}",
                                NickName = $"P{i + 1}",
                                Position = (PlayerPosition)(i % 4),
                                JerseyNumber = jersey++,
                                IsCaptain = i == 0,
                                TeamId = team.Id,
                                IsDeleted = false,
                                CreatedAt = now
                            });
                        }
                        jersey = 1;
                    }

                    await db.SaveChangesAsync();

                    // Resolve player IDs by team for goals (we need ScorerId = PlayerId for a player in the scoring team)
                    var playersByTeam = await db.Players.Where(p => p.TeamId == team1Id || p.TeamId == team2Id || p.TeamId == team3Id || p.TeamId == team4Id)
                        .Select(p => new { p.Id, p.TeamId })
                        .ToListAsync();
                    var team1Players = playersByTeam.Where(x => x.TeamId == team1Id).Select(x => x.Id).ToList();
                    var team2Players = playersByTeam.Where(x => x.TeamId == team2Id).Select(x => x.Id).ToList();
                    var team3Players = playersByTeam.Where(x => x.TeamId == team3Id).Select(x => x.Id).ToList();
                    var team4Players = playersByTeam.Where(x => x.TeamId == team4Id).Select(x => x.Id).ToList();

                    // Matches (Group A: team1 vs team2, team3 vs team4 in Group B; then cross or more)
                    var match1Id = Guid.Parse("d0000001-0001-0001-0001-000000000001");
                    var match2Id = Guid.Parse("d0000001-0001-0001-0001-000000000002");
                    var match3Id = Guid.Parse("d0000001-0001-0001-0001-000000000003");
                    var match4Id = Guid.Parse("d0000001-0001-0001-0001-000000000004");

                    var kick1 = DateTime.UtcNow.AddDays(-7);
                    var kick2 = DateTime.UtcNow.AddDays(-5);
                    var kick3 = DateTime.UtcNow.AddDays(2);
                    var kick4 = DateTime.UtcNow.AddDays(4);

                    db.Matches.Add(new Match
                    {
                        Id = match1Id,
                        HomeTeamId = team1Id,
                        AwayTeamId = team2Id,
                        TournamentId = tournamentId,
                        GroupId = groupAId,
                        KickoffTime = kick1,
                        FinalWhistleTime = kick1.AddHours(2),
                        Status = MatchStatus.FINISHED,
                        Venue = "Main Stadium",
                        StageType = StageType.GROUP,
                        IsDeleted = false,
                        CreatedAt = now
                    });
                    db.Matches.Add(new Match
                    {
                        Id = match2Id,
                        HomeTeamId = team3Id,
                        AwayTeamId = team4Id,
                        TournamentId = tournamentId,
                        GroupId = groupBId,
                        KickoffTime = kick2,
                        FinalWhistleTime = kick2.AddHours(2),
                        Status = MatchStatus.FINISHED,
                        Venue = "Arena Park",
                        StageType = StageType.GROUP,
                        IsDeleted = false,
                        CreatedAt = now
                    });
                    db.Matches.Add(new Match
                    {
                        Id = match3Id,
                        HomeTeamId = team1Id,
                        AwayTeamId = team3Id,
                        TournamentId = tournamentId,
                        GroupId = null,
                        KickoffTime = kick3,
                        FinalWhistleTime = null,
                        Status = MatchStatus.SCHEDULED,
                        Venue = "Main Stadium",
                        StageType = StageType.KNOCKOUT,
                        IsDeleted = false,
                        CreatedAt = now
                    });
                    db.Matches.Add(new Match
                    {
                        Id = match4Id,
                        HomeTeamId = team2Id,
                        AwayTeamId = team4Id,
                        TournamentId = tournamentId,
                        GroupId = null,
                        KickoffTime = kick4,
                        FinalWhistleTime = null,
                        Status = MatchStatus.SCHEDULED,
                        Venue = "Arena Park",
                        StageType = StageType.KNOCKOUT,
                        IsDeleted = false,
                        CreatedAt = now
                    });

                    await db.SaveChangesAsync();

                    // Goals for finished matches (match1: 2-1, match2: 0-0)
                    if (team1Players.Count > 0 && team2Players.Count > 0)
                    {
                        db.MatchGoals.Add(new MatchGoal { Id = Guid.CreateVersion7(), MatchId = match1Id, TeamId = team1Id, ScorerId = team1Players[0], Minute = 23, GoalType = GoalType.REGULAR, IsDeleted = false, CreatedAt = now });
                        db.MatchGoals.Add(new MatchGoal { Id = Guid.CreateVersion7(), MatchId = match1Id, TeamId = team1Id, ScorerId = team1Players[1], Minute = 67, GoalType = GoalType.REGULAR, IsDeleted = false, CreatedAt = now });
                        db.MatchGoals.Add(new MatchGoal { Id = Guid.CreateVersion7(), MatchId = match1Id, TeamId = team2Id, ScorerId = team2Players[0], Minute = 89, GoalType = GoalType.PENALITY, IsDeleted = false, CreatedAt = now });
                    }

                    await db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

    }
}
