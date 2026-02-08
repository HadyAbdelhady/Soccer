namespace Business.DTOs.Tournaments
{
    public class UpdateTournamentResponse
    {
        public string Message { get; set; } = string.Empty;
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
