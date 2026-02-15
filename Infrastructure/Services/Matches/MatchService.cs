using Business.DTOs.Matches;
using Business.Services.Tournaments;
using Data.Entities;
using Infra.enums;
using Infra.Interface;
using Infra.ResultWrapper;
using Microsoft.EntityFrameworkCore;

namespace Business.Services.Matches
{
    public class MatchService(IUnitOfWork unitOfWork, ITournamentService tournamentService) : IMatchService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;
        private readonly ITournamentService tournamentService = tournamentService;
        

        public async Task<Result<MatchResponse>> CreateMatch(CreateMatchRequest request)
        {
            if (request.HomeTeamId == request.AwayTeamId)
            {
                return Result<MatchResponse>.FailureStatusCode("Home and Away teams must be different", ErrorType.BadRequest);
            }

            var tournament = await unitOfWork.Repository<Tournament>().GetByIdAsync(request.TournamentId);
            if (tournament == null)
            {
                return Result<MatchResponse>.FailureStatusCode("Tournament not found", ErrorType.NotFound);
            }

            if (request.HomeTeamId == null || request.AwayTeamId == null)
            {
                return Result<MatchResponse>.FailureStatusCode("Home and Away teams must be specified for manual match creation", ErrorType.BadRequest);
            }

            var homeTeam = await unitOfWork.Repository<Team>().GetByIdAsync(request.HomeTeamId.Value);
            var awayTeam = await unitOfWork.Repository<Team>().GetByIdAsync(request.AwayTeamId.Value);

            if (homeTeam == null || awayTeam == null)
            {
                return Result<MatchResponse>.FailureStatusCode("One or both teams not found", ErrorType.NotFound);
            }

            // In a real scenario, we'd check if teams belong to the tournament/group
            // but for now we follow the requirement: Initial match status must be "scheduled"
            
            var match = new Match
            {
                Id = Guid.NewGuid(),
                TournamentId = request.TournamentId,
                GroupId = request.GroupId,
                HomeTeamId = request.HomeTeamId,
                AwayTeamId = request.AwayTeamId,
                KickoffTime = request.KickoffTime,
                Status = MatchStatus.SCHEDULED,
                Venue = request.Venue,
                StageType = request.StageType,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await unitOfWork.Repository<Match>().AddAsync(match);
            await unitOfWork.SaveChangesAsync();

            return Result<MatchResponse>.Success(new MatchResponse
            {
                Id = match.Id,
                TournamentId = match.TournamentId,
                GroupId = match.GroupId,
                HomeTeamId = match.HomeTeamId,
                HomeTeamName = homeTeam?.Name,
                AwayTeamId = match.AwayTeamId,
                AwayTeamName = awayTeam?.Name,
                HomeTeamPlaceholder = match.HomeTeamPlaceholder,
                AwayTeamPlaceholder = match.AwayTeamPlaceholder,
                KickoffTime = match.KickoffTime,
                Venue = match.Venue,
                StageType = match.StageType,
                Status = match.Status,
                Message = "Created Successfully"
            });
        }

        public async Task<Result<SubmitResultResponse>> SubmitMatchResult(Guid matchId, SubmitResultRequest request)
        {
            var match = await unitOfWork.Repository<Match>()
                .GetAll()
                .Include(m => m.Goals)
                .Include(m => m.Cards)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null)
            {
                return Result<SubmitResultResponse>.FailureStatusCode("Match not found", ErrorType.NotFound);
            }

            // Update Status
            match.Status = MatchStatus.FINISHED;
            match.FinalWhistleTime = DateTime.UtcNow;
            match.UpdatedAt = DateTimeOffset.UtcNow;

            // Clear existing if editable
            foreach (var goal in match.Goals.ToList()) unitOfWork.Repository<MatchGoal>().Remove(goal);
            foreach (var card in match.Cards.ToList()) unitOfWork.Repository<MatchCard>().Remove(card);

            // Add Goals
            foreach (var gReq in request.Goals)
            {
                var goal = new MatchGoal
                {
                    Id = Guid.NewGuid(),
                    MatchId = matchId,
                    ScorerId = gReq.ScorerId,
                    TeamId = gReq.TeamId,
                    Minute = gReq.Minute,
                    GoalType = gReq.GoalType,
                    CreatedAt = DateTimeOffset.UtcNow
                };
                await unitOfWork.Repository<MatchGoal>().AddAsync(goal);
            }

            // Add Cards
            foreach (var cReq in request.Cards)
            {
                var card = new MatchCard
                {
                    Id = Guid.NewGuid(),
                    MatchId = matchId,
                    PlayerId = cReq.PlayerId,
                    TeamId = cReq.TeamId,
                    Minute = cReq.Minute,
                    CardType = cReq.CardType,
                    CreatedAt = DateTimeOffset.UtcNow
                };
                // Logic: Second yellow automatically results in a red card can be handled in UI or here
                // If we detect two yellows for same player, we could add a red. 
                // But usually the client sends exactly what happened.
                await unitOfWork.Repository<MatchCard>().AddAsync(card);
            }

            unitOfWork.Repository<Match>().Update(match);
            await unitOfWork.SaveChangesAsync();

            // Resolve placeholders in the tournament
            await tournamentService.ResolvePlaceholders(match.TournamentId);

            return Result<SubmitResultResponse>.Success(new SubmitResultResponse
            {
                MatchId = matchId
            });
        }

