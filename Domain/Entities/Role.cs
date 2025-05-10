using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortal.Domain.Entities
{
    [Table("roles"), Index(nameof(Id))]
    public class Role
    {
        [Column("id")]
        public int? Id { get; set; }

        [MaxLength(50), Column("name")]
        public string? Name { get; set; }

        [MaxLength(50), Column("code")]
        public string? Code { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
