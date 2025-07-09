namespace ServicePortals.Application.Dtos.HRManagement.Requests
{
    public class AssignMultiplePeopleToAttendanceManagerRequest
    {
        public List<string>? UserCodes { get; set; }
        public List<string>? UserCodesManage { get; set; }
    }
}
