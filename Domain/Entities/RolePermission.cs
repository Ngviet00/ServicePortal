using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortal.Domain.Entities
{
    [Table("role_permissions"), Index(nameof(RoleId), nameof(PermissionId))]
    public class RolePermission
    {
        [Column("id")]
        public int? Id { get; set; }

        [Column("role_id")]
        public int? RoleId { get; set; }

        [Column("permission_id")]
        public int? PermissionId {  get; set; }

        public Role? Role { get; set; }
        public Permission? Permission { get; set; }
    }
}
