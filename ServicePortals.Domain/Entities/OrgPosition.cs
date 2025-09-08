using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("org_positions")]
    public class OrgPosition
    {
        public int? Id { get; set; }
        public string? PositionCode { get; set; }
        public string? Name { get; set; }
        public int? OrgUnitId { get; set; }
        public int? ParentOrgPositionId { get; set; }
        public OrgUnit? OrgUnit { get; set; }
        public OrgPosition? ParentOrgPosition { get; set; }
    }
}
