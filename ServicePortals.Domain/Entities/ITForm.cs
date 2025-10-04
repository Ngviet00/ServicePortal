using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("it_forms")]
    public class ITForm
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long ApplicationFormItemId { get; set; }
        public int DepartmentId { get; set; }

        [MaxLength(40)]
        public string? Email { get; set; }

        [MaxLength(100)]
        public string? Position { get; set; }
        public string? Reason { get; set; }
        public int PriorityId { get; set; }
        public string? NoteManagerIT { get; set; }
        public DateTimeOffset? RequestDate { get; set; }
        public DateTimeOffset? RequiredCompletionDate { get; set; }
        public DateTimeOffset? TargetCompletionDate { get; set; }
        public DateTimeOffset? ActualCompletionDate { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public OrgUnit? OrgUnit { get; set; }
        public Priority? Priority { get; set; }
        public ApplicationFormItem? ApplicationFormItem { get; set; }
        public ICollection<ITFormCategory> ItFormCategories { get; set; } = [];
        [NotMapped]
        public List<File> Files { get; set; } = [];
    }
}
