using Infra.enums;

namespace Business.DTOs.Teams
{
    public class GetTeamDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
    }

    public class GetAllTeamsResponse
    {
        public List<GetTeamDto> Teams { get; set; } = [];
    }
}
