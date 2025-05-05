using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortal.Domain.Entities
{
    [Table("users"), Index(nameof(Code), nameof(Email), nameof(Id))]
    public class User
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("code")]
        public string? Code { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("password")]
        public string? Password { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("is_active")]
        public bool? IsActive { get; set; }

        [Column("date_join_company")]
        public DateTime? DateJoinCompany{ get; set; }

        [Column("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }

        [Column("phone")]
        public string? Phone{ get; set; }

        [Column("sex")]
        public byte? Sex { get; set; } //1 male - nam, 2 female - nu

        [Column("position")]
        public string? Position { get; set; }

        [Column("department_id")]
        public int? DepartmentId { get; set; }

        [Column("level")]
        public string? Level { get; set; }

        [Column("level_parent")]
        public string? LevelParent { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        public Department? Department { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<UserPermission> UserPermission { get; set; } = new List<UserPermission>();
    }
}
