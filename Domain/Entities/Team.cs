using Infra.Interface;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    [Index(nameof(Username), IsUnique = true)] 
    public class Team : ISoftDeletableEntity
    {
        public Guid Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(255)] 
        public string HashedPassword { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }


        // NAVIGATION PROPERTIES
        public ICollection<Player> Players { get; set; } = [];
        public ICollection<Match> HomeMatches { get; set; } = []; 
        public ICollection<Match> AwayMatches { get; set; } = []; 
        public ICollection<Tournament> Tournaments { get; set; } = []; // Many-to-Many
    }
}
