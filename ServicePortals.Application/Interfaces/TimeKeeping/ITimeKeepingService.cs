using ServicePortals.Application.Dtos.LeaveRequest.Requests;
using ServicePortals.Application.Dtos.TimeKeeping.Requests;
using ServicePortals.Application.Dtos.TimeKeeping.Responses;
namespace ServicePortals.Application.Interfaces.TimeKeeping
{
    public interface ITimeKeepingService
    {
        Task<IEnumerable<object>> GetPersonalTimeKeeping(GetPersonalTimeKeepingRequest request);
        Task<PagedResults<GroupedUserTimeKeeping>> GetManagementTimeKeeping(GetManagementTimeKeepingRequest request);

        Task<object> UpdateUserHavePermissionMngTimeKeeping(List<string> userCodes);
        Task<object> GetUserHavePermissionMngTimeKeeping();

        Task<object> AttachUserManageOrgUnit(AttachUserManageOrgUnitRequest request);
        Task<object> GetOrgUnitIdAttachedByUserCode(string userCode);

        Task<object> ChangeUserMngTimeKeeping(ChangeUserMngTimeKeepingRequest request);

        Task<object> GetIdOrgUnitByUserCodeAndUnitId(string userCode, int unitId);
        Task<object> GetDeptUserMngTimeKeeping(string userCode);

        Task<object> EditTimeKeeping(CreateTimeAttendanceRequest request); //old value, new value

        Task<object> GetListHistoryEditTimeKeeping(GetListHistoryEditTimeKeepingRequest request);

        Task<object> DeleteHistoryEditTimeKeeping(int id); //chỉ xóa được những cái chưa gửi cho hr

        Task<int> CountHistoryEditTimeKeepingNotSendHR(string userCode);

        Task<object> ConfirmTimeKeepingToHr(GetManagementTimeKeepingRequest request);
    }
}
