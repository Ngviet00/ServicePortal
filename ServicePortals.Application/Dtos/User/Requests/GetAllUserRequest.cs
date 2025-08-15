using Microsoft.AspNetCore.Mvc;

namespace ServicePortals.Application.Dtos.User.Requests
{
    public class GetAllUserRequest
    {
        public int? DepartmentId { get; set; }
        public int? Sex { get; set; }
        public string? Name { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
