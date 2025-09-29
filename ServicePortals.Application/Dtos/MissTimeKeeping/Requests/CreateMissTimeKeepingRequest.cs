namespace ServicePortals.Application.Dtos.MissTimeKeeping.Requests;

public class CreateMissTimeKeepingRequest
{
    public int OrgPositionId { get; set; }
    public string? UserCodeCreated { get; set; }
    public string? UserNameCreated { get; set; }
    public string? Email { get; set; }
    public int DepartmentId { get; set; }
    public List<ListCreateMissTimeKeeping> ListCreateMissTimeKeepings { get; set; } = [];
}

public class ListCreateMissTimeKeeping
{
    public long? Id { get; set; }
    public string? UserCode { get; set; }
    public string? UserName { get; set; }
    public DateTimeOffset? DateRegister { get; set; }
    public string? Shift { get; set; }
    public string? AdditionalIn { get; set; }
    public string? AdditionalOut { get; set; }
    public string? FacialRecognitionIn { get; set; }
    public string? FacialRecognitionOut { get; set; }
    public string? GateIn { get; set; }
    public string? GateOut { get; set; }
    public string? Reason { get; set; }
}