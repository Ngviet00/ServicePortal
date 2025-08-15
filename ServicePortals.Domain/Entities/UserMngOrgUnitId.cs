using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("user_mng_org_unit_id")]
    public class UserMngOrgUnitId
    {
        public int? Id {  get; set; }
        public string? UserCode { get; set; }
        public int? OrgUnitId { get; set; }
        public string? ManagementType { get; set; }
    }
}
