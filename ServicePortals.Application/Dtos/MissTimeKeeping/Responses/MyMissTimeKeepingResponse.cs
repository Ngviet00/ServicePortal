namespace ServicePortals.Application.Dtos.MissTimeKeeping.Responses;

public class MyMissTimeKeepingResponse
{
    public string? Code { get; set; }
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
    public int? RequestStatusId { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
}