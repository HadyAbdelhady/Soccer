namespace Business.DTOs.Tournaments
{
    public class AddTeamToTournamentRequest
    {
        public Guid TournamentId { get; set; }
        public Guid TeamId { get; set; }
    }
}
