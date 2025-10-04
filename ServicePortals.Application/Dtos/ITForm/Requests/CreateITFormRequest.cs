using Microsoft.AspNetCore.Http;

namespace ServicePortals.Application.Dtos.ITForm.Requests
{
    public class CreateITFormRequest
    {
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
        public int? DepartmentId { get; set; }
        public string? Email { get; set; }
        public string? Position { get; set; }
        public string? Reason { get; set; }
        public int? PriorityId { get; set; }
        public int OrgPositionId { get; set; }
        public List<int> ITCategories { get; set; } = [];
        public DateTimeOffset? RequestDate { get; set; }
        public DateTimeOffset? RequiredCompletionDate { get; set; }
        public DateTimeOffset? TargetCompletionDate { get; set; }
        public DateTimeOffset? ActualCompletionDate { get; set; }
        public IFormFile[] Files { get; set; } = [];
    }
}
