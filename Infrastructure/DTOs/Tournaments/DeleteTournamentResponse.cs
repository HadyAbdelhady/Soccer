namespace Business.DTOs.Tournaments
{
    public class DeleteTournamentResponse
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
