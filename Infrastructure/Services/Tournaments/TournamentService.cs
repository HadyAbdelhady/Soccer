using Business.DTOs.Tournaments;
using Business.Services.Standings;
using Data.Entities;
using Infra.enums;
using Infra.Interface;
using Infra.ResultWrapper;
using Microsoft.EntityFrameworkCore;

namespace Business.Services.Tournaments
{
    public class TournamentService(IUnitOfWork unitOfWork, IStandingsService standingsService) : ITournamentService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;
        private readonly IStandingsService standingsService = standingsService;

        public async Task<Result<CreateTournamentResponse>> CreateTournament(CreateTournamentRequest request)
        {
            var tournament = new Tournament
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                StartDateTime = request.StartDate,
                EndDateTime = request.EndDate,
                Type = request.Type,
                Legs = request.Legs,
                GroupCount = request.GroupCount,
                TeamsToAdvance = request.TeamsToAdvance,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await unitOfWork.Repository<Tournament>().AddAsync(tournament);
            await unitOfWork.SaveChangesAsync();

            var result = new CreateTournamentResponse
            {
                Id = tournament.Id,
                Name = tournament.Name,
                Message = "Created Successfully"
            };
            return Result<CreateTournamentResponse>.Success(result);
        }

        public async Task<Result<AddTeamToTournamentResponse>> AddTeamToTournament(AddTeamToTournamentRequest request)
        {
            var tournament = await unitOfWork.Repository<Tournament>()
                .GetAll()
                .Include(t => t.Teams)
                .FirstOrDefaultAsync(t => t.Id == request.TournamentId);

            if (tournament == null)
            {
                return Result<AddTeamToTournamentResponse>.FailureStatusCode("Tournament not found", ErrorType.NotFound);
            }

            var team = await unitOfWork.Repository<Team>().GetByIdAsync(request.TeamId);

            if (team == null)
            {
                return Result<AddTeamToTournamentResponse>.FailureStatusCode("Team not found", ErrorType.NotFound);
            }

            if (tournament.Teams.Any(t => t.Id == request.TeamId))
            {
                return Result<AddTeamToTournamentResponse>.FailureStatusCode("Team is already enrolled in this tournament", ErrorType.BadRequest);
            }

            tournament.Teams.Add(team);
            tournament.UpdatedAt = DateTimeOffset.UtcNow;

            unitOfWork.Repository<Tournament>().Update(tournament);
            await unitOfWork.SaveChangesAsync();

            var response = new AddTeamToTournamentResponse
            {
                TournamentId = tournament.Id,
                TeamId = team.Id,
                TeamName = team.Name,
                Message = "Team added to tournament successfully"
            };

            return Result<AddTeamToTournamentResponse>.Success(response);
        }


