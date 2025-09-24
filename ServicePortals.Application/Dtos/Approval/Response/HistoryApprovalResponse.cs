namespace ServicePortals.Application.Dtos.Approval.Response
{
    public class HistoryApprovalResponse
    {
        public long Id { get; set; }
        public string? Code { get; set; }
        public int RequestTypeId { get; set; }
        public string? RequestTypeName { get; set; }
        public string? RequestTypeNameE { get; set; }
        public string? UserNameCreatedForm { get; set; }
        public DateTimeOffset? ActionAt { get; set; }
        public string? Action { get; set; }
        public int RequestStatusId { get; set; }
    }
}
