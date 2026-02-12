using Infra.Interface;
using Infra.enums;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class Tournament : ISoftDeletableEntity
    {
        public Guid Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        public TournamentType Type { get; set; }
        public LegsType Legs { get; set; }
        public int? GroupCount { get; set; }
        public int? TeamsToAdvance { get; set; }

        public ICollection<Match> Matches { get; set; } = [];
        public ICollection<TeamUser> Teams { get; set; } = [];
        public ICollection<Group> Groups { get; set; } = []; 

        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
