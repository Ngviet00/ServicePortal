using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{

    [Table("org_units")]
    public class OrgUnit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string? Name { get; set; }
        public int? ParentOrgUnitId { get; set; }
        public int UnitId { get; set; }
        public Unit? Unit { get; set; }
        public OrgUnit? ParentOrgUnit { get; set; }
        public ICollection<OrgUnit> Children { get; set; } = [];
    }
}
