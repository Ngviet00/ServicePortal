using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("permission_groups")]
    public class PermissionGroup
    {
        public int? Id { get; set; }
        [StringLength(50)]
        public string? GroupName { get; set; }
        public string? Description { get; set; }
        public ICollection<Permission>? Permissions { get; set; } = [];
    }
}
