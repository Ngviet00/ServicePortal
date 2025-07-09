namespace ServicePortals.Application.Dtos.User.Requests
{
    public class SearchAllUserFromViclockRequest
    {
        public string? Keysearch { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
