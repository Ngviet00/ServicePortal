namespace ServicePortals.Application.Dtos.MemoNotification.Requests
{
    public class ApprovalMemoNotifyRequest
    {
        public Guid? MemoNotificationId { get; set; }
        public string? UserCodeApproval { get; set; }
        public string? UserNameApproval { get; set; }
        public int? OrgUnitId { get; set; }
        public bool? Status { get; set; }
        public string? Note { get; set; }
        public string? UrlFrontend { get; set; }
    }
}
