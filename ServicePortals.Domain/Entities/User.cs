using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("users"), Index(nameof(UserCode))]
    public class User
    {
        public Guid Id { get; set; }
        public string? UserCode { get; set; }
        public string? Password { get; set; }
        public byte? IsChangePassword { get; set; }
        public byte? IsActive { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTimeOffset? DateOfBirth { get; set; }
        public ICollection<UserRole>? UserRoles { get; set; } = [];
        public ICollection<UserPermission>? UserPermissions { get; set; } = [];
    }
}
