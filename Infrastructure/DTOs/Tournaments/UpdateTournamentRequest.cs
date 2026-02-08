namespace Business.DTOs.Tournaments
{
    public class UpdateTournamentRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
