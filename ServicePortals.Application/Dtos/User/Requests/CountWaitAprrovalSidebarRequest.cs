using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicePortals.Application.Dtos.User.Requests
{
    public class CountWaitAprrovalSidebarRequest
    {
        public string? UserCode { get; set; }
        public int? OrgUnitId { get; set; }
    }
}
