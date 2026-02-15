using Infra.enums;
using Infra.Interface;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    /// <summary>
    /// Base user entity for TPT (Table-Per-Type) inheritance
    /// Each derived type will have its own table with only specific properties
    /// </summary>
    [Index(nameof(Username), IsUnique = true)]
    public abstract class BaseUser : ISoftDeletableEntity
    {
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string HashedPassword { get; set; } = string.Empty;

        /// <summary>FCM device token for push notifications.</summary>
        [MaxLength(512)]
        public string? FcmToken { get; set; }

        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
