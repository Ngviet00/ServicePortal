using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("user_mng_org_unit_id")]
    public class UserMngOrgUnitId
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {  get; set; }
        [MaxLength(30)]
        public string? UserCode { get; set; }
        public int OrgUnitId { get; set; }
        [MaxLength(50)]
        public string? ManagementType { get; set; }
    }
}
