namespace ServicePortals.Application.Dtos.Approval.Request
{
    public class ListHistoryApprovalProcessedRequest
    {
        public int? RequestTypeId { get; set; }
        public string? UserCode { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
