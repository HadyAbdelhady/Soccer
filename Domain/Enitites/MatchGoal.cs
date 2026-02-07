using System.ComponentModel.DataAnnotations.Schema;
using Data.enums;
using Data.Interface;

namespace Data.Enitites
{
    public class MatchGoal : ISoftDeletableEntity
    {

        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid ScorerId { get; set; }
        public Player Scorer { get; set; } = null!;

        [ForeignKey(nameof(Match))]
        public Guid MatchId { get; set; }
        public Match Match { get; set; } = null!;
        public DateTime GoalTime { get; set; }
        public GoalType GoalType { get; set; }
        public bool IsDeleted { get; set; }
    }
}
