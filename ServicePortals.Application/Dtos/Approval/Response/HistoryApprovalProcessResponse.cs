namespace ServicePortals.Application.Dtos.Approval.Response
{
    public class HistoryApprovalProcessResponse
    {
        public Guid? Id { get; set; }
        public string? Code { get; set; }
        public string? Action { get; set; }
        public Domain.Entities.RequestType? RequestType { get; set; }
        public Domain.Entities.RequestStatus? RequestStatus { get; set; }
        public Domain.Entities.OrgUnit? OrgUnit { get; set; }
        public string? UserCodeRequestor { get; set; }
        public string? UserNameRequestor { get; set; }
        public DateTimeOffset? ApprovedAt { get; set; }
        public int? OrgPositionId { get; set; }
    }
}
