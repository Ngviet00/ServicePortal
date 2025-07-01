using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("roles"), Index(nameof(Id))]
    public class Role
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public ICollection<UserRole>? UserRoles { get; set; } = [];
        public ICollection<RolePermission>? RolePermissions { get; set; } = [];
    }
}
