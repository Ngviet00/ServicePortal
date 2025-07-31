namespace ServicePortals.Application.Dtos.TimeKeeping.Requests
{
    public class GetManagementTimeKeepingRequest
    {
        public Dictionary<string, string>? StatusColors { get; set; }
        public Dictionary<string, string>? StatusDefine { get; set; }
        public string? UserCode { get; set; }
        public int? Month {  get; set; }
        public int? Year { get; set; }
        public string? EmailSender { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? keySearch { get; set; }
        public int? Team { get; set; }
        public int? DeptId { get; set; }
    }
}
