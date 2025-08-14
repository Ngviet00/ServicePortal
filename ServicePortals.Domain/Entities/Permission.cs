using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("permissions"), Index(nameof(Id))]
    public class Permission
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Group { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; } = [];
        public ICollection<UserPermission> UserPermissions { get; set; } = [];
    }
}
