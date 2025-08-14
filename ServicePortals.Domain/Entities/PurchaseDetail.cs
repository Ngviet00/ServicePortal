using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("purchase_details")]
    public class PurchaseDetail
    {
        public int? Id { get; set; }
        public string? ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public int? Quantity { get; set; } = 0;
        public string? Unit { get; set; }
        public DateTimeOffset? RequiredDate { get; set; }
        public string? CostCenter { get; set; }
        public string? Note { get; set; }
    }
}
