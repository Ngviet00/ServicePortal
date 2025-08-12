using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("purchases")]
    public class Purchase
    {
        public Guid? Id { get; set; }
        public Guid? ApplicationFormId { get; set; }
        public string? UserCode { get; set; }
        public string? Name { get; set; }
        public int? DepartmentId { get; set; }
        public DateTimeOffset? RequiredDate { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public ApplicationForm? ApplicationForm { get; set; }
        public ICollection<PurchaseDetail>? PurchaseDetails { get; set; }
    }
}
