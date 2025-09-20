using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("roles"), Index(nameof(Id))]
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string? Name { get; set; }

        [MaxLength(50)]
        public string? Code { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = [];
        public ICollection<RolePermission> RolePermissions { get; set; } = [];
    }
}
