using Microsoft.AspNetCore.Mvc;

namespace ServicePortal.Modules.TypeLeave.DTO.Requests
{
    public class GetAllTypeLeaveRequest
    {
        [FromQuery(Name = "name")]
        public string? Name { get; set; } //search by condition

        [FromQuery(Name = "page")]
        public int Page { get; set; } = 1; //current page

        [FromQuery(Name = "page_size")]
        public int PageSize { get; set; } = 5; //each item in per page
    }
}
