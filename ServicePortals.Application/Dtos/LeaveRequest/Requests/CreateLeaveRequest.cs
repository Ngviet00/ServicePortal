namespace ServicePortals.Application.Dtos.LeaveRequest.Requests
{
    public class CreateLeaveRequest
    {
        public Guid? Id { get; set; }
        public int? OrgUnitId { get; set; }
        public string? RequesterUserCode { get; set; } //nguoi yeu cau
        public string? WriteLeaveUserCode { get; set; } //nguoi viet yeu cau
        public string? UserNameWriteLeaveRequest { get; set; } //ten nguoi viet phep
        public string? Name { get; set; }
        public int? DepartmentId { get; set; }
        public string? Position { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public int? TypeLeaveId { get; set; }
        public int? TimeLeaveId { get; set; }
        public string? Reason { get; set; }
        public byte? HaveSalary { get; set; }
        public string? Image { get; set; }
        public string? UrlFrontend { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
    }
}
