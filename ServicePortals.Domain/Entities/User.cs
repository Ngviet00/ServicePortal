using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("users"), Index(nameof(UserCode), nameof(Id))]
    public class User
    {
        public Guid Id { get; set; }
        public string? UserCode { get; set; }
        public string? Password { get; set; }
        public int? PositionId { get; set; }
        public byte? IsChangePassword { get; set; }
        public byte? IsActive { get; set; }
        public string? Email { get; set; }
        public int? Nationality { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
