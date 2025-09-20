using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("org_positions")]
    public class OrgPosition
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string? PositionCode { get; set; }

        [MaxLength(50)]
        public string? Name { get; set; }
        public int OrgUnitId { get; set; }
        public int? ParentOrgPositionId { get; set; }
        public int UnitId { get; set; }
        public bool IsStaff { get; set; } = false;
        public OrgUnit? OrgUnit { get; set; }
        public OrgPosition? ParentOrgPosition { get; set; }
        public Unit? Unit { get; set; }
    }
}
