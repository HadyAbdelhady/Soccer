using Infra.enums;
using Infra.Interface;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class MatchCard : ISoftDeletableEntity
    {
        public Guid Id { get; set; }
        public CardType CardType { get; set; }
        [ForeignKey(nameof(Player))]
        public Guid PlayerId { get; set; }
        public Player Player { get; set; } = null!;

        [ForeignKey(nameof(Team))]
        public Guid TeamId { get; set; }
        public TeamUser Team { get; set; } = null!;

        [ForeignKey(nameof(Match))]
        public Guid MatchId { get; set; }
        public Match Match { get; set; } = null!;
        public int Minute { get; set; }
        public bool IsDeleted { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
