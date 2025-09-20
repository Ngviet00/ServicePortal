using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("purchase_details")]
    public class PurchaseDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long PurchaseId { get; set; }
        public string? ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public decimal Quantity { get; set; } = 0;

        [MaxLength(30)]
        public string? UnitMeasurement { get; set; }
        public DateTimeOffset? RequiredDate { get; set; }
        public int CostCenterId { get; set; }
        public string? Note { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public CostCenter? CostCenter { get; set; }
    }
}
