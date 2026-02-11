using Infra.Interface;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    /// <summary>
    /// App user for Admin and Viewer roles. Teams use the Team entity.
    /// </summary>
    [Index(nameof(Username), IsUnique = true)]
    public class User : ISoftDeletableEntity
    {
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string HashedPassword { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Role { get; set; } = string.Empty; // "Admin" or "Viewer"

        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
