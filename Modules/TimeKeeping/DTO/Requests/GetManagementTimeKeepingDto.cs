namespace ServicePortal.Modules.TimeKeeping.DTO.Requests
{
    public class GetManagementTimeKeepingDto
    {
        public string? UserCode { get; set; }
        public int? Month {  get; set; }
        public int? Year { get; set; }
    }
}
