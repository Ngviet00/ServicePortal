using Microsoft.AspNetCore.Http;

namespace ServicePortals.Application.Dtos.LeaveRequest.Requests
{
    public class CreateLeaveRequest
    {
        public string? EmailCreated { get; set; }
        public int? OrgPositionId { get; set; }
        public string? UserCodeCreated { get; set; }
        public string? CreatedBy { get; set; }
        public List<CreateLeaveRequestDto> CreateLeaveRequestDto { get; set; } = [];
        public IFormFile? ImportByExcel { get; set; }
    }

    public class CreateLeaveRequestDto
    {
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
        public int? DepartmentId { get; set; }
        public string? Position { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public int? TypeLeaveId { get; set; }
        public int? TimeLeaveId { get; set; }
        public string? Reason { get; set; }
        public IFormFile? Image { get; set; }
    }
}
