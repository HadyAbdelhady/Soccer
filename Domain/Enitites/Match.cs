using Data.Interface;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Enitites
{
    public class Match : ISoftDeletableEntity
    {
        public Guid Id { get; set; }

        [ForeignKey(nameof(HomeTeam))]
        public Guid HomeTeamId { get; set; }
        public Team HomeTeam { get; set; } = null!;

        [ForeignKey(nameof(AwayTeam))]
        public Guid AwayTeamId { get; set; }
        public Team AwayTeam { get; set; } = null!;

        public DateTime KickoffTime { get; set; }
        public DateTime? FinalWhistleTime { get; set; }

        [ForeignKey(nameof(Tournament))]
        public Guid TournamentId { get; set; }
        public Tournament Tournament { get; set; } = null!;

        // NAVIGATION
        public ICollection<MatchCard> Cards { get; set; } = [];
        public ICollection<MatchGoal> Goals { get; set; } = [];
        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