        public async Task<Result<SetMatchLineupResponse>> SetMatchLineup(Guid matchId, SetMatchLineupRequest request)
        {
            var match = await unitOfWork.Repository<Match>()
                .GetAll()
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null)
            {
                return Result<SetMatchLineupResponse>.FailureStatusCode("Match not found", ErrorType.NotFound);
            }

            if (request.Players == null || !request.Players.Any())
            {
                return Result<SetMatchLineupResponse>.FailureStatusCode("No lineup players provided", ErrorType.BadRequest);
            }

            // Clear existing lineup entries for this match (soft delete via interceptor)
            var existingLineups = await unitOfWork.Repository<MatchLineup>()
                .Find(l => l.MatchId == matchId)
                .ToListAsync();

            if (existingLineups.Any())
            {
                unitOfWork.Repository<MatchLineup>().RemoveRange(existingLineups);
            }

            foreach (var p in request.Players)
            {
                var lineup = new MatchLineup
                {
                    Id = Guid.NewGuid(),
                    MatchId = matchId,
                    TeamId = p.TeamId,
                    PlayerId = p.PlayerId,
                    IsStarting = p.IsStarting,
                    ShirtNumber = p.ShirtNumber,
                    Position = p.Position,
                    CreatedAt = DateTimeOffset.UtcNow
                };

                await unitOfWork.Repository<MatchLineup>().AddAsync(lineup);
            }

            await unitOfWork.SaveChangesAsync();

            var starting = request.Players.Count(p => p.IsStarting);
            var bench = request.Players.Count - starting;

            var response = new SetMatchLineupResponse
            {
                MatchId = matchId,
                StartingCount = starting,
                BenchCount = bench
            };

