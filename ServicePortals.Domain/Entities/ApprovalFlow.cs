using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("approval_flows")]
    public class ApprovalFlow
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? RequestTypeId { get; set; }
        public int? DepartmentId { get; set; }
        public int? UnitId { get; set; }
        public int? Step { get; set; }
        public int? FromOrgPositionId { get; set; }

        [MaxLength(50)]
        public string? PositonContext { get; set; }

        public int? ToOrgPositionId { get; set; }

        [MaxLength(30)]
        public string? ToSpecificUserCode { get; set; }

        public bool? IsFinal { get; set; }
        public string? Condition { get; set; } = "{}";
    }
}
