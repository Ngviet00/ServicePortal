using ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Dtos.LeaveRequest
{
    public class LeaveRequestDto
    {
        public Guid? Id { get; set; }
        public string? RequesterUserCode { get; set; } //nguoi yeu cau
        public string? WriteLeaveUserCode { get; set; } //nguoi viet yeu cau
        public string? UserNameWriteLeaveRequest { get; set; } //ten nguoi viet phep
        public string? Name { get; set; }
        public string? Department { get; set; }
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
        public TimeLeave? TimeLeave { get; set; }
        public Domain.Entities.TypeLeave? TypeLeave { get; set; }
        public ApplicationForm? ApplicationForm { get; set; }
        public HistoryApplicationForm? HistoryApplicationForm { get; set; }
    }

    public class CreateLeaveRequestForManyPeopleRequest
    {
        public List<LeaveRequestDto>? Leaves { get; set; }
    }
}