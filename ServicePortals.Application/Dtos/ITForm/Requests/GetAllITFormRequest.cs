namespace ServicePortals.Application.Dtos.ITForm.Requests
{
    public class GetAllITFormRequest
    {
        public string? UserCode { get; set; } = "22757";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
