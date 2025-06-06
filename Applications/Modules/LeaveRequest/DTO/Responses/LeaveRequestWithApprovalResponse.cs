﻿using ServicePortal.Domain.Entities;

namespace ServicePortal.Applications.Modules.LeaveRequest.DTO.Responses
{
    public class LeaveRequestWithApprovalResponse
    {
        public required Domain.Entities.LeaveRequest LeaveRequest { get; set; }
        public required ApprovalRequest ApprovalRequest { get; set; }
    }
}
