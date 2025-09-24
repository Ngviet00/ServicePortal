using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("delegations")]
    public class Delegations
    {
        public int Id { get; set; }
        public int? FromOrgPositionId { get; set; }
        public string? UserCodeDelegation { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string? CreatedBy { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
    }
}
