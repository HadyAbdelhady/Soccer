using Infra.enums;

namespace Business.DTOs.Tournaments
{
    public class CreateTournamentRequest
    {
        public string Name { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TournamentType Type { get; set; }
        public LegsType Legs { get; set; }
        public int? GroupCount { get; set; }
        public int? TeamsToAdvance { get; set; }
        public List<Guid> TeamIds { get; set; } = [];
    }
}
