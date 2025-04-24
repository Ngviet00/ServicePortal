using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortal.Domain.Entities
{
    [Table("leave_requests"), Index(nameof(Id), nameof(UserCode))]
    public class LeaveRequest
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("user_code")]
        public string? UserCode { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("name_register")]
        public string? NameRegister { get; set; }

        [Column("position")]
        public string? Position { get; set; }

        [Column("deparment")]
        public string? Deparment { get; set; }

        [Column("from_date")]
        public DateTime? FromDate { get; set; }

        [Column("to_date")]
        public DateTime? ToDate { get; set; }

        [Column("reason")]
        public string? Reason { get; set; }

        [Column("time_leave")]
        public byte? TimeLeave { get; set; }

        [Column("type_leave")]
        public byte? TypeLeave { get; set; }

        [Column("status")]
        public byte? Status { get; set; }

        [Column("have_salary")]
        public bool? HaveSalary { get; set; }

        [Column("image")]
        public string? Image { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }
    }
}
