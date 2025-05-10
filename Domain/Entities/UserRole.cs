using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortal.Domain.Entities
{
    [Table("user_roles"), Index(nameof(UserCode), nameof(RoleId), nameof(DepartmentId))]
    public class UserRole
    {
        [Column("id")]
        public Guid? Id { get; set; }

        [Column("user_code")]
        public string? UserCode { get; set; }

        [Column("role_id")]
        public int? RoleId { get; set; }

        [Column("department_id")]
        public int? DepartmentId { get; set; }

        public User? User { get; set; }
        public Role? Role { get; set; }
        public Department? Department { get; set; }
    }
}
