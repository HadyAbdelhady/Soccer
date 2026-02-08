namespace Business.DTOs.Tournaments
{
    public class CreateTournamentResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
