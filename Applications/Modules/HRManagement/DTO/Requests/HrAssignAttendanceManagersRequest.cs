namespace ServicePortal.Applications.Modules.HRManagement.DTO.Requests
{
    public class HrAssignAttendanceManagersRequest
    {
        public List<DataHrAssignAttendanceManagers>? Data { get; set; }
    }

    public class DataHrAssignAttendanceManagers
    {
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
    }
}
