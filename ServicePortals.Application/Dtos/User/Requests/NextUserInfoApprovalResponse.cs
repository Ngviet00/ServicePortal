using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicePortals.Application.Dtos.User.Requests
{
    public class NextUserInfoApprovalResponse
    {
        public string? NVMaNV { get; set; }
        public string? NVHoTen { get; set; }
        public string? BPTen { get; set; }
        public string? CVTen { get; set; }
        public string? Email { get; set; }
        public int? OrgUnitID { get; set; }
    }
}
