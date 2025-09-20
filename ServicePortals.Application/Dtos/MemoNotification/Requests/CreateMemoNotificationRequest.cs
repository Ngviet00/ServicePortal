using System.ComponentModel.DataAnnotations;

namespace ServicePortals.Application.Dtos.MemoNotification.Requests
{
    public class CreateMemoNotificationRequest
    {
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Content { get; set; }
        [Required]
        public int? DepartmentId { get; set; }
        public int[]? DepartmentIdApply { get; set; }
        public bool Status { get; set; }
        public bool ApplyAllDepartment { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public string? UserCodeCreated { get; set; }
        public string? UserNameCreated { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public List<string>? DeleteFiles { get; set; }
        public int? OrgPositionId { get; set; }
    }
}
