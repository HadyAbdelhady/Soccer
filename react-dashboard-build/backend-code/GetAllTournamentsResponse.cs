using Infra.enums;

namespace Business.DTOs.Tournaments
{
    public class GetTournamentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public TournamentType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class GetAllTournamentsResponse
    {
        public List<GetTournamentDto> Tournaments { get; set; } = [];
    }
}
