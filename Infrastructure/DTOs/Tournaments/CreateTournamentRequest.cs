namespace Business.DTOs.Tournaments
{
    public class CreateTournamentRequest
    {
        public string Name { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
