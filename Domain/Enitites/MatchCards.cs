using Data.enums;
using Data.Interface;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Enitites
{
    public class MatchCard : ISoftDeletableEntity
    {
        public Guid Id { get; set; }
        public CardType CardType { get; set; }
        [ForeignKey(nameof(Player))]
        public Guid PlayerId { get; set; }
        public Player Player { get; set; } = null!;

        [ForeignKey(nameof(Match))]
        public Guid MatchId { get; set; }
        public Match Match { get; set; } = null!;
        public DateTime IncidentTime { get; set; }
        public bool IsDeleted { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
