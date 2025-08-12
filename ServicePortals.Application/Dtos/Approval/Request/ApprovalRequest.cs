namespace ServicePortals.Application.Dtos.Approval.Request
{
    public class ApprovalRequest
    {
        public int? RequestTypeId { get; set; } //để biết là loại đơn nào
        public Guid? MemoNotificationId { get; set; }
        public string? UserCodeApproval { get; set; }
        public string? UserNameApproval { get; set; }
        public int? OrgUnitId { get; set; }
        public bool? Status { get; set; }
        public string? Note { get; set; }
        public string? UrlFrontend { get; set; }
    }
}
