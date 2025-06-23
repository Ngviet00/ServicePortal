namespace ServicePortals.Application.Dtos.TimeKeeping.Requests
{
    public class GetPersonalTimeKeepingRequest
    {
        public string? UserCode { get; set; }
        public DateTimeOffset? FromDate {  get; set; }
        public DateTimeOffset? ToDate { get; set; }
    }
}
