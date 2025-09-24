namespace ServicePortals.Application.Dtos.Approval.Request
{
    public class ListHistoryApprovalRequest
    {
        public int? RequestTypeId { get; set; }
        public string? UserCode { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int? DepartmentId { get; set; }
        public int? Status { get; set; }
    }
}
