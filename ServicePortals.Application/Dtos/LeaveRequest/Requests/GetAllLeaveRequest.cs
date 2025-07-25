﻿using Microsoft.AspNetCore.Mvc;

namespace ServicePortal.Applications.Modules.LeaveRequest.DTO.Requests
{
    public class GetAllLeaveRequest
    {
        [FromQuery(Name = "Keysearch")]
        public string? Keysearch { get; set; }

        [FromQuery(Name = "UserCode")]
        public string? UserCode { get; set; }

        [FromQuery(Name = "Status")]
        public int? Status { get; set; } = 1; //default pending

        [FromQuery(Name = "Page")]
        public int Page { get; set; } = 1;

        [FromQuery(Name = "PageSize")]
        public int PageSize { get; set; } = 10;

        [FromQuery(Name = "Date")]
        public DateTimeOffset? Date { get; set; }
    }
}
