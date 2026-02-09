namespace Business.DTOs.Groups
{
    public class CreateGroupRequest
    {
        public string Name { get; set; } = null!;
        public Guid TournamentId { get; set; }
    }
}
