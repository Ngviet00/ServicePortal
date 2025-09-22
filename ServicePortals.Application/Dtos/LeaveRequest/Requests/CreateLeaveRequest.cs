using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ServicePortals.Application.Dtos.LeaveRequest.Requests
{
    public class CreateLeaveRequest
    {
        public string? EmailCreated { get; set; }
        public int OrgPositionId { get; set; }
        public string? UserCodeCreated { get; set; }
        public string? UserNameCreated { get; set; }
        public List<CreateListLeaveRequest> CreateListLeaveRequests { get; set; } = [];
        public IFormFile? File { get; set; }
    }

    public class CreateListLeaveRequest
    {
        public long Id { get; set; }
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        public string? Position { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        [Required]
        public int TypeLeaveId { get; set; }
        [Required]
        public int TimeLeaveId { get; set; }
        public string? Reason { get; set; }
        public IFormFile? Image { get; set; }
    }
}
