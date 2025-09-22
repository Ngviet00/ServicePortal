using ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Dtos.LeaveRequest.Responses
{
    public class MyLeaveRequestRegisteredResponse
    {
        public long Id { get;set; }
        public string? Code { get;set; }
        public string? UserNameCreatedForm { get;set; }
        public DateTimeOffset? CreatedAt { get;set; }
        public RequestStatus? RequestStatus { get; set; }
        public Domain.Entities.RequestType? RequestType { get; set; }
    }
}
