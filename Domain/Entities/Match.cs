using Infra.Interface;
using Infra.enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class Match : ISoftDeletableEntity
    {
        public Guid Id { get; set; }

        [ForeignKey(nameof(HomeTeam))]
        public Guid? HomeTeamId { get; set; }
        public Team? HomeTeam { get; set; }

        [ForeignKey(nameof(AwayTeam))]
        public Guid? AwayTeamId { get; set; }
        public Team? AwayTeam { get; set; }

        public string? HomeTeamPlaceholder { get; set; }
        public string? AwayTeamPlaceholder { get; set; }

        public int RoundNumber { get; set; }

        public DateTime KickoffTime { get; set; }
        public DateTime? FinalWhistleTime { get; set; }

        [ForeignKey(nameof(Tournament))]
        public Guid TournamentId { get; set; }
        public Tournament Tournament { get; set; } = null!;

        [ForeignKey(nameof(Group))]
        public Guid? GroupId { get; set; }
        public Group? Group { get; set; }
        public MatchStatus Status { get; set; }
        public string? Venue { get; set; }
        public StageType StageType { get; set; }

        public ICollection<MatchCard> Cards { get; set; } = [];
        public ICollection<MatchGoal> Goals { get; set; } = [];
        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
