namespace Business.DTOs.Tournaments
{
    public class AddTeamToTournamentResponse
    {
        public Guid TournamentId { get; set; }
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
