using Business.DTOs.Matches;
using Data.Entities;
using Infra.enums;
using Infra.Interface;
using Infra.ResultWrapper;
using Microsoft.EntityFrameworkCore;

namespace Business.Services.Matches
{
    public class MatchService(IUnitOfWork unitOfWork) : IMatchService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;

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

            var homeTeam = await unitOfWork.Repository<Team>().GetByIdAsync(request.HomeTeamId);
            var awayTeam = await unitOfWork.Repository<Team>().GetByIdAsync(request.AwayTeamId);

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
                HomeTeamName = homeTeam.Name,
                AwayTeamId = match.AwayTeamId,
                AwayTeamName = awayTeam.Name,
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

            return Result<SubmitResultResponse>.Success(new SubmitResultResponse
            {
                MatchId = matchId
            });
        }
    }
}
