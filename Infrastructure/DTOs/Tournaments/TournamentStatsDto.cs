namespace Business.DTOs.Tournaments
{
    /// <summary>Overall tournament statistics (sum of goals, assists, cards, match count).</summary>
    public class TournamentStatsDto
    {
        public Guid TournamentId { get; set; }
        public string TournamentName { get; set; } = string.Empty;
        public int TotalGoals { get; set; }
        public int TotalAssists { get; set; }
        public int RedCards { get; set; }
        public int YellowCards { get; set; }
        public int MatchesCount { get; set; }
    }
}
