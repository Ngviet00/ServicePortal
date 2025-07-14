using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicePortals.Application.Dtos.LeaveRequest.Requests
{
    public class SearchUserRegisterLeaveRequest
    {
        public string? UserCodeRegister { get; set; }
        public string? UserCode { get; set; }
    }
}
