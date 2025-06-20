namespace ServicePortal.Applications.Modules.HRManagement.DTO.Requests
{
    public class AssignMultiplePeopleToAttendanceManagerRequest
    {
        public List<string>? UserCodes { get; set; }
        public List<string>? UserCodesManage { get; set; }
    }
}
