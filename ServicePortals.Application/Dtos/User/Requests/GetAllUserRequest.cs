using Microsoft.AspNetCore.Mvc;

namespace ServicePortals.Application.Dtos.User.Requests
{
    public class GetAllUserRequest
    {
        public int? DepartmentId { get; set; }
        public int? PositionId { get; set; }
        public int? Sex { get; set; }

        [FromQuery(Name = "name")]
        public string? Name { get; set; } //search by name

        [FromQuery(Name = "page")]
        public int Page { get; set; } = 1; //current page

        [FromQuery(Name = "page_size")]
        public int PageSize { get; set; } = 5; //each item in per page
    }
}
