namespace ServicePortals.Application.Dtos.OverTime.Requests
{
    public class RejectSomeOverTimeRequest
    {
        public List<long> OverTimeIds { get; set; } = [];
        public string? Note { get; set; }
        public string? UserCodeReject { get; set; }
        public string? UserNameReject { get; set; }
        public string? ApplicationFormCode { get; set; }
        public int? OrgPositionId { get; set; }
    }
}
