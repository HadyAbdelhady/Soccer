namespace Business.DTOs.Tournaments
{
    public class RegenerateGroupsResponse
    {
        public Guid TournamentId { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<GeneratedGroupDto> Groups { get; set; } = [];
    }
}