        public async Task<Result<GenerateTournamentGroupsResponse>> GenerateGroupsAsync(Guid tournamentId)
        {
            var tournament = await unitOfWork.Repository<Tournament>()
                .GetAll()
                .Include(t => t.Teams)
                .Include(t => t.Groups)
                .FirstOrDefaultAsync(t => t.Id == tournamentId);

            if (tournament == null)
            {
                return Result<GenerateTournamentGroupsResponse>.FailureStatusCode("Tournament not found", ErrorType.NotFound);
            }

            if (tournament.Teams == null || !tournament.Teams.Any())
            {
                return Result<GenerateTournamentGroupsResponse>.FailureStatusCode("Tournament has no teams to draw into groups", ErrorType.BadRequest);
            }

            if (tournament.Groups.Any())
            {
                return Result<GenerateTournamentGroupsResponse>.FailureStatusCode("Groups already exist for this tournament", ErrorType.BadRequest);
            }

            var createdGroups = new List<Group>();

            if (tournament.Type == TournamentType.SINGLE_GROUP)
            {
                var shuffledTeams = tournament.Teams.OrderBy(t => Guid.NewGuid()).ToList();

                var group = new Group
                {
                    Id = Guid.NewGuid(),
                    Name = "Group A",
                    TournamentId = tournament.Id,
                    Tournament = tournament,
                    CreatedAt = DateTimeOffset.UtcNow
                };

                group.Teams = shuffledTeams;
                tournament.Groups.Add(group);

                foreach (var team in shuffledTeams)
                {
                    team.GroupId = group.Id;
                    team.UpdatedAt = DateTimeOffset.UtcNow;
                    unitOfWork.Repository<Team>().Update(team);
                }

                createdGroups.Add(group);
            }
            else if (tournament.Type == TournamentType.MULTI_GROUP_KNOCKOUT)
            {
                if (tournament.GroupCount == null || tournament.GroupCount <= 0)
                {
                    return Result<GenerateTournamentGroupsResponse>.FailureStatusCode("GroupCount must be specified for multi-group tournaments", ErrorType.BadRequest);
                }

                var teams = tournament.Teams.ToList();
                if (teams.Count % tournament.GroupCount != 0)
                {
                    return Result<GenerateTournamentGroupsResponse>.FailureStatusCode("Teams cannot be evenly distributed across the configured number of groups", ErrorType.BadRequest);
                }

                var shuffledTeams = teams.OrderBy(t => Guid.NewGuid()).ToList();
                int teamsPerGroup = teams.Count / tournament.GroupCount.Value;

                for (int i = 0; i < tournament.GroupCount.Value; i++)
                {
                    var group = new Group
                    {
                        Id = Guid.NewGuid(),
                        Name = $"Group {((char)('A' + i))}",
                        TournamentId = tournament.Id,
                        Tournament = tournament,
                        CreatedAt = DateTimeOffset.UtcNow
                    };

                    var groupTeams = shuffledTeams.Skip(i * teamsPerGroup).Take(teamsPerGroup).ToList();
                    group.Teams = groupTeams;
                    tournament.Groups.Add(group);

                    foreach (var team in groupTeams)
                    {
                        team.GroupId = group.Id;
                        team.UpdatedAt = DateTimeOffset.UtcNow;
                        unitOfWork.Repository<Team>().Update(team);
                    }

                    createdGroups.Add(group);
                }
            }
            else
            {
                return Result<GenerateTournamentGroupsResponse>.FailureStatusCode("Unsupported tournament type for group draw", ErrorType.BadRequest);
            }

            unitOfWork.Repository<Tournament>().Update(tournament);
            await unitOfWork.SaveChangesAsync();

            var response = new GenerateTournamentGroupsResponse
            {
                TournamentId = tournament.Id,
                Message = "Groups generated successfully",
                Groups = createdGroups
                    .Select(g => new GeneratedGroupDto
                    {
                        Id = g.Id,
                        Name = g.Name,
                        TeamCount = g.Teams.Count
                    })
                    .ToList()
            };

            return Result<GenerateTournamentGroupsResponse>.Success(response);
        }

