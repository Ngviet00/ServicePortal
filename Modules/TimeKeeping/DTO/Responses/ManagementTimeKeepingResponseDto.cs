namespace ServicePortal.Modules.TimeKeeping.DTO.Responses
{
    public class ManagementTimeKeepingResponseDto
    {
        public string? UserCode { get; set; }
        public string? Name { get; set; }
        public List<Attendance>? Attendances { get; set; }
    }

    public class Attendance
    {
        public string? Date { get; set; }
        public string? Status { get; set; }
    }
}
