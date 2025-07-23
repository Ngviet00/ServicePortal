using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicePortals.Domain.Entities
{

    [Table("org_units")]
    public class OrgUnit
    {
        public int? Id { get; set; }
        public int? DeptId { get; set; }
        public string? Name { get; set; }
        public int? UnitId { get; set; }
        public int? ParentOrgUnitId { get; set; }
        public int? ParentJobTitleId { get; set; }
        public string? ManagerUserCode { get; set; }
        public string? DeputyUserCode { get; set; }
    }
}
