using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("permissions"), Index(nameof(Id))]
    public class Permission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string? Name { get; set; }

        [MaxLength(50)]
        public string? Group { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; } = [];
        public ICollection<UserPermission> UserPermissions { get; set; } = [];
    }
}
