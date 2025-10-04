namespace ServicePortals.Application.Dtos.Approval.Response
{
    public class PendingApproval
    {
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? UserCodeCreatedForm { get; set; }
        public string? UserNameCreatedForm { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public int OrgPositionId { get; set; }
        public int RequestStatusId { get; set; }
        public int RequestTypeId { get; set; }
        public string RequestTypeName { get; set; } = string.Empty;
        public string? RequestTypeNameE { get; set; }
    }
}
