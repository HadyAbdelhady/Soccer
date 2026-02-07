using Data.Interface;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Data.Enitites
{
    [Index(nameof(Username), IsUnique = true)] // Ensure unique usernames at the database level
    public class Team : ISoftDeletableEntity
    {
        public Guid Id { get; set; }
        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(255)] // Store BCrypt/Argon2 hash ONLY
        public string HashedPassword { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        // Existing team properties (Name, LogoUrl, etc.)

        // NAVIGATION PROPERTIES (REMOVE TournamentId/MatchId!)
        public ICollection<Player> Players { get; set; } = [];
        public ICollection<Match> HomeMatches { get; set; } = []; // Was "Team A"
        public ICollection<Match> AwayMatches { get; set; } = []; // Was "Team B"
        public ICollection<Tournament> Tournaments { get; set; } = []; // Many-to-Many
    }
}