        public async Task<Result<RegenerateGroupsResponse>> RegenerateGroupsAsync(Guid tournamentId)
        {
            var tournament = await unitOfWork.Repository<Tournament>()
                .GetAll()
                .Include(t => t.Teams)
                .Include(t => t.Groups)
                    .ThenInclude(g => g.Teams)
                .FirstOrDefaultAsync(t => t.Id == tournamentId);

            if (tournament == null)
            {
                return Result<RegenerateGroupsResponse>.FailureStatusCode("Tournament not found", ErrorType.NotFound);
            }

            if (tournament.Teams == null || !tournament.Teams.Any())
            {
                return Result<RegenerateGroupsResponse>.FailureStatusCode("Tournament has no teams to redistribute into groups", ErrorType.BadRequest);
            }

            if (!tournament.Groups.Any())
            {
                return Result<RegenerateGroupsResponse>.FailureStatusCode("No groups exist. Please generate groups first using the draw endpoint", ErrorType.BadRequest);
            }

            var shuffledTeams = tournament.Teams.OrderBy(t => Guid.NewGuid()).ToList();

            if (tournament.Type == TournamentType.SINGLE_GROUP)
            {
                // For single group, just reassign all teams to the existing group
                var group = tournament.Groups.First();

                // Clear existing team assignments
                foreach (var team in group.Teams.ToList())
                {
                    team.GroupId = null;
                    team.UpdatedAt = DateTimeOffset.UtcNow;
                    unitOfWork.Repository<Team>().Update(team);
                }
                group.Teams.Clear();

                // Assign shuffled teams to the group
                foreach (var team in shuffledTeams)
                {
                    team.GroupId = group.Id;
                    team.UpdatedAt = DateTimeOffset.UtcNow;
                    unitOfWork.Repository<Team>().Update(team);
                }
                group.Teams = shuffledTeams;
                group.UpdatedAt = DateTimeOffset.UtcNow;
            }
            else if (tournament.Type == TournamentType.MULTI_GROUP_KNOCKOUT)
            {
                if (tournament.GroupCount == null || tournament.GroupCount <= 0)
                {
                    return Result<RegenerateGroupsResponse>.FailureStatusCode("GroupCount must be specified for multi-group tournaments", ErrorType.BadRequest);
                }

                if (shuffledTeams.Count % tournament.GroupCount != 0)
                {
                    return Result<RegenerateGroupsResponse>.FailureStatusCode("Teams cannot be evenly distributed across the configured number of groups", ErrorType.BadRequest);
                }

                int teamsPerGroup = shuffledTeams.Count / tournament.GroupCount.Value;

                // Get existing groups ordered by name
                var existingGroups = tournament.Groups.OrderBy(g => g.Name).ToList();

                // Clear all existing team assignments
                foreach (var group in existingGroups)
                {
                    foreach (var team in group.Teams.ToList())
                    {
                        team.GroupId = null;
                        team.UpdatedAt = DateTimeOffset.UtcNow;
                        unitOfWork.Repository<Team>().Update(team);
                    }
                    group.Teams.Clear();
                }

                // Redistribute teams to existing groups
                for (int i = 0; i < existingGroups.Count; i++)
                {
                    var group = existingGroups[i];
                    var groupTeams = shuffledTeams.Skip(i * teamsPerGroup).Take(teamsPerGroup).ToList();

                    foreach (var team in groupTeams)
                    {
                        team.GroupId = group.Id;
                        team.UpdatedAt = DateTimeOffset.UtcNow;
                        unitOfWork.Repository<Team>().Update(team);
                    }

                    group.Teams = groupTeams;
                    group.UpdatedAt = DateTimeOffset.UtcNow;
                }
            }
            else
            {
                return Result<RegenerateGroupsResponse>.FailureStatusCode("Unsupported tournament type for group regeneration", ErrorType.BadRequest);
            }

            unitOfWork.Repository<Tournament>().Update(tournament);
            await unitOfWork.SaveChangesAsync();

            var response = new RegenerateGroupsResponse
            {
                TournamentId = tournament.Id,
                Message = "Groups regenerated successfully",
                Groups = tournament.Groups
                    .OrderBy(g => g.Name)
                    .Select(g => new GeneratedGroupDto
                    {
                        Id = g.Id,
                        Name = g.Name,
                        TeamCount = g.Teams.Count
                    })
                    .ToList()
            };

            return Result<RegenerateGroupsResponse>.Success(response);
        }


