using ServicePortals.Application.Dtos.LeaveRequest.Requests;
using ServicePortals.Application.Dtos.TimeKeeping;
using ServicePortals.Application.Dtos.TimeKeeping.Requests;
namespace ServicePortals.Application.Interfaces.TimeKeeping
{
    public interface ITimeKeepingService
    {
        Task<IEnumerable<object>> GetPersonalTimeKeeping(GetPersonalTimeKeepingRequest request);
        Task<PagedResults<dynamic>> GetManagementTimeKeeping(GetManagementTimeKeepingRequest request);

        Task<object> UpdateUserHavePermissionMngTimeKeeping(List<string> userCodes);
        Task<object> GetUserHavePermissionMngTimeKeeping();

        Task<object> AttachUserManageOrgUnit(AttachUserManageOrgUnitRequest request);
        Task<object> GetOrgUnitIdMngByUser(string userCode);

        Task<object> ChangeUserMngTimeKeeping(ChangeUserMngTimeKeepingRequest request);

        Task<object> EditTimeKeeping(CreateTimeAttendanceRequest request); //old value, new value

        Task<PagedResults<TimeAttendanceHistoryDto>> GetListHistoryEditTimeKeeping(GetListHistoryEditTimeKeepingRequest request);

        Task<object> DeleteHistoryEditTimeKeeping(int id); //chỉ xóa được những cái chưa gửi cho hr

        Task<int> CountHistoryEditTimeKeepingNotSendHR(string userCode);

        Task<object> ConfirmTimeKeepingToHr(GetManagementTimeKeepingRequest request);
    }
}
