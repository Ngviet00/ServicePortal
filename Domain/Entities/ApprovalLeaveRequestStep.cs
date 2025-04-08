using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortal.Domain.Entities
{
    [Table("approval_leave_request_steps")]
    public class ApprovalLeaveRequestStep
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("leave_request_id")]
        public Guid? LeaveRequestId { get; set; }

        [Column("user_code_approver")]
        public string? UserCodeApprover { get; set; }

        [Column("order")]
        public int? Order { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("position")]
        public string? Position { get; set; }

        [Column("deparment")]
        public string? Deparment { get; set; }

        [Column("note"), MaxLength(255)]
        public string? Note { get; set; }

        [Column("approved_at")]
        public DateTime? ApprovedAt { get; set; }
    }
}
