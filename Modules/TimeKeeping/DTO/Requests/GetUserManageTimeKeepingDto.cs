namespace ServicePortal.Modules.TimeKeeping.DTO.Requests
{
    public class GetUserManageTimeKeepingDto
    {
        public int? Position { get; set; }
        public string? UserCode { get; set; }
        public string? Name { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
