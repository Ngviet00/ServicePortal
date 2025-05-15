using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortal.Domain.Entities
{
    [Table("leave_requests"), Index(nameof(Id), nameof(RequesterUserCode))]
    public class LeaveRequest
    {
        public Guid Id { get; set; }
        public string? RequesterUserCode { get; set; } //nguoi yeu cau
        public string? UserCodeWriteLeave { get; set; } //nguoi viet yeu cau
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public string? TypeLeave { get; set; }
        public string? TimeLeave { get; set; }
        public string? Reason { get; set; }
        public byte? HaveSalary { get; set; }
        public string? Image { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
    }
}
