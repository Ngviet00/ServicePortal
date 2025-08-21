using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("it_forms")]
    public class ITForm
    {
        public Guid? Id { get; set; }
        public Guid? ApplicationFormId { get; set; }
        public string? Code { get; set; }
        public string? UserCodeRequestor { get; set; }
        public string? UserNameRequestor { get; set;}
        public string? UserCodeCreated { get; set; }
        public string? UserNameCreated { get; set; }
        public int? DepartmentId { get; set; }
        public string? Email { get; set; }
        public string? Position { get; set; }
        public string? Reason { get; set; }
        public int? PriorityId { get; set; }
        public string? NoteManagerIT { get; set; }
        public DateTimeOffset? RequestDate { get; set; }
        public DateTimeOffset? RequiredCompletionDate { get; set; }
        public DateTimeOffset? TargetCompletionDate { get; set; }
        public DateTimeOffset? ActualCompletionDate { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public OrgUnit? OrgUnit { get; set; }
        public User? UserRelationCreated { get; set; }
        public Priority? Priority { get; set; }
        public ApplicationForm? ApplicationForm { get; set; }
        public ICollection<ITFormCategory> ItFormCategories { get; set; } = [];
    }
}
