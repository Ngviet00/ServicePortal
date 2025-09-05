namespace ServicePortals.Application.Dtos.Approval.Response
{
    public class PendingApproval
    {
        public Guid? Id { get; set; }
        public string? Code { get; set; }
        public int? OrgPositionId { get; set; }
        public string? UserCodeRequestor { get; set; }
        public string? UserNameRequestor { get; set; }
        public string? UserNameCreated { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public Domain.Entities.RequestType? RequestType { get; set; }
        public Domain.Entities.RequestStatus? RequestStatus { get; set; }
        public Domain.Entities.OrgUnit? OrgUnit { get; set; }
        public Domain.Entities.HistoryApplicationForm? HistoryApplicationForm { get; set; }
    }
}
