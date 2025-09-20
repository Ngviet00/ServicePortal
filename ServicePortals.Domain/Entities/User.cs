using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("users"), Index(nameof(UserCode))]
    public class User
    {
        public Guid Id { get; set; }

        [MaxLength(30)]
        public string? UserCode { get; set; }
        public string? Password { get; set; }
        public bool IsChangePassword { get; set; } = false;
        public bool IsActive { get; set; } = true;

        [MaxLength(50)]
        public string? Email { get; set; }
        [MaxLength(50)]
        public string? Phone { get; set; }
        public DateTimeOffset? DateOfBirth { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = [];
        public ICollection<UserPermission> UserPermissions { get; set; } = [];
        public ICollection<UserConfig> UserConfigs { get; set; } = [];
    }
}
