using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("memo_notifications")]
    public class MemoNotification
    {
        public Guid? Id { get; set; }
        public Guid? ApplicationFormItemId { get; set; }
        public int? DepartmentId { get; set;}
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTimeOffset? FromDate {  get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public int? Priority { get; set; }
        public bool? Status { get; set; }
        public bool? ApplyAllDepartment { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public ICollection<MemoNotificationDepartment> MemoNotificationDepartments { get; set; } = [];
        public ApplicationFormItem? ApplicationFormItem { get; set; }
        public OrgUnit? OrgUnit { get; set; }

        [NotMapped]
        public List<File> Files { get; set; } = [];
    }
}
