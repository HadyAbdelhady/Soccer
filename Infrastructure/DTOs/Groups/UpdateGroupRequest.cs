namespace Business.DTOs.Groups
{
    public class UpdateGroupRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid TournamentId { get; set; }
    }
}
