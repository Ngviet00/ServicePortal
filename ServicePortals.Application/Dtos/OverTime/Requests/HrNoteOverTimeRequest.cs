namespace ServicePortals.Application.Dtos.OverTime.Requests
{
    public class HrNoteOverTimeRequest
    {
        public string? UserCode { get; set; }
        public long ApplicationFormId { get; set; }
        public long OverTimeId { get; set; }
        public string? NoteOfHr { get; set; }
    }
}
