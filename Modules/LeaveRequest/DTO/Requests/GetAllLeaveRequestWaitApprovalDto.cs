using Microsoft.AspNetCore.Mvc;

namespace ServicePortal.Modules.LeaveRequest.DTO.Requests
{
    public class GetAllLeaveRequestWaitApprovalDto
    {
        [FromQuery(Name = "PositionId")]
        public int? PositionId { get; set; }

        [FromQuery(Name = "Page")]
        public int Page { get; set; } = 1;

        [FromQuery(Name = "PageSize")]
        public int PageSize { get; set; } = 10;
    }
}
