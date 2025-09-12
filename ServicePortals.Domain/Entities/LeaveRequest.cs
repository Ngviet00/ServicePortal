using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("leave_requests"), Index(nameof(Id))]
    public class LeaveRequest
    {
        public Guid Id { get; set; }
        public Guid? ApplicationFormItemId { get; set; }
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
        public int? DepartmentId { get; set; }
        public string? Position { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public int? TypeLeaveId { get; set; }
        public int? TimeLeaveId { get; set; }
        public string? Reason { get; set; }
        public byte[]? Image { get; set; }
        public byte? HaveSalary { get; set; }
        public string? NoteOfHR { get; set; } //ghi chú của HR về đơn nghỉ phép này
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdateAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }


        //relationship
        public ApplicationFormItem? ApplicationFormItem { get; set; } //ApplicationFormItemId
        public OrgUnit? OrgUnit { get; set; } //DepartmentId
        public TimeLeave? TimeLeave { get; set; } //TimeLeaveId
        public TypeLeave? TypeLeave { get; set; } //TypeLeaveId
    }
}
