namespace ServicePortals.Application.Dtos.Approval.Request
{
    public class ListWaitApprovalRequest
    {
        public string? UserCode { get; set; }
        public int? OrgUnitId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
