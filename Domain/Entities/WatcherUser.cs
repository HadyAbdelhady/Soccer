using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    /// <summary>
    /// Watcher user with read-only access
    /// TPT: Separate table with only Watcher-specific properties
    /// </summary>
    [Table("WatcherUsers")]
    public class WatcherUser : BaseUser
    {
        // Watcher-specific properties can be added here
        // For now, inherits all base properties
        
        // EF Core will create separate WatcherUsers table with foreign key to BaseUsers
    }
}
