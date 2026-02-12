using Infra.Interface;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    /// <summary>
    /// Represents a player's participation in a specific match (per‑match lineup).
    /// </summary>
    public class MatchLineup : ISoftDeletableEntity
    {
        public Guid Id { get; set; }

        [ForeignKey(nameof(Match))]
        public Guid MatchId { get; set; }
        public Match Match { get; set; } = null!;

        [ForeignKey(nameof(Team))]
        public Guid TeamId { get; set; }
        public Team Team { get; set; } = null!;

        [ForeignKey(nameof(Player))]
        public Guid PlayerId { get; set; }
        public Player Player { get; set; } = null!;

        public int? ShirtNumber { get; set; }
        public string? Position { get; set; } = null!;

        /// <summary>
        /// True if player is in the starting lineup, false if on the bench.
        /// </summary>
        public bool IsStarting { get; set; }

        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}

