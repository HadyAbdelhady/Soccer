namespace Business.DTOs.Tournaments
{
    public class GenerateTournamentGroupsResponse
    {
        public Guid TournamentId { get; set; }
        public List<GeneratedGroupDto> Groups { get; set; } = [];
        public string Message { get; set; } = string.Empty;
    }

    public class GeneratedGroupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TeamCount { get; set; }
    }
}

