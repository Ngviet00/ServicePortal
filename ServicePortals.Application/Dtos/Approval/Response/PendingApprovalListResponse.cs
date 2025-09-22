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

        //public Guid? Id { get; set; }
        //public string? Code { get; set; }
        //public int? OrgPositionId { get; set; }
        //public string? UserCodeRequestor { get; set; }
        //public string? UserNameRequestor { get; set; }
        //public string? UserNameCreated { get; set; }
        //public DateTimeOffset? CreatedAt { get; set; }
        //public Domain.Entities.RequestType? RequestType { get; set; }
        //public Domain.Entities.RequestStatus? RequestStatus { get; set; }
        //public Domain.Entities.OrgUnit? OrgUnit { get; set; }
        //public Domain.Entities.HistoryApplicationForm? HistoryApplicationForm { get; set; }
    }
}
