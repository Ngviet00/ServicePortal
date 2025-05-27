namespace ServicePortal.Modules.TimeKeeping.DTO.Responses
{
    public class ManagementTimeKeepingResponseDto
    {
        public List<Holiday>? Holidays { get; set; }
        public List<UserDataTimeKeeping>? UserData { get; set; }
    }

    public class UserDataTimeKeeping
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

    public class Holiday
    {
        public string? Date { get; set; }
        public string? Type { get; set; } //sunday, special holiday
    }
}
