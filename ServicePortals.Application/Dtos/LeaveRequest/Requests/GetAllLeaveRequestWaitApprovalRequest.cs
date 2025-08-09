using Microsoft.AspNetCore.Mvc;

namespace ServicePortals.Application.Dtos.LeaveRequest.Requests
{
    public class GetAllLeaveRequestWaitApprovalRequest
    {
        public string? SelectedDepartment { get; set; }

        [FromQuery(Name = "UserCode")]
        public string? UserCode { get; set; }

        [FromQuery(Name = "OrgUnitId")]
        public int? OrgUnitId { get; set; }

        [FromQuery(Name = "Page")]
        public int Page { get; set; } = 1;

        [FromQuery(Name = "PageSize")]
        public int PageSize { get; set; } = 10;
    }
}
