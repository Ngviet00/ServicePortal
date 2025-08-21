namespace ServicePortals.Application.Dtos.Approval.Response
{
    public class PendingApproval
    {
        public Guid? Id { get; set; }
        public int? RequestTypeId { get; set; }
        public int? OrgPositionId { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public Domain.Entities.RequestType? RequestType { get; set; }
        public Domain.Entities.HistoryApplicationForm? HistoryApplicationForm { get; set; }
        public CommonDataPendingApproval? LeaveRequest { get; set; }
        public CommonDataPendingApproval? MemoNotification  { get; set; }
        public CommonDataPendingApproval? ITForm { get; set; }
    }

    public class CommonDataPendingApproval
    {
        public Guid? Id { get; set; }
        public string? Code { get; set; }
        public string? UserNameRequestor { get; set; }
        public string? UserNameCreated { get; set; }
        public string? DepartmentName { get; set; }
    }
}