        public async Task<Result<GenerateTournamentMatchesResponse>> GenerateMatchesAsync(Guid tournamentId)
        {
            var tournament = await unitOfWork.Repository<Tournament>()
                .GetAll()
                .Include(t => t.Teams)
                .Include(t => t.Groups).ThenInclude(g => g.Teams)
                .Include(t => t.Matches)
                .FirstOrDefaultAsync(t => t.Id == tournamentId);

            if (tournament == null)
            {
                return Result<GenerateTournamentMatchesResponse>.FailureStatusCode("Tournament not found", ErrorType.NotFound);
            }

            if (!tournament.Teams.Any())
            {
                return Result<GenerateTournamentMatchesResponse>.FailureStatusCode("Tournament has no teams to generate matches for", ErrorType.BadRequest);
            }

            if (!tournament.Groups.Any())
            {
                return Result<GenerateTournamentMatchesResponse>.FailureStatusCode("Generate groups before generating matches", ErrorType.BadRequest);
            }

            if (tournament.Matches.Any())
            {
                return Result<GenerateTournamentMatchesResponse>.FailureStatusCode("Matches already exist for this tournament", ErrorType.BadRequest);
            }

            int beforeCount = tournament.Matches.Count;

            if (tournament.Type == TournamentType.SINGLE_GROUP)
            {
                if (tournament.Groups.Count != 1)
                {
                    return Result<GenerateTournamentMatchesResponse>.FailureStatusCode("Single-group tournaments must have exactly one group", ErrorType.BadRequest);
                }

                var group = tournament.Groups.First();
                var groupTeams = group.Teams.ToList();

                if (groupTeams.Count < 2)
                {
                    return Result<GenerateTournamentMatchesResponse>.FailureStatusCode("Group must contain at least two teams to generate matches", ErrorType.BadRequest);
                }

                GenerateSingleGroupDraw(tournament, groupTeams, group);
            }
            else if (tournament.Type == TournamentType.MULTI_GROUP_KNOCKOUT)
            {
                GenerateMultiGroupKnockoutDraw(tournament);
            }
            else
            {
                return Result<GenerateTournamentMatchesResponse>.FailureStatusCode("Unsupported tournament type for match draw", ErrorType.BadRequest);
            }

            await unitOfWork.SaveChangesAsync();

            int createdCount = tournament.Matches.Count - beforeCount;

            var response = new GenerateTournamentMatchesResponse
            {
                TournamentId = tournament.Id,
                CreatedMatchCount = createdCount,
                Message = "Matches generated successfully"
            };

            return Result<GenerateTournamentMatchesResponse>.Success(response);
        }

        private void GenerateSingleGroupDraw(Tournament tournament, List<Team> teams, Group? group)
        {
            var teamList = teams.OrderBy(x => Guid.NewGuid()).ToList();
            int n = teamList.Count;
            bool hasGhost = n % 2 != 0;
            if (hasGhost) n++;

            int rounds = n - 1;
            int matchesPerRound = n / 2;

            for (int r = 0; r < rounds; r++)
            {
                for (int m = 0; m < matchesPerRound; m++)
                {
                    int homeIdx = (r + m) % (n - 1);
                    int awayIdx = (n - 1 - m + r) % (n - 1);

                    if (m == 0) homeIdx = n - 1;

                    if (homeIdx >= teamList.Count || awayIdx >= teamList.Count) continue;

                    var homeTeam = teamList[homeIdx];
                    var awayTeam = teamList[awayIdx];

                    // Swap home/away every other round for fairness
                    if (r % 2 == 1) (homeTeam, awayTeam) = (awayTeam, homeTeam);

                    AddMatch(tournament, group, homeTeam, awayTeam, r + 1);

                    if (tournament.Legs == LegsType.DOUBLE)
                    {
                        AddMatch(tournament, group, awayTeam, homeTeam, r + 1 + rounds);
                    }
                }
            }
        }

        private void GenerateMultiGroupKnockoutDraw(Tournament tournament)
        {
            if (tournament.GroupCount == null || tournament.GroupCount <= 0) return;
            if (!tournament.Groups.Any()) return;

            foreach (var group in tournament.Groups.OrderBy(g => g.Name))
            {
                var groupTeams = group.Teams.ToList();
                if (groupTeams.Count < 2) continue;

                GenerateSingleGroupDraw(tournament, groupTeams, group);
            }

            GenerateKnockoutPlaceholders(tournament);
        }
        
