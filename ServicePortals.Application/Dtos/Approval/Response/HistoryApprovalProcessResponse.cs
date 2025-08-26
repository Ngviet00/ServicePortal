namespace ServicePortals.Application.Dtos.Approval.Response
{
    public class HistoryApprovalProcessResponse
    {
        public DateTimeOffset? CreatedAt { get; set; }
        public string? Action { get; set; }
        public int? RequestStatusId { get; set; }
        public Domain.Entities.RequestType? RequestType { get; set; }
        public CommonDataHistoryApproval? LeaveRequest { get; set; }
        public CommonDataHistoryApproval? MemoNotification { get; set; }
        public CommonDataHistoryApproval? ITForm { get; set; }
    }

    public class CommonDataHistoryApproval
    {
        public Guid? Id { get; set; }
        public string? Code { get; set; }
        public string? UserNameRequestor { get; set; }
        public string? UserNameCreated { get; set; }
        public string? DepartmentName { get; set; }
    }
}
