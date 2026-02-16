using Infra.enums;

namespace Business.DTOs.Tournaments
{
    public class GetTournamentWithTeamCountDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public TournamentType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TeamCount { get; set; }
    }

    public class GetAllTournamentsWithTeamCountResponse
    {
        public List<GetTournamentWithTeamCountDto> Tournaments { get; set; } = [];
    }
}
