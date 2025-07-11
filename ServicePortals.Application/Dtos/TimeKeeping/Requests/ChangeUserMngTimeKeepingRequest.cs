using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicePortals.Application.Dtos.TimeKeeping.Requests
{
    public class ChangeUserMngTimeKeepingRequest
    {
        public string? OldUserCode { get; set; }
        public string? NewUserCode { get; set; }
    }
}