            return Result<SetMatchLineupResponse>.Success(response);
        }

        public async Task<Result<UpdateMatchScheduleResponse>> UpdateMatchSchedule(Guid matchId, UpdateMatchScheduleRequest request)
        {
            var match = await unitOfWork.Repository<Match>().GetByIdAsync(matchId);

            if (match == null)
            {
                return Result<UpdateMatchScheduleResponse>.FailureStatusCode("Match not found", ErrorType.NotFound);
            }

            if (request.KickoffTime.HasValue)
            {
                match.KickoffTime = request.KickoffTime;
            }

            if (request.Venue != null)
            {
                match.Venue = request.Venue;
            }

            match.UpdatedAt = DateTimeOffset.UtcNow;

            unitOfWork.Repository<Match>().Update(match);
            await unitOfWork.SaveChangesAsync();

            var response = new UpdateMatchScheduleResponse
            {
                Id = match.Id,
                KickoffTime = match.KickoffTime,
                Venue = match.Venue,
                Message = "Match schedule updated successfully"
            };

            return Result<UpdateMatchScheduleResponse>.Success(response);
        }

        public async Task<Result<GetMatchLineupResponse>> GetMatchLineup(Guid matchId)
        {
            var match = await unitOfWork.Repository<Match>()
                .GetAll()
                .Include(m => m.Lineups)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null)
            {
                return Result<GetMatchLineupResponse>.FailureStatusCode("Match not found", ErrorType.NotFound);
            }

            var players = match.Lineups
                .Select(l => new LineupPlayerDto
                {
                    PlayerId = l.PlayerId,
                    TeamId = l.TeamId,
                    IsStarting = l.IsStarting,
                    ShirtNumber = l.ShirtNumber,
                    Position = l.Position
                })
                .ToList();

            var response = new GetMatchLineupResponse
            {
                MatchId = matchId,
                Players = players
            };

            return Result<GetMatchLineupResponse>.Success(response);
        }

        public async Task<Result<GetMatchLineupResponse>> GetMatchLineupForTeam(Guid matchId, Guid teamId)
        {
            var match = await unitOfWork.Repository<Match>()
                .GetAll()
                .Include(m => m.Lineups)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null)
            {
                return Result<GetMatchLineupResponse>.FailureStatusCode("Match not found", ErrorType.NotFound);
            }

            var players = match.Lineups
                .Where(l => l.TeamId == teamId)
                .Select(l => new LineupPlayerDto
                {
                    PlayerId = l.PlayerId,
                    TeamId = l.TeamId,
                    IsStarting = l.IsStarting,
                    ShirtNumber = l.ShirtNumber,
                    Position = l.Position
                })
                .ToList();

            var response = new GetMatchLineupResponse
            {
                MatchId = matchId,
                Players = players
            };

            return Result<GetMatchLineupResponse>.Success(response);
        }

        public async Task<Result<GetAllMatchesResponse>> GetAllMatches(DateTime? date = null, Guid? teamId = null)
        {
            IQueryable<Match> query = unitOfWork.Repository<Match>()
                .GetAll()
                .Include(m => m.Tournament)
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam);

            if (date.HasValue)
            {
                var dayStart = date.Value.Date;
                var dayEnd = dayStart.AddDays(1);
                query = query.Where(m => m.KickoffTime >= dayStart && m.KickoffTime < dayEnd);
            }

            if (teamId.HasValue)
            {
                var id = teamId.Value;
                query = query.Where(m => m.HomeTeamId == id || m.AwayTeamId == id);
            }

            var matches = await query
                .OrderBy(m => m.TournamentId)
                .ThenBy(m => m.KickoffTime)
                .ToListAsync();

            var tournaments = matches
                .GroupBy(m => new { m.TournamentId, TournamentName = m.Tournament?.Name ?? "" })
                .Select(g => new TournamentWithMatchesDto
                {
                    TournamentId = g.Key.TournamentId,
                    TournamentName = g.Key.TournamentName,
                    Matches = g.Select(m => new MatchResponse
                    {
                        Id = m.Id,
                        TournamentId = m.TournamentId,
                        GroupId = m.GroupId,
                        HomeTeamId = m.HomeTeamId,
                        HomeTeamName = m.HomeTeam != null ? m.HomeTeam.FullName : m.HomeTeamPlaceholder,
                        AwayTeamId = m.AwayTeamId,
                        AwayTeamName = m.AwayTeam != null ? m.AwayTeam.FullName : m.AwayTeamPlaceholder,
                        HomeTeamPlaceholder = m.HomeTeamPlaceholder,
                        AwayTeamPlaceholder = m.AwayTeamPlaceholder,
                        KickoffTime = m.KickoffTime,
                        Venue = m.Venue,
                        StageType = m.StageType,
                        Status = m.Status
                    }).ToList()
                })
                .ToList();

            return Result<GetAllMatchesResponse>.Success(new GetAllMatchesResponse { Tournaments = tournaments });
        }
    }
}
