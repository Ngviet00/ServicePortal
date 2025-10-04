using Microsoft.AspNetCore.Http;

namespace ServicePortals.Application.Dtos.OverTime.Requests
{
    public class CreateOverTimeRequest
    {
        public int OrgPositionId { get; set; }
        public int DepartmentId { get; set; }
        public int OrgUnitCompanyId { get; set; }
        public int TypeOverTimeId { get; set; }
        public DateTimeOffset? DateRegisterOT { get; set; }
        public string? UserCodeCreated { get; set; }
        public string? UserNameCreated { get; set; }
        public List<CreateListOverTimeRequest> CreateListOverTimeRequests { get; set; } = [];
        public IFormFile? File { get; set; }
    }

    public class CreateListOverTimeRequest
    {
        public long? Id { get; set; }
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
        public string? Position { get; set; }
        public string? FromHour { get; set; }
        public string? ToHour { get; set; }
        public int NumberHour { get; set; }
        public string? Note { get; set; }
        public string? NoteOfHR { get; set; }
    }
}
