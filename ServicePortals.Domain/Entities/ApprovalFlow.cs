using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("approval_flows"), Index(nameof(FromOrgPositionId), nameof(ToOrgPositionId))]
    public class ApprovalFlow
    {
        public int? Id { get; set; }
        public int? RequestTypeId { get; set; }
        public int? DepartmentId { get; set; }
        public int? UnitId { get; set; }
        public int? Step { get; set; }
        public int? FromOrgPositionId { get; set; }
        public string? PositonContext { get; set; }
        public int? ToOrgPositionId { get; set; }
        public string? ToSpecificUserCode { get; set; }
        public bool? IsFinal { get; set; }
        public string? Condition { get; set; } = "{}";
    }
}
