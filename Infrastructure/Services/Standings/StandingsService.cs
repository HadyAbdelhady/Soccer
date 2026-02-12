using Business.DTOs.Groups;
using Business.DTOs.Tournaments;
using Data.Entities;
using Infra.enums;
using Infra.Interface;
using Infra.ResultWrapper;
using Microsoft.EntityFrameworkCore;

namespace Business.Services.Standings
{
    public class StandingsService(IUnitOfWork unitOfWork) : IStandingsService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<GroupStandingsResponse>> GetGroupStandingsAsync(Guid groupId)
        {
            var group = await _unitOfWork.Repository<Group>()
                .GetAll()
                .Include(g => g.Teams)
                .Include(g => g.Matches).ThenInclude(m => m.Goals)
                .Include(g => g.Matches).ThenInclude(m => m.Cards)
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
                return Result<GroupStandingsResponse>.FailureStatusCode("Group not found", ErrorType.NotFound);

            var standings = group.Teams
                .Select(t => new TeamStandingDto
                {
                    TeamId = t.Id,
                    TeamName = t.FullName,
                    MatchesPlayed = 0,
                    HomeMatchesPlayed = 0,
                    AwayMatchesPlayed = 0,
                    Wins = 0,
                    Draws = 0,
                    Losses = 0,
                    GoalsFor = 0,
                    GoalsAgainst = 0,
                    Points = 0,
                    YellowCards = 0,
                    RedCards = 0,
                    FairPlayScore = 0
                })
                .ToDictionary(t => t.TeamId);

            // Filter only group matches (important if Knockout exists)
            var completedMatches = group.Matches
                .Where(m => m.Status == MatchStatus.FINISHED && m.GroupId == groupId)
                .ToList();

            // Alphabetical if no matches
            if (!completedMatches.Any())
            {
                var alphabetical = standings.Values
                    .OrderBy(t => t.TeamName)
                    .ToList();

                for (int i = 0; i < alphabetical.Count; i++)
                    alphabetical[i].Rank = i + 1;

                return Result<GroupStandingsResponse>.Success(new GroupStandingsResponse
                {
                    GroupId = group.Id,
                    GroupName = group.Name,
                    Standings = alphabetical,
                    TopScorers = [],
                    MostCards = []
                });
            }

            // Process finished matches
            foreach (var match in completedMatches)
            {
                var (homeGoals, awayGoals) = CalculateScore(match);

                UpdateTeamStats(standings[match.HomeTeamId!.Value], homeGoals, awayGoals, true);
                UpdateTeamStats(standings[match.AwayTeamId!.Value], awayGoals, homeGoals, false);

                foreach (var card in match.Cards)
                {
                    if (!standings.TryGetValue(card.TeamId, out var team)) continue;

                    switch (card.CardType)
                    {
                        case CardType.YELLOW:
                            team.YellowCards++;
                            team.FairPlayScore += 1;
                            break;

                        case CardType.SECONDYELLOW:
                            team.YellowCards++;
                            team.RedCards++;
                            team.FairPlayScore += 3;
                            break;

                        case CardType.RED:
                            team.RedCards++;
                            team.FairPlayScore += 3;
                            break;
                    }
                }
            }

            // Initial sorting: Points -> GD -> GF -> Alphabetical (H2H will be applied next)
            var sorted = standings.Values
                .OrderByDescending(t => t.Points)
                .ThenByDescending(t => t.GoalDifference)
                .ThenByDescending(t => t.GoalsFor)
                .ThenBy(t => t.TeamName)
                .ToList();

            // Apply Head-to-Head tie breaker after Points, GD, GF
            var finalStandings = ApplyHeadToHead(sorted, completedMatches);

            // Player stats
            var teamIds = group.Teams.Select(t => t.Id).ToList();
            var players = await _unitOfWork.Repository<Player>()
                .GetAll()
                .Where(p => teamIds.Contains(p.TeamId))
                .ToListAsync();

            var allGoals = completedMatches.SelectMany(m => m.Goals).ToList();
            var allCards = completedMatches.SelectMany(m => m.Cards).ToList();

            var playerStats = players.Select(p => new PlayerStandingDto
            {
                PlayerId = p.Id,
                PlayerName = p.FullName,
                TeamName = standings[p.TeamId].TeamName,
                Goals = allGoals.Count(g => g.ScorerId == p.Id && g.GoalType != GoalType.OWNGOAL),
                YellowCards = allCards.Count(c => c.PlayerId == p.Id &&
                    (c.CardType == CardType.YELLOW || c.CardType == CardType.SECONDYELLOW)),
                RedCards = allCards.Count(c => c.PlayerId == p.Id &&
                    (c.CardType == CardType.RED || c.CardType == CardType.SECONDYELLOW))
            }).ToList();

            for (int i = 0; i < finalStandings.Count; i++)
                finalStandings[i].Rank = i + 1;

            return Result<GroupStandingsResponse>.Success(new GroupStandingsResponse
            {
                GroupId = group.Id,
                GroupName = group.Name,
                Standings = finalStandings,
                TopScorers = playerStats.Where(p => p.Goals > 0).OrderByDescending(p => p.Goals).Take(10).ToList(),
                MostCards = playerStats
                    .Where(p => p.YellowCards > 0 || p.RedCards > 0)
                    .OrderByDescending(p => p.RedCards)
                    .ThenByDescending(p => p.YellowCards)
                    .Take(10)
                    .ToList()
            });
        }

