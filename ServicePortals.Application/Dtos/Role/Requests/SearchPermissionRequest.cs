namespace ServicePortals.Application.Dtos.Role.Requests
{
    public class SearchPermissionRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
