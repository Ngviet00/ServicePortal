using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("purchase_details")]
    public class PurchaseDetail
    {
        public int? Id { get; set; }
        public Guid? PurchaseId { get; set; }
        public string? ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public int Quantity { get; set; } = 0;
        public string? UnitMeasurement { get; set; }
        public DateTimeOffset? RequiredDate { get; set; }
        public int? CostCenterId { get; set; }
        public string? Note { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public CostCenter? CostCenter { get; set; }
    }
}
