using Microsoft.AspNetCore.Mvc;

namespace ServicePortal.Modules.LeaveRequest.Requests
{
    public class GetAllLeaveRequest
    {
        [FromQuery(Name = "position_id")]
        public int? PositionId { get; set; }

        [FromQuery(Name = "user_code")]
        public string? UserCode { get; set; }

        [FromQuery(Name = "status")]
        public byte? Status { get; set; } = 1; //default get pending

        [FromQuery(Name = "year")]
        public int? Year { get; set; } = DateTime.Now.Year;

        [FromQuery(Name = "page")]
        public int Page { get; set; } = 1; //current page

        [FromQuery(Name = "page_size")]
        public int PageSize { get; set; } = 10; //each item in per page
    }
}
