using Data.Interface;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Enitites
{
    public class Group : ISoftDeletableEntity
    {
        public Guid Id { get; set; }

        [Required, MaxLength(10)]
        public string Name { get; set; } = string.Empty;

        [ForeignKey(nameof(Tournament))]
        public Guid TournamentId { get; set; }
        public Tournament Tournament { get; set; } = null!;
        public ICollection<Match> Matches { get; set; } = [];

        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
