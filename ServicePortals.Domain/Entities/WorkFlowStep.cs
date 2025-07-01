using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("work_flow_steps"), Index(nameof(FromOrgUnitId))]
    public class WorkFlowStep
    {
        public int? Id { get; set; }
        public int? RequestTypeId { get; set; }
        public int? UnitId { get; set; }
        public int? FromOrgUnitId { get; set; }
        public string? ToOrgUnitContext { get; set; }
        public int? ToSpecificOrgUnitId { get; set; }
        public int? ToSpecificDeptId { get; set; }
        public string? ToSpecificUserCode { get; set; }
        public bool? IsFinal { get; set; }
    }
}
