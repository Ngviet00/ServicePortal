using Microsoft.AspNetCore.Mvc;

namespace ServicePortal.Modules.LeaveRequest.DTO.Requests
{
    public class GetAllLeaveRequestWaitApprovalDto
    {
        [FromQuery(Name = "department_id")]
        public int? DepartmentId { get; set; }

        [FromQuery(Name = "level")]
        public string? Level { get; set; }

        [FromQuery(Name = "page")]
        public int Page { get; set; } = 1; //current page

        [FromQuery(Name = "page_size")]
        public int PageSize { get; set; } = 10; //each item in per page
    }
}
