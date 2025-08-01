namespace ServicePortals.Application.Dtos.TimeKeeping.Responses
{
    public class UserDailyRecord
    {
        public string? thu { get; set; }
        public string? bcNgay { get; set; }
        public DateTime? vao { get; set; }
        public DateTime? ra { get; set; }
        public string? result { get; set; }
        public string? bcGhiChu { get; set; }
        public string? CustomValueTimeAttendance { get; set; }
        public bool? IsSentToHR { get; set; }
    }

    public class GroupedUserTimeKeeping
    {
        public string? NVMaNV { get; set; }
        public string? NVHoTen { get; set; }
        public string? BPTen { get; set; }
        public List<UserDailyRecord> DataTimeKeeping { get; set; } = [];
    }
}
