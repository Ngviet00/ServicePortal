using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortal.Domain.Entities
{
    [Table("permissions"), Index(nameof(Id))]
    public class Permission
    {
        [Column("id")]
        public int? Id { get; set; }

        [MaxLength(255), Column("name")]
        public string? Name { get; set; }

        [MaxLength(255), Column("description")]
        public string? Description { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

        public ICollection<UserPermission> UserPermission { get; set; } = new List<UserPermission>();
    }
}
