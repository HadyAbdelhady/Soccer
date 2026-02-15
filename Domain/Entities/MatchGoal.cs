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
        /// <summary>Optional. For REGULAR: player who assisted (same team). For OWNGOAL: player from the other team who caused it.</summary>
        [ForeignKey(nameof(Assister))]
        public Guid? AssisterId { get; set; }
        public Player? Assister { get; set; }
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
