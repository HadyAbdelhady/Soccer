using System.Text.Json.Serialization;

namespace Business.DTOs.Groups
{
    public class TournamentGroupsResponseDto
    {
        public Guid TournamentId { get; set; }
        public List<GroupResponseDto> Groups { get; set; } = [];
    }

    public class GroupResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<TeamStandingDto> Standings { get; set; } = [];
    }
}
