using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    /// <summary>
    /// Admin user with full system access
    /// TPT: Separate table with only Admin-specific properties
    /// </summary>
    [Table("AdminUsers")]
    public class AdminUser : BaseUser
    {
        // Admin-specific properties can be added here
        // For now, inherits all base properties
        
        // EF Core will create separate AdminUsers table with foreign key to BaseUsers
    }
}
