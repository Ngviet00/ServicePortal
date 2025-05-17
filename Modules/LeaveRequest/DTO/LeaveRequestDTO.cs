using ServicePortal.Domain.Entities;

namespace ServicePortal.Modules.LeaveRequest.DTO
{
    public class LeaveRequestDto
    {
        public Guid? Id { get; set; }
        public string? RequesterUserCode { get; set; } //nguoi yeu cau
        public string? WriteLeaveUserCode { get; set; } //nguoi viet yeu cau
        public string? Name { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public int? TypeLeave { get; set; }
        public int? TimeLeave { get; set; }
        public string? Reason { get; set; }
        public byte? HaveSalary { get; set; }
        public string? Image { get; set; }
        public string? UrlFrontend { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public ApprovalRequest? ApprovalRequest { get; set; }
        public ApprovalAction? ApprovalAction { get; set; }
    }
}