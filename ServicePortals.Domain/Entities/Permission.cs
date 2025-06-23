using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("permissions"), Index(nameof(Id))]
    public class Permission
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
