namespace ServicePortals.Application.Dtos.MemoNotification
{
    public class MemoNotificationDto
    {
        public Guid? Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int? CreatedByRoleId { get; set; }
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
        public string? DepartmentNames { get; set; }
        public Guid? ApplicationFormId { get; set; }
        public string? RequesterUserCode { get; set; }
        public int? RequestStatusId { get; set; }
        public int? CurrentOrgUnitId { get; set; }
        public DateTimeOffset? ApplicationFormCreatedAt { get; set; }
        public Guid? LatestHistoryApplicationFormId { get; set; }
        public string? UserApproval { get; set; }
        public string? UserCodeApproval { get; set; }
        public string? ActionType { get; set; }
        public string? Comment { get; set; }
        public DateTimeOffset? HistoryApplicationFormCreatedAt { get; set; }
        public int? TotalRecords { get; set; }
    }
}
