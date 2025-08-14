using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("it_forms")]
    public class ITForm
    {
        public Guid? Id { get; set; }
        public string? Code { get; set; }
        public string? Content { get; set; }
        public string? UserCode { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int? DepartmentId { get; set; }
        public string? Position { get; set; }
        public int? CategoryIT { get; set; }
        public string? OtherCategoryIT { get; set; }
        public Guid? ApplicationFormId { get; set; }
        public string? UserCodeRegistrant { get; set; }
        public string? Registrant { get; set; }
        public DateTimeOffset? RequestDate { get; set; }
        public DateTimeOffset? RequiredCompletionDate { get; set; }
        public DateTimeOffset? TargetCompletionDate { get; set; }
        public DateTimeOffset? ActualCompletionDate { get; set; }
        public string? NoteManagerIT { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public int? Priority { get; set; }
        public ApplicationForm? ApplicationForm { get; set; }
    }
}