        private void GenerateKnockoutPlaceholders(Tournament tournament)
        {
            if (tournament.GroupCount == null || tournament.TeamsToAdvance == null)
                return;

            int totalQualified =
                tournament.GroupCount.Value * tournament.TeamsToAdvance.Value;

            if ((totalQualified & (totalQualified - 1)) != 0)
                return;

            // 1️⃣ Build initial placeholders from groups
            var currentRound = new List<string>();

            for (int g = 0; g < tournament.GroupCount.Value; g++)
            {
                char groupChar = (char)('A' + g);

                for (int r = 1; r <= tournament.TeamsToAdvance.Value; r++)
                {
                    currentRound.Add(
                        r == 1
                            ? $"Winner OF Group {groupChar}"
                            : $"Runner-up OF Group {groupChar}"
                    );
                }
            }

            int roundNumber = 1;
            int matchCount = currentRound.Count / 2;

            // 2️⃣ Generate knockout rounds
            while (currentRound.Count > 1)
            {
                var nextRound = new List<string>();
                string stagePrefix = GetStageName(matchCount);

                for (int i = 0; i < currentRound.Count; i += 2)
                {
                    string matchName = $"{stagePrefix} {i / 2 + 1}";

                    AddPlaceholderMatch(
                        tournament,
                        currentRound[i],
                        currentRound[i + 1],
                        roundNumber,
                        matchName
                    );

                    nextRound.Add($"Winner OF {matchName}");
                }

                currentRound = nextRound;
                matchCount /= 2;
                roundNumber++;
            }
        }
        private string GetStageName(int matchCount)
        {
            return matchCount switch
            {
                16 => "R32",
                8 => "R16",
                4 => "QF",
                2 => "SF",
                1 => "Final",
                _ => "KO"
            };
        }

        private void AddMatch(Tournament tournament, Group? group, Team home, Team away, int round)
        {
            var match = new Match
            {
                Id = Guid.NewGuid(),
                TournamentId = tournament.Id,
                Tournament = tournament,
                GroupId = group?.Id,
                Group = group,
                HomeTeamId = home.Id,
                HomeTeam = home,
                AwayTeamId = away.Id,
                AwayTeam = away,
                RoundNumber = round,
                Status = MatchStatus.SCHEDULED,
                StageType = group != null || tournament.Type == TournamentType.SINGLE_GROUP ? StageType.GROUP : StageType.KNOCKOUT,
                CreatedAt = DateTimeOffset.UtcNow
            };
            tournament.Matches.Add(match);
        }

        private void AddPlaceholderMatch(Tournament tournament, string homePlaceholder, string awayPlaceholder, int round, string stageName)
        {
            var match = new Match
            {
                Id = Guid.NewGuid(),
                TournamentId = tournament.Id,
                Tournament = tournament,
                HomeTeamPlaceholder = homePlaceholder,
                AwayTeamPlaceholder = awayPlaceholder,
                RoundNumber = round,
                Status = MatchStatus.SCHEDULED,
                StageType = StageType.KNOCKOUT,
                Venue = stageName,
                CreatedAt = DateTimeOffset.UtcNow
            };
            tournament.Matches.Add(match);
        }


        public async Task<Result<bool>> ResolvePlaceholders(Guid tournamentId)
        {
            var tournament = await unitOfWork.Repository<Tournament>()
                .GetAll()
                .Include(t => t.Groups).ThenInclude(g => g.Matches)
                .Include(t => t.Matches)
                .FirstOrDefaultAsync(t => t.Id == tournamentId);

            if (tournament == null) return Result<bool>.FailureStatusCode("Tournament not found", ErrorType.NotFound);

            bool resolvedAny = false;

            // 1. Resolve Group Placeholders
            foreach (var group in tournament.Groups)
            {
                if (group.Matches.All(m => m.Status == MatchStatus.FINISHED))
                {
                    var standingsResult = await standingsService.GetGroupStandingsAsync(group.Id);
                    if (standingsResult.IsSuccess)
                    {
                        var standings = standingsResult.Value.Standings;

                        // Winner OF Group X
                        var winner = standings.FirstOrDefault(s => s.Rank == 1);
                        if (winner != null)
                        {
                            resolvedAny |= FillPlaceholder(tournament, $"Winner OF Group {group.Name.Split(' ').Last()}", winner.TeamId);
                        }

                        // Runner-up OF Group X
                        var runnerUp = standings.FirstOrDefault(s => s.Rank == 2);
                        if (runnerUp != null)
                        {
                            resolvedAny |= FillPlaceholder(tournament, $"Runner-up OF Group {group.Name.Split(' ').Last()}", runnerUp.TeamId);
                        }
                    }
                }
            }

            // 2. Resolve Bracket Placeholders (QF -> SF -> Final)
            var finishedMatches = tournament.Matches.Where(m => m.Status == MatchStatus.FINISHED).ToList();
            foreach (var match in finishedMatches)
            {
                if (string.IsNullOrEmpty(match.Venue)) continue; // Stage name stored in Venue for placeholders

                var (winnerId, _) = GetWinner(match);
                if (winnerId != null)
                {
                    resolvedAny |= FillPlaceholder(tournament, $"Winner OF {match.Venue}", winnerId.Value);
                }
            }

            if (resolvedAny)
            {
                await unitOfWork.SaveChangesAsync();
            }

            return Result<bool>.Success(resolvedAny);
        }

