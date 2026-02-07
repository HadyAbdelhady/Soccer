using Data.Interface;
using System.ComponentModel.DataAnnotations;

namespace Data.Enitites
{
    public class Tournament : ISoftDeletableEntity
    {
        public Guid Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        // NAVIGATION (NO TeamBId here!)
        public ICollection<Match> Matches { get; set; } = [];
        public ICollection<Team> Teams { get; set; } = [];
        public ICollection<Group> Groups { get; set; } = []; 

        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
