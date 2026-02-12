using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    /// <summary>
    /// Team user with team-specific access and relationships
    /// TPT: Separate table with only Team-specific properties
    /// </summary>
    [Table("TeamUsers")]
    public class TeamUser : BaseUser
    {
        // Team-specific properties
        [ForeignKey(nameof(Group))]
        public Guid? GroupId { get; set; }
        public Group? Group { get; set; }

        // Team relationships
        public ICollection<Player> Players { get; set; } = [];
        public ICollection<Match> HomeMatches { get; set; } = []; 
        public ICollection<Match> AwayMatches { get; set; } = []; 
        public ICollection<Tournament> Tournaments { get; set; } = [];
        
        // EF Core will create separate TeamUsers table with foreign key to BaseUsers
    }
}
