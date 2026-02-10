using Infra.enums;
using Infra.Interface;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class Player : ISoftDeletableEntity
    {
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        [Required, MaxLength(10)]
        public string NickName { get; set; } = string.Empty;

        public PlayerPosition Position { get; set; }

        public int JerseyNumber { get; set; }
        public bool IsCaptain { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(Team))]
        public Guid TeamId { get; set; }
        public Team Team { get; set; } = null!;
        public ICollection<MatchCard> Cards { get; set; } = [];
        public ICollection<MatchGoal> Goals { get; set; } = [];
    }
}
