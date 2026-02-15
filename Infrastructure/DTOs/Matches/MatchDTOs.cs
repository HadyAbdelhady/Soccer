using System.Text.Json.Serialization;
using Infra.enums;

namespace Business.DTOs.Matches
{
    public class CreateMatchRequest
    {
        public Guid TournamentId { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? HomeTeamId { get; set; }
        public Guid? AwayTeamId { get; set; }
        public DateTime KickoffTime { get; set; }
        public string? Venue { get; set; }
        public StageType StageType { get; set; }
    }

    public class MatchResponse
    {
        public Guid Id { get; set; }
        public Guid TournamentId { get; set; }
        /// <summary>Tournament name (included when returning all matches).</summary>
        public string? TournamentName { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? HomeTeamId { get; set; }
        public string? HomeTeamName { get; set; }
        public Guid? AwayTeamId { get; set; }
        public string? AwayTeamName { get; set; }
        public string? HomeTeamPlaceholder { get; set; }
        public string? AwayTeamPlaceholder { get; set; }
        public DateTime? KickoffTime { get; set; }
        public string? Venue { get; set; }
        public StageType StageType { get; set; }
        public MatchStatus Status { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>Tournament with its matches (for get-all-matches response).</summary>
    public class TournamentWithMatchesDto
    {
        public Guid TournamentId { get; set; }
        public string TournamentName { get; set; } = string.Empty;
        public List<MatchResponse> Matches { get; set; } = [];
    }

    public class GetAllMatchesResponse
    {
        public List<TournamentWithMatchesDto> Tournaments { get; set; } = [];
    }

    public class UpdateMatchScheduleRequest
    {
        public DateTime? KickoffTime { get; set; }
        public string? Venue { get; set; }
    }

    public class UpdateMatchScheduleResponse
    {
        public Guid Id { get; set; }
        public DateTime? KickoffTime { get; set; }
        public string? Venue { get; set; }
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
        /// <summary>Optional. For REGULAR: same-team assister. For OWNGOAL: player from the other team who caused the own goal. Not used for PENALTY or FOUL.</summary>
        public Guid? AssisterId { get; set; }
        public int Minute { get; set; }
        public GoalType GoalType { get; set; }
    }

    public class CardRequest
    {
        public Guid TeamId { get; set; }
        public Guid PlayerId { get; set; }
        /// <summary>Alias for PlayerId. If client sends "scorerId" instead of "playerId", it is accepted.</summary>
        [JsonPropertyName("scorerId")]
        public Guid ScorerId { get; set; }
        public int Minute { get; set; }
        public CardType CardType { get; set; }
    }

    public class SubmitResultResponse
    {
        public Guid MatchId { get; set; }
        public string Message { get; set; } = "Created Successfully";
    }

    public class AddGoalResponse
    {
        public Guid MatchId { get; set; }
        public Guid GoalId { get; set; }
        public string Message { get; set; } = "Goal added.";
    }

    public class AddCardResponse
    {
        public Guid MatchId { get; set; }
        public Guid CardId { get; set; }
        public string Message { get; set; } = "Card added.";
    }

    public class LineupPlayerDto
    {
        public Guid PlayerId { get; set; }
        public Guid TeamId { get; set; }
        public bool IsStarting { get; set; }
        public int? ShirtNumber { get; set; }
        public string? Position { get; set; }
    }

    public class SetMatchLineupRequest
    {
        public List<LineupPlayerDto> Players { get; set; } = [];
    }

    public class SetMatchLineupResponse
    {
        public Guid MatchId { get; set; }
        public int StartingCount { get; set; }
        public int BenchCount { get; set; }
        public string Message { get; set; } = "Lineup set successfully";
    }

    public class GetMatchLineupResponse
    {
        public Guid MatchId { get; set; }
        public List<LineupPlayerDto> Players { get; set; } = [];
    }
}
