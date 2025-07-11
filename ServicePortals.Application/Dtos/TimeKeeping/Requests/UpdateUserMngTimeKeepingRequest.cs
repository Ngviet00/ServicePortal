using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicePortals.Application.Dtos.TimeKeeping.Requests
{
    public class UpdateUserMngTimeKeepingRequest
    {
        public string? UserCode { get; set; }
        public List<int> OrgUnitId { get; set; } = []; 
    }
}
