﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicePortals.Application.Dtos.LeaveRequest.Requests
{
    public class AttachUserManageOrgUnitRequest
    {
        public string? UserCode { get; set; }
        public List<int> OrgUnitIds { get; set; } = [];
    }
}
