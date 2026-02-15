namespace Business.DTOs.Tournaments
{
    /// <summary>Per-player statistics within a tournament.</summary>
    public class TournamentPlayerStatsDto
    {
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string? TeamName { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int RedCards { get; set; }
        public int YellowCards { get; set; }
        public int MatchesPlayed { get; set; }
    }
}
