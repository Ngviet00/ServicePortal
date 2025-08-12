namespace ServicePortals.Application.Dtos.Approval.Response
{
    public class HistoryApprovalProcessResponse
    {
        public DateTimeOffset? CreatedAt { get; set; }
        public string? ActionType { get; set; }
        public int? RequestStatusId { get; set; }
        public Domain.Entities.RequestType? RequestType { get; set; }
        public Domain.Entities.LeaveRequest? LeaveRequest { get; set; }
        public Domain.Entities.MemoNotification? MemoNotification { get; set; }
    }
}
