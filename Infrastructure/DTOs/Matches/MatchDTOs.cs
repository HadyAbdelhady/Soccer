using Infra.enums;

namespace Business.DTOs.Matches
{
    public class CreateMatchRequest
    {
        public Guid TournamentId { get; set; }
        public Guid? GroupId { get; set; }
        public Guid HomeTeamId { get; set; }
        public Guid AwayTeamId { get; set; }
        public DateTime KickoffTime { get; set; }
        public string? Venue { get; set; }
        public StageType StageType { get; set; }
    }

    public class MatchResponse
    {
        public Guid Id { get; set; }
        public Guid TournamentId { get; set; }
        public Guid? GroupId { get; set; }
        public Guid HomeTeamId { get; set; }
        public string HomeTeamName { get; set; } = null!;
        public Guid AwayTeamId { get; set; }
        public string AwayTeamName { get; set; } = null!;
        public DateTime KickoffTime { get; set; }
        public string? Venue { get; set; }
        public StageType StageType { get; set; }
        public MatchStatus Status { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class SubmitResultRequest
    {
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
        public List<GoalRequest> Goals { get; set; } = [];
        public List<CardRequest> Cards { get; set; } = [];
    }

    public class GoalRequest
    {
        public Guid TeamId { get; set; }
        public Guid ScorerId { get; set; }
        public int Minute { get; set; }
        public GoalType GoalType { get; set; }
    }

    public class CardRequest
    {
        public Guid TeamId { get; set; }
        public Guid PlayerId { get; set; }
        public int Minute { get; set; }
        public CardType CardType { get; set; }
    }

    public class SubmitResultResponse
    {
        public Guid MatchId { get; set; }
        public string Message { get; set; } = "Submitted Successfully";
    }
}
