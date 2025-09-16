using ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Dtos.LeaveRequest.Responses
{
    public class ViewDetailLeaveRequestWithHistoryResponse
    {
        public Guid? Id { get; set; }
        public Guid? ApplicationFormItemId { get; set; }
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
        public int? DepartmentId { get; set; }
        public string? Position { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public int? TypeLeaveId { get; set; }
        public int? TimeLeaveId { get; set; }
        public string? Reason { get; set; }
        public byte[]? Image { get; set; }
        public byte? HaveSalary { get; set; }
        public string? NoteOfHR { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdateAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public Domain.Entities.OrgUnit? OrgUnit { get; set; }
        public TimeLeave? TimeLeave { get; set; }
        public Domain.Entities.TypeLeave? TypeLeave { get; set; }
        public Domain.Entities.ApplicationForm? ApplicationForm { get; set; }
    }
}
