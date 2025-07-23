using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicePortals.Domain.Entities
{
    [Table("org_unit_roles")]
    public class OrgUnitRole
    {
        public int? Id { get; set; }
        public int? OrgUnitId { get; set; }
        public int? RoleId { get; set; }
        public Role? Role { get; set; }
    }
}
