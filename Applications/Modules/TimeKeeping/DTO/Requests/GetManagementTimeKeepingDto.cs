namespace ServicePortal.Applications.Modules.TimeKeeping.DTO.Requests
{
    public class GetManagementTimeKeepingDto
    {
        public Dictionary<string, string>? StatusColors { get; set; }
        public Dictionary<string, string>? StatusDefine { get; set; }
        public string? UserCode { get; set; }
        public int? Month {  get; set; }
        public int? Year { get; set; }
    }
}
