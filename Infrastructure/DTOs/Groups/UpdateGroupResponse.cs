namespace Business.DTOs.Groups
{
    public class UpdateGroupResponse
    {
        public string Message { get; set; } = string.Empty;
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid TournamentId { get; set; }
    }
}
