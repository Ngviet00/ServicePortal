namespace ServicePortal.Applications.Modules.HRManagement.DTO.Requests
{
    public class ChangeManageAttendanceRequest
    {
        public DataChangeManageAttendance? NewUserManageAttendance { get; set; }
        public DataChangeManageAttendance? OldUserManageAttendance { get; set; }
    }

    public class DataChangeManageAttendance
    {
        public string? Label { get; set; }
        public string? Value { get; set; }
    }
}
