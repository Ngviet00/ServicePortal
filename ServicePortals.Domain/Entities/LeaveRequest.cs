using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("leave_requests"), Index(nameof(Id), nameof(RequesterUserCode))]
    public class LeaveRequest
    {
        public Guid Id { get; set; }
        public Guid? ApplicationFormId { get; set; }
        public string? RequesterUserCode { get; set; }
        public string? UserCodeWriteLeaveRequest { get; set; }
        public string? Name { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public int? TypeLeaveId { get; set; }
        public int? TimeLeaveId { get; set; }
        public string? Reason { get; set; }
        public byte? HaveSalary { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public User? User { get; set; }
        public ApplicationForm? ApplicationForm { get; set; }
        public TimeLeave? TimeLeave { get; set; }
        public TypeLeave? TypeLeave{ get; set; }
    }
}
