using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("positions")]
    public class Position
    {
        public int? Id { get; set; }
        public string? PositionCode { get; set; }
        public string? Name { get; set; }
        public int? OrgUnitId { get; set; }
        public int? ParentPositionId { get; set; }
        public OrgUnit? OrgUnit { get; set; }
    }
}
