namespace ServicePortals.Application.Dtos.Approval.Request
{
    public class ListAssignedTaskRequest
    {
        public string? UserCode { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? DepartmentId { get; set; }
        public int? RequestTypeId { get; set; }
    }
}
