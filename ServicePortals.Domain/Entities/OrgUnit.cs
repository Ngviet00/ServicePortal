using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{

    [Table("org_units")]
    public class OrgUnit
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public int? ParentOrgUnitId { get; set; }
        public int? UnitId { get; set; }
        public Unit? Unit { get; set; }
    }
}
