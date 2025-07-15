using ServicePortals.Application.Dtos.LeaveRequest.Requests;
using ServicePortals.Application.Dtos.TimeKeeping.Requests;
using ServicePortals.Application.Dtos.TimeKeeping.Responses;
namespace ServicePortals.Application.Interfaces.TimeKeeping
{
    public interface ITimeKeepingService
    {
        Task<IEnumerable<object>> GetPersonalTimeKeeping(GetPersonalTimeKeepingRequest request);
        Task<PagedResults<GroupedUserTimeKeeping>> GetManagementTimeKeeping(GetManagementTimeKeepingRequest request);
        Task<object> ConfirmTimeKeepingToHr(GetManagementTimeKeepingRequest request);

        Task<object> UpdateUserHavePermissionMngTimeKeeping(List<string> userCodes);
        Task<object> GetUserHavePermissionMngTimeKeeping();

        Task<object> AttachUserManageOrgUnit(AttachUserManageOrgUnitRequest request);
        Task<object> GetOrgUnitIdAttachedByUserCode(string userCode);

        Task<object> ChangeUserMngTimeKeeping(ChangeUserMngTimeKeepingRequest request);
    }
}
