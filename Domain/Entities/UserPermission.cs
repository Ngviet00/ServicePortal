using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortal.Domain.Entities
{
    [Table("user_permissions"), Index(nameof(UserCode), nameof(PermissionId))]

    public class UserPermission
    {
        [Column("id")]
        public int? Id { get; set; }

        [Column("user_code")]
        public string? UserCode { get; set; }

        [Column("permission_id")]
        public int? PermissionId { get; set; }

        public User? User { get; set; }
        public Permission? Permission { get; set; }
    }
}
