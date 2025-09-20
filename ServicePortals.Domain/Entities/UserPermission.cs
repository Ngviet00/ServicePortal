using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("user_permissions"), Index(nameof(UserCode), nameof(PermissionId))]
    public class UserPermission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(30)]
        public string? UserCode { get; set; }
        public int PermissionId { get; set; }
        public User? User { get; set; }
        public Permission? Permission { get; set; }
    }
}