        private bool FillPlaceholder(Tournament tournament, string placeholder, Guid teamId)
        {
            bool changed = false;
            var placeholderMatches = tournament.Matches
                .Where(m => m.HomeTeamPlaceholder == placeholder || m.AwayTeamPlaceholder == placeholder)
                .ToList();

            foreach (var match in placeholderMatches)
            {
                if (match.HomeTeamPlaceholder == placeholder && match.HomeTeamId == null)
                {
                    match.HomeTeamId = teamId;
                    changed = true;
                }
                if (match.AwayTeamPlaceholder == placeholder && match.AwayTeamId == null)
                {
                    match.AwayTeamId = teamId;
                    changed = true;
                }
            }
            return changed;
        }

        private (Guid? WinnerId, Guid? LoserId) GetWinner(Match match)
        {
            // Simple logic: most goals wins. 
            // In a real scenario, we'd check MatchGoal count and handle pens/extra time.
            // For now, assuming SubmitMatchResult already processed goals.
            int homeScore = match.Goals.Count(g => g.TeamId == match.HomeTeamId && g.GoalType != GoalType.OWNGOAL) +
                            match.Goals.Count(g => g.TeamId == match.AwayTeamId && g.GoalType == GoalType.OWNGOAL);
            int awayScore = match.Goals.Count(g => g.TeamId == match.AwayTeamId && g.GoalType != GoalType.OWNGOAL) +
                            match.Goals.Count(g => g.TeamId == match.HomeTeamId && g.GoalType == GoalType.OWNGOAL);

            if (homeScore > awayScore) return (match.HomeTeamId, match.AwayTeamId);
            if (awayScore > homeScore) return (match.AwayTeamId, match.HomeTeamId);

            return (null, null); // Tie (shouldn't happen in final knockout stages due to pens)
        }

        public async Task<Result<UpdateTournamentResponse>> UpdateTournament(UpdateTournamentRequest request)
        {
            var tournament = await unitOfWork.Repository<Tournament>().GetByIdAsync(request.Id);

            if (tournament == null)
            {
                return Result<UpdateTournamentResponse>.FailureStatusCode("Tournament not found", ErrorType.NotFound);
            }

            tournament.Name = request.Name;
            tournament.StartDateTime = request.StartDate;
            tournament.EndDateTime = request.EndDate;
            tournament.UpdatedAt = DateTimeOffset.UtcNow;

            unitOfWork.Repository<Tournament>().Update(tournament);
            await unitOfWork.SaveChangesAsync();

            var response = new UpdateTournamentResponse
            {
                Message = "Updated Successfully",
                Id = tournament.Id,
                Name = tournament.Name,
                StartDate = tournament.StartDateTime,
                EndDate = tournament.EndDateTime
            };

            return Result<UpdateTournamentResponse>.Success(response);
        }

        public async Task<Result<DeleteTournamentResponse>> DeleteTournament(Guid id)
        {
            var tournament = await unitOfWork.Repository<Tournament>().GetByIdAsync(id);

            if (tournament == null)
            {
                return Result<DeleteTournamentResponse>.FailureStatusCode("Tournament not found", ErrorType.NotFound);
            }

            tournament.UpdatedAt = DateTimeOffset.UtcNow;
            unitOfWork.Repository<Tournament>().Remove(tournament);
            await unitOfWork.SaveChangesAsync();

            var response = new DeleteTournamentResponse
            {
                Id = id,
                IsDeleted = true,
                Message = "Deleted Successfully"
            };

            return Result<DeleteTournamentResponse>.Success(response);
        }
    }
}
