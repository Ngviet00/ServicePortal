﻿namespace ServicePortals.Application.Dtos.MemoNotification.Requests
{
    public class MemoNotifyWaitApprovalRequest
    {
        public int? OrgUnitId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
