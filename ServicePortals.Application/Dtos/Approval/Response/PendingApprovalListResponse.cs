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
        public Domain.Entities.LeaveRequest? LeaveRequest { get; set; }
        public Domain.Entities.MemoNotification? MemoNotification  { get; set; }
    }
}
