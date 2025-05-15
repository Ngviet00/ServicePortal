using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortal.Domain.Entities
{
    [Table("approval_actions"), Index(nameof(Id), nameof(ApprovalRequestId), nameof(ApproverUserCode))]
    public class ApprovalAction
    {
        public Guid Id { get; set; }
        public Guid? ApprovalRequestId { get; set; } //khóa ngoại approval request id
        public string? ApproverUserCode { get; set; } //mã người duyệt,
        public string? ApproverName { get; set; } //tên người duyệt,
        public string? Action { get; set; } //Approved, Rejected
        public string? Comment { get; set; } //ghi chú
        public DateTimeOffset? CreatedAt { get; set; }
    }
}
