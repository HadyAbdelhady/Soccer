namespace Business.DTOs.Tournaments
{
    public class AddTeamsToTournamentRequest
    {
        public Guid TournamentId { get; set; }
        public List<Guid> TeamIds { get; set; } = new List<Guid>();
    }
}
