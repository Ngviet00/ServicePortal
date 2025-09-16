using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("purchases")]
    public class Purchase
    {
        public Guid? Id { get; set; }
        public Guid? ApplicationFormItemId { get; set; }
        public int? DepartmentId { get; set; }
        public DateTimeOffset? RequestedDate { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public ApplicationFormItem? ApplicationFormItem { get; set; }
        public ICollection<PurchaseDetail>? PurchaseDetails { get; set; }
        public OrgUnit? OrgUnit { get; set; }
    }
}
