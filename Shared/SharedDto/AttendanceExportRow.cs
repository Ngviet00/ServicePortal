namespace ServicePortals.Shared.SharedDto
{
    public class AttendanceExportRow
    {
        public string? UserCode { get; set; }
        public string? FullName { get; set; }
        public string? DepartmentName { get; set; }
        public string? JoinDate { get; set; }
        public Dictionary<int, string> DayValues { get; set; } = new(); // key = day 1..31
    }
}
