using ServicePortals.Application.Dtos.TimeKeeping.Requests;
namespace ServicePortals.Application.Interfaces.TimeKeeping
{
    public interface ITimeKeepingService
    {
        Task<IEnumerable<object>> GetPersonalTimeKeeping(GetPersonalTimeKeepingRequest request);
        Task<IEnumerable<dynamic>> GetManagementTimeKeeping(GetManagementTimeKeepingRequest request);
        Task<object> ConfirmTimeKeepingToHr(GetManagementTimeKeepingRequest request);
        Task<object> UpdateUserHavePermissionMngTimeKeeping(List<string> userCodes);
        Task<object> UpdateUserMngTimeKeeping(UpdateUserMngTimeKeepingRequest request);
        Task<object> GetUserHavePermissionMngTimeKeeping();
        Task<object> ChangeUserMngTimeKeeping(ChangeUserMngTimeKeepingRequest request);
    }
}
