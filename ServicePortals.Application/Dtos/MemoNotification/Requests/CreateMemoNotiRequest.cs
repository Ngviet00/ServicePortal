using System.ComponentModel.DataAnnotations;

namespace ServicePortals.Application.Dtos.MemoNotification.Requests
{
    public class CreateMemoNotiRequest
    {
        public Guid? Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Content { get; set; }
        [Required]
        public int? DepartmentId { get; set; }
        public int[]? DepartmentIdApply { get; set; }
        public bool? Status { get; set; }
        public bool? ApplyAllDepartment { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public string? UserCodeCreated { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public List<string>? DeleteFiles { get; set; }
        public int? OrgPositionId { get; set; }
        public string? UrlFrontend { get; set; }
    }
}
