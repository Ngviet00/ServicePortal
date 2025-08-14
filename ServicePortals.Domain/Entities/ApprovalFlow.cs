using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("approval_flows"), Index(nameof(FromPositionId))]
    public class ApprovalFlow
    {
        public int? Id { get; set; }
        public int? RequestTypeId { get; set; }
        public int? DepartmentId { get; set; }
        public int? UnitId { get; set; }
        public int? Step { get; set; }
        public int? FromPositionId { get; set; }
        public string? PositonContext { get; set; }
        public int? ToPositionId { get; set; }
        public string? ToSpecificUserCode { get; set; }
        public bool? IsFinal { get; set; }
    }
}
