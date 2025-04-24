using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortal.Domain.Entities
{
    [Table("leave_request_steps"), Index(nameof(Id), nameof(LeaveRequestId), nameof(PositionIdApproval), nameof(StatusStep), nameof(CodeApprover))]
    public class LeaveRequestStep
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("leave_request_id")]
        public Guid? LeaveRequestId { get; set; }

        [Column("position_id_approval")]
        public int? PositionIdApproval { get; set; } //position approver, exam: Staff IT write leaver, position 2, => get position 1 Manager IT

        [Column("status_step")]
        public byte? StatusStep{ get; set; } //1:pending, 2:complete, 3;reject 1,2

        [Column("approved_by")]
        public string? ApprovedBy { get; set; }

        [Column("code_approver")]
        public string? CodeApprover { get; set; }

        [Column("note"), MaxLength(255)]
        public string? Note { get; set; }

        [Column("approved_at")]
        public DateTime? ApprovedAt { get; set; }
    }
}
