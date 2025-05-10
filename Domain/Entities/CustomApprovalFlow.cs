using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortal.Domain.Entities
{
    [Table("custom_approval_flows"), Index(nameof(DepartmentId), nameof(TypeCustomApproval))]
    public class CustomApprovalFlow
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("department_id")]
        public int? DepartmentId { get; set; }

        [Column("type_custom_approval")]
        public string? TypeCustomApproval { get; set; }

        [Column("from")]
        public string? From { get; set; }

        [Column("to")]
        public string? To { get; set; }

        public Department? Department { get; set; }
    }
}
