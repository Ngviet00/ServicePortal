namespace ServicePortals.Application.Dtos.MissTimeKeeping.Requests;

public class MyMissTimeKeepingRequest
{
    public int? Status { get; set; }
    public string? UserCode { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}