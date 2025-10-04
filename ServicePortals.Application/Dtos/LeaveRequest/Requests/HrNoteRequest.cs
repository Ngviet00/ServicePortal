namespace ServicePortals.Application.Dtos.LeaveRequest.Requests
{
    public class HrNoteRequest
    {
        public string? UserCode { get; set; }
        public long ApplicationFormId { get; set; }
        public long LeaveRequestId { get; set; }
        public string? NoteOfHr { get; set; }
    }
}
