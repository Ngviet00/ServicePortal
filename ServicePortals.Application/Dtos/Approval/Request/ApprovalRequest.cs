using System.ComponentModel.DataAnnotations;

namespace ServicePortals.Application.Dtos.Approval.Request
{
    public class ApprovalRequest
    {
        [Required]
        public int RequestTypeId { get; set; }
        public long ApplicationFormId { get; set; }
        public string? ApplicationFormCode { get; set; }
        public string? UserCodeApproval { get; set; }
        public string? UserNameApproval { get; set; }
        public int OrgPositionId { get; set; }
        public bool Status { get; set; }
        public string? Note { get; set; }
    }
}
