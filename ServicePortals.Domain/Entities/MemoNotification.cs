using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("memo_notifications")]
    public class MemoNotification
    {
        public Guid? Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int? CreatedByDepartmentId { get; set; }
        public DateTimeOffset? FromDate {  get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public string? UserCodeCreated { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public int? Priority { get; set; } = 3; //1 normal, 2 medium, 3 high
        public bool? Status { get; set; }
        public bool? ApplyAllDepartment { get; set; }
    }
}
