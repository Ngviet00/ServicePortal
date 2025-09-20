using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("user_roles"), Index(nameof(UserCode), nameof(RoleId))]
    public class UserRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(30)]
        public string? UserCode { get; set; }
        public int RoleId { get; set; }
        public User? User { get; set; }
        public Role? Role { get; set; }
    }
}
