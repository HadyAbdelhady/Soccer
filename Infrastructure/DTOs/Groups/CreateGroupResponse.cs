namespace Business.DTOs.Groups
{
    public class CreateGroupResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid TournamentId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
