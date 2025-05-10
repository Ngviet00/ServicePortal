using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortal.Domain.Entities
{
    [Table("leave_request_steps"), Index(nameof(Id), nameof(LeaveRequestId), nameof(UserCodeApprover), nameof(StatusStep))]
    public class LeaveRequestStep
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("leave_request_id")]
        public Guid? LeaveRequestId { get; set; }

        [Column("user_code_approver")]
        public string? UserCodeApprover { get; set; }

        [Column("status_step")]
        public byte? StatusStep { get; set; } //1:pending, 2:approval, 3;reject 1,2

        [Column("level_approval"), MaxLength(50)]
        public string? LevelApproval { get; set; }

        [Column("note"), MaxLength(255)]
        public string? Note { get; set; }

        [Column("approved_by")]
        public string? ApprovedBy { get; set; }

        [Column("approved_at")]
        public DateTime? ApprovedAt { get; set; }
    }
}
