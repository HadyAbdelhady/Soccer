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
            var teams = await unitOfWork.Repository<Team>()
                .GetAll()
                .Where(t => request.TeamIds.Contains(t.Id))
                .ToListAsync();

            if (teams.Count != request.TeamIds.Count)
                return Result<CreateTournamentResponse>.FailureStatusCode("One or more teams not found", ErrorType.NotFound);

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
                CreatedAt = DateTimeOffset.UtcNow,
                Teams = teams
            };

            GenerateDraw(tournament, teams);

            await unitOfWork.Repository<Tournament>().AddAsync(tournament);
            await unitOfWork.SaveChangesAsync();

            var result = new CreateTournamentResponse
            {
                Id = tournament.Id,
                Name = tournament.Name,
                Message = "Created Successfully with Draw"
            };
            return Result<CreateTournamentResponse>.Success(result);
        }

        private void GenerateDraw(Tournament tournament, List<Team> teams)
        {
            if (tournament.Type == TournamentType.SINGLE_GROUP)
            {
                GenerateSingleGroupDraw(tournament, teams, null);
            }
            else if (tournament.Type == TournamentType.MULTI_GROUP_KNOCKOUT)
            {
                GenerateMultiGroupKnockoutDraw(tournament, teams);
            }
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

        private void GenerateMultiGroupKnockoutDraw(Tournament tournament, List<Team> teams)
        {
            if (tournament.GroupCount == null || tournament.GroupCount <= 0) return;
            if (teams.Count % tournament.GroupCount != 0) return;

            var shuffledTeams = teams.OrderBy(x => Guid.NewGuid()).ToList();
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

                GenerateSingleGroupDraw(tournament, groupTeams, group);
            }

            GenerateKnockoutPlaceholders(tournament);
        }

        //private void GenerateKnockoutPlaceholders(Tournament tournament)
        //{
        //    // Simplified fixed bracket logic for demo (QF -> SF -> Final)
        //    // Assuming 4 groups, top 2 advance = 8 teams
        //    if (tournament.GroupCount == 4 && tournament.TeamsToAdvance == 2)
        //    {
        //        // QF1: A1 vs C2
        //        // QF2: B1 vs D2
        //        // QF3: C1 vs A2
        //        // QF4: D1 vs B2
        //        AddPlaceholderMatch(tournament, "Winner OF Group A", "Runner-up OF Group C", 1, "QF 1");
        //        AddPlaceholderMatch(tournament, "Winner OF Group B", "Runner-up OF Group D", 1, "QF 2");
        //        AddPlaceholderMatch(tournament, "Winner OF Group C", "Runner-up OF Group A", 1, "QF 3");
        //        AddPlaceholderMatch(tournament, "Winner OF Group D", "Runner-up OF Group B", 1, "QF 4");

        //        // SF1: WQF1 vs WQF3
        //        AddPlaceholderMatch(tournament, "Winner OF QF 1", "Winner OF QF 3", 2, "SF 1");
        //        // SF2: WQF2 vs WQF4
        //        AddPlaceholderMatch(tournament, "Winner OF QF 2", "Winner OF QF 4", 2, "SF 2");

        //        // Final: WSF1 vs WSF2
        //        AddPlaceholderMatch(tournament, "Winner OF SF 1", "Winner OF SF 2", 3, "Final");
        //    }
        //    else if (tournament.GroupCount == 2 && tournament.TeamsToAdvance == 2)
        //    {
        //        // SF1: A1 vs B2
        //        // SF2: B1 vs A2
        //        AddPlaceholderMatch(tournament, "Winner OF Group A", "Runner-up OF Group B", 1, "SF 1");
        //        AddPlaceholderMatch(tournament, "Winner OF Group B", "Runner-up OF Group A", 1, "SF 2");

        //        // Final: WSF1 vs WSF2
        //        AddPlaceholderMatch(tournament, "Winner OF SF 1", "Winner OF SF 2", 2, "Final");
        //    }
        //}
        private void GenerateKnockoutPlaceholders(Tournament tournament)
        {
            if (tournament.GroupCount == null || tournament.TeamsToAdvance == null)
                return;

            int totalQualified =
                tournament.GroupCount.Value * tournament.TeamsToAdvance.Value;

            // لازم يكون Power of 2 (8, 16, 32...)
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
