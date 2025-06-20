using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortal.Domain.Entities
{
    [Table("leave_requests"), Index(nameof(Id), nameof(RequesterUserCode))]
    public class LeaveRequest
    {
        public Guid Id { get; set; }
        public string? RequesterUserCode { get; set; } //nguoi yeu cau
        public string? WriteLeaveUserCode { get; set; } //nguoi viet yeu cau
        public string? WriteLeaveName { get; set; } //ten nguoi viet phep
        public string? Name { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public int? TypeLeave { get; set; }
        public int? TimeLeave { get; set; }
        public string? Reason { get; set; }
        public byte? HaveSalary { get; set; }
        public string? Image { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
    }
}
