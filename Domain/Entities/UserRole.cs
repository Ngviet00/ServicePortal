using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortal.Domain.Entities
{
    [Table("user_roles"), Index(nameof(UserCode), nameof(RoleId))]
    public class UserRole
    {
        public Guid Id { get; set; }
        public string? UserCode { get; set; }
        public int? RoleId { get; set; }
        public User? User { get; set; }
        public Role? Role { get; set; }
    }
}
