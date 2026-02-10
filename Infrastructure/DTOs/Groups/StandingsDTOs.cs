using Infra.enums;

namespace Business.DTOs.Groups
{
    public class TeamStandingDto
    {
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = null!;
        public int MatchesPlayed { get; set; }
        public int HomeMatchesPlayed { get; set; }
        public int AwayMatchesPlayed { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifference => GoalsFor - GoalsAgainst;
        public int Points { get; set; }
        public int YellowCards { get; set; }
        public int RedCards { get; set; }
        public int FairPlayScore { get; set; } // Points based on card weight
        public int Rank { get; set; }
    }

    public class PlayerStandingDto
    {
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; } = null!;
        public string TeamName { get; set; } = null!;
        public int Goals { get; set; }
        public int YellowCards { get; set; }
        public int RedCards { get; set; }
    }

    public class GroupStandingsResponse
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; } = null!;
        public List<TeamStandingDto> Standings { get; set; } = [];
        public List<PlayerStandingDto> TopScorers { get; set; } = [];
        public List<PlayerStandingDto> MostCards { get; set; } = [];
    }
}
