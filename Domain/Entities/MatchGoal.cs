using System.ComponentModel.DataAnnotations.Schema;
using Infra.enums;
using Infra.Interface;

namespace Data.Entities
{
    public class MatchGoal : ISoftDeletableEntity
    {

        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid ScorerId { get; set; }
        public Player Scorer { get; set; } = null!;
        [ForeignKey(nameof(Team))]
        public Guid TeamId { get; set; }
        public TeamUser Team { get; set; } = null!;

        [ForeignKey(nameof(Match))]
        public Guid MatchId { get; set; }
        public Match Match { get; set; } = null!;
        public int Minute { get; set; }
        public GoalType GoalType { get; set; }
        public bool IsDeleted { get; set; }
    }
}
