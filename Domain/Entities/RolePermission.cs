using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortal.Domain.Entities
{
    [Table("role_permissions"), Index(nameof(RoleId), nameof(PermissionId))]
    public class RolePermission
    {
        public int? Id { get; set; }
        public int? RoleId { get; set; }
        public int? PermissionId {  get; set; }
        public Role? Role { get; set; }
        public Permission? Permission { get; set; }
    }
}
