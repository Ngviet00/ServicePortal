namespace ServicePortals.Application.Dtos.TimeKeeping.Requests
{
    public class GetManagementTimeKeepingRequest
    {
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
        public int? Month {  get; set; }
        public int? Year { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? keySearch { get; set; }
        public int? Team { get; set; }
        public int? DeptId { get; set; }
    }
}
