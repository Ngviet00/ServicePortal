namespace ServicePortals.Application.Dtos.TimeKeeping
{
    public class TimeAttendanceHistoryDto
    {
        public int? Id { get; set; }
        public DateTimeOffset? Datetime { get; set; }
        public string? UserCode { get; set; }
        public string? OldValue { get; set; }
        public string? CurrentValue { get; set; }
        public string? UserCodeUpdate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public bool? IsSentToHR { get; set; }
    }
}
