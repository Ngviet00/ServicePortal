namespace ServicePortals.Application.Dtos.LeaveRequest.Requests
{
    public class ApprovalRequests
    {
        public string? UserCodeApproval { get; set; }
        public string? NameUserApproval { get; set; }
        public string? LeaveRequestId { get; set; }
        public bool Status { get; set; } //true approval, false reject
        public string? Note { get; set; }
        public string? UrlFrontEnd { get; set; }
    }
}
