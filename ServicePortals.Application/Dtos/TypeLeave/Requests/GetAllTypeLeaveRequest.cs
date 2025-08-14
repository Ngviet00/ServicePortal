namespace ServicePortals.Application.Dtos.TypeLeave.Requests
{
    public class GetAllTypeLeaveRequest
    {
        public string? Name { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}
