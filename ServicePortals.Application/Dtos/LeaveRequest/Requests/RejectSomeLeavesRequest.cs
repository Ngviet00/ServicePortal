namespace ServicePortals.Application.Dtos.LeaveRequest.Requests
{
    public class RejectSomeLeavesRequest
    {
        public List<long> LeaveIds { get; set; } = [];
        public string? Note { get; set; }
        public string? UserCodeReject { get; set; }
        public string? UserNameReject { get; set; }
        public string? ApplicationFormCode { get; set; }
        public int? OrgPositionId { get; set; }
    }
}
