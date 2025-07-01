using ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Dtos.MemoNotification.Responses
{
    public class GetAllMemoNotifyResponse
    {
        public Guid? Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int? CreatedByDepartmentId { get; set; }
        public bool? Status { get; set; }
        public bool? ApplyAllDepartment { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public string? UserCodeCreated { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public int? Priority { get; set; } = 3; //1 normal, 2 medium, 3 high
        public List<int?> DepartmentIdApply { get; set; } = new List<int?>();
        public List<Domain.Entities.File> Files { get; set; } = new List<Domain.Entities.File>();
    }
}