        public async Task<Result<List<TopScorerDto>>> GetTournamentTopScorersAsync(Guid tournamentId, int? topN)
        {
            var tournament = await _unitOfWork.Repository<Tournament>()
                .GetAll()
                .Include(t => t.Matches).ThenInclude(m => m.Goals)
                .Include(t => t.Teams)
                .FirstOrDefaultAsync(t => t.Id == tournamentId);

            if (tournament == null)
                return Result<List<TopScorerDto>>.FailureStatusCode("Tournament not found", ErrorType.NotFound);

            var completedMatches = tournament.Matches
                .Where(m => m.Status == MatchStatus.FINISHED)
                .ToList();

            var playerIds = completedMatches
                .SelectMany(m => m.Goals)
                .Where(g => g.GoalType != GoalType.OWNGOAL)
                .Select(g => g.ScorerId)
                .Distinct()
                .ToList();

            var players = await _unitOfWork.Repository<Player>()
                .GetAll()
                .Include(p => p.Team)
                .Where(p => playerIds.Contains(p.Id))
                .ToListAsync();

            var playerStats = players.Select(p => new TopScorerDto
            {
                PlayerId = p.Id,
                PlayerName = p.FullName,
                TeamName = p.Team?.FullName ?? "Unknown",
                Goals = completedMatches.SelectMany(m => m.Goals)
                    .Count(g => g.ScorerId == p.Id && g.GoalType != GoalType.OWNGOAL)
            }).ToList();

            var result = playerStats
                .Where(p => p.Goals > 0)
                .OrderByDescending(p => p.Goals)
                .AsQueryable();

            if (topN.HasValue && topN.Value > 0)
                result = result.Take(topN.Value);

            return Result<List<TopScorerDto>>.Success(result.ToList());
        }

        private static (int home, int away) CalculateScore(Match match)
        {
            int home =
                match.Goals.Count(g => g.TeamId == match.HomeTeamId && g.GoalType != GoalType.OWNGOAL) +
                match.Goals.Count(g => g.TeamId == match.AwayTeamId && g.GoalType == GoalType.OWNGOAL);

            int away =
                match.Goals.Count(g => g.TeamId == match.AwayTeamId && g.GoalType != GoalType.OWNGOAL) +
                match.Goals.Count(g => g.TeamId == match.HomeTeamId && g.GoalType == GoalType.OWNGOAL);

            return (home, away);
        }

        private static void UpdateTeamStats(TeamStandingDto team, int scored, int conceded, bool isHome)
        {
            team.MatchesPlayed++;
            if (isHome) team.HomeMatchesPlayed++;
            else team.AwayMatchesPlayed++;

            team.GoalsFor += scored;
            team.GoalsAgainst += conceded;

            if (scored > conceded)
            {
                team.Wins++;
                team.Points += 3;
            }
            else if (scored == conceded)
            {
                team.Draws++;
                team.Points += 1;
            }
            else
            {
                team.Losses++;
            }
        }

        private static List<TeamStandingDto> ApplyHeadToHead(
            List<TeamStandingDto> sorted,
            List<Match> matches)
        {
            var result = new List<TeamStandingDto>(sorted);

            var tiedGroups = sorted
                .GroupBy(t => new { t.Points, t.GoalDifference, t.GoalsFor })
                .Where(g => g.Count() > 1);

            foreach (var group in tiedGroups)
            {
                var ids = group.Select(t => t.TeamId).ToList();
                var relevantMatches = matches
                    .Where(m => m.HomeTeamId.HasValue && m.AwayTeamId.HasValue && 
                                ids.Contains(m.HomeTeamId.Value) && ids.Contains(m.AwayTeamId.Value))
                    .ToList();

                if (!relevantMatches.Any()) continue;

                var mini = ids.ToDictionary(
                    id => id,
                    id => new MiniStanding { TeamId = id });

                foreach (var m in relevantMatches)
                {
                    var (hg, ag) = CalculateScore(m);

                    mini[m.HomeTeamId!.Value].GoalsFor += hg;
                    mini[m.HomeTeamId!.Value].GoalsAgainst += ag;

                    mini[m.AwayTeamId!.Value].GoalsFor += ag;
                    mini[m.AwayTeamId!.Value].GoalsAgainst += hg;

                    if (hg > ag) mini[m.HomeTeamId!.Value].Points += 3;
                    else if (hg < ag) mini[m.AwayTeamId!.Value].Points += 3;
                    else
                    {
                        mini[m.HomeTeamId!.Value].Points++;
                        mini[m.AwayTeamId!.Value].Points++;
                    }
                }

                var orderedIds = mini.Values
                    .OrderByDescending(m => m.Points)
                    .ThenByDescending(m => m.GoalDifference)
                    .ThenByDescending(m => m.GoalsFor)
                    .Select(m => m.TeamId)
                    .ToList();

                var indexes = group
                    .Select(t => result.FindIndex(r => r.TeamId == t.TeamId))
                    .OrderBy(i => i)
                    .ToList();

                for (int i = 0; i < orderedIds.Count; i++)
                    result[indexes[i]] = sorted.First(t => t.TeamId == orderedIds[i]);
            }

            return result;
        }

        private class MiniStanding
        {
            public Guid TeamId { get; set; }
            public int Points { get; set; }
            public int GoalsFor { get; set; }
            public int GoalsAgainst { get; set; }
            public int GoalDifference => GoalsFor - GoalsAgainst;
        }
    }
}
