namespace ServicePortals.Application.Dtos.TimeKeeping.Requests
{
    public class CreateTimeAttendanceRequest
    {
        public DateTimeOffset? Datetime { get; set; }
        public string? UserCode { get; set; }
        public string? OldValue { get; set; }
        public string? CurrentValue { get; set; }
        public string? UserCodeUpdate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
