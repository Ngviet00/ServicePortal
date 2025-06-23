namespace ServicePortals.Application.Dtos.HRManagement.Requests
{
    public class GetAssignableAttendanceUsersRequest
    {
        public string? Key { get; set; }
        public int? DepartmentId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
