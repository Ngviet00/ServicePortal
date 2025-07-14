using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicePortals.Application.Dtos.LeaveRequest.Responses
{
    public class SearchUserRegisterLeaveRequestResponse
    {
        public string? NVMaNV { get; set; }
        public string? NVHoTen { get; set; }
        public string? BPTen { get; set; }
        public string? CVTen { get; set; }
        public int? OrgUnitID { get; set; }
        public int? OrgUnitIdMngLeaveRequest { get; set; }
        public int? ParentOrgUnitId { get; set; }
    }
}
