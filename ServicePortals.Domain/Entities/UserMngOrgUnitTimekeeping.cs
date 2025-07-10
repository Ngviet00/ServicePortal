using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicePortals.Domain.Entities
{
    [Table("user_mng_org_unit_time_keeping")]
    public class UserMngOrgUnitTimekeeping
    {
        public int? Id {  get; set; }
        public string? UserCode { get; set; }
        public int? OrgUnitId { get; set; }
    }
}
