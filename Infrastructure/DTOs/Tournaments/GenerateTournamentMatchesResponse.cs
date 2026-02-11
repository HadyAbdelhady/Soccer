namespace Business.DTOs.Tournaments
{
    public class GenerateTournamentMatchesResponse
    {
        public Guid TournamentId { get; set; }
        public int CreatedMatchCount { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

