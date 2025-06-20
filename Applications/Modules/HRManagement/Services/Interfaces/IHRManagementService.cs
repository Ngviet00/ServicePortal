using ServicePortal.Applications.Modules.HRManagement.DTO.Requests;
using ServicePortal.Common;
using ServicePortal.Domain.Entities;
using ServicePortal.Modules.HRManagement.DTO;

namespace ServicePortal.Applications.Modules.HRManagement.Services
{
    public interface IHRManagementService
    {
        Task<object> ChangeManageAttendance(ChangeManageAttendanceRequest request);
        Task<PagedResults<object>> GetAssignableAttendanceUsersRequest(GetAssignableAttendanceUsersRequest request);
        Task<object> GetAllHR();
        Task<object> GetUsersWithAttendanceManagers();
        Task<object> SaveHrManagement(SaveHRManagementRequest request);
        Task<object> HrAssignAttendanceManagers(HrAssignAttendanceManagersRequest request);
        Task<object> AssignMultiplePeopleToAttendanceManager(AssignMultiplePeopleToAttendanceManagerRequest request);
        Task<List<HrManagements>> GetHrManagements();
        Task<List<HrManagements>> GetHrManagementsByType(string type);
        Task<List<string>> GetEmailHRByType(string type);
    }
}
