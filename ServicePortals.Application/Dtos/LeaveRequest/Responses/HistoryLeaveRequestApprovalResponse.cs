namespace ServicePortals.Application.Dtos.LeaveRequest.Responses
{
    public class HistoryLeaveRequestApprovalResponse
    {
        public Guid? Id { get; set; }
        public string? RequesterUserCode { get; set; }
        public string? Name { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public int? TypeLeave { get; set; }
        public int? TimeLeave { get; set; }
        public string? Reason { get; set; }
        public string? ApproverName { get; set; }
        public DateTimeOffset? ApprovalAt { get; set; }
    }
}
