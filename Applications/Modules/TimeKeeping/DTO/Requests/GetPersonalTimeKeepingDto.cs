namespace ServicePortal.Applications.Modules.TimeKeeping.DTO.Requests
{
    public class GetPersonalTimeKeepingDto
    {
        public string? UserCode { get; set; }
        public DateTimeOffset? FromDate {  get; set; }
        public DateTimeOffset? ToDate { get; set; }
    }
}
