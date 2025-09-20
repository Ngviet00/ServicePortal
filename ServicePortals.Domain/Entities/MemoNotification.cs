using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("memo_notifications")]
    public class MemoNotification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long ApplicationFormItemId { get; set; }
        public int DepartmentId { get; set;}
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTimeOffset? FromDate {  get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public bool Status { get; set; } = true;
        public bool ApplyAllDepartment { get; set; } = false;
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public OrgUnit? OrgUnit { get; set; }
        public ApplicationFormItem? ApplicationFormItem { get; set; }
        public ICollection<MemoNotificationDepartment> MemoNotificationDepartments { get; set; } = [];

        [NotMapped]
        public List<File> Files { get; set; } = [];
    }
}
