using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("leave_requests"), Index(nameof(Id))]
    public class LeaveRequest
    {
        public Guid Id { get; set; }
        public Guid? ApplicationFormId { get; set; }
        public int? DepartmentId { get; set; }
        public string? Position { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public int? TypeLeaveId { get; set; }
        public int? TimeLeaveId { get; set; }
        public string? Reason { get; set; }
        public byte[]? Image { get; set; }
        public byte? HaveSalary { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdateAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public User? User { get; set; }
        public ApplicationForm? ApplicationForm { get; set; }
        public TimeLeave? TimeLeave { get; set; }
        public TypeLeave? TypeLeave{ get; set; }
        public OrgUnit? OrgUnit { get; set; } //foreign key departmentId
    }
}
