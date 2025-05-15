using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortal.Domain.Entities
{
    [Table("approval_flows")]
    public class ApprovalFlow
    {
        public int? Id { get; set; }
        public string? Type { get; set; }
        public int? FromPosition { get; set; }
        public int? ToPosition { get; set; }
        public int? DepartmentId { get; set; }
        public int? StepOrder { get; set; }
    }
}
