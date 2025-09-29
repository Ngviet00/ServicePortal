namespace ServicePortals.Application.Dtos.MissTimeKeeping.Requests;

public class HrNoteMissTimeKeepingRequest
{
    public string? UserCode { get; set; }
    public long ApplicationFormId { get; set; }
    public long MissTimeKeepingId { get; set; }
    public string? NoteOfHr { get; set; }
}