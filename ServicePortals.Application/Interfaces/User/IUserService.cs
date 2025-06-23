using ServicePortals.Application.Dtos.User.Requests;
using ServicePortals.Application.Dtos.User.Responses;

namespace ServicePortals.Application.Interfaces.User
{
    public interface IUserService
    {
        Task<GetUserPersonalInfoResponse?> GetMe(string userCode);
        Task<bool> CheckUserIsExistsInViClock(string UserCode);
        Task<PagedResults<GetAllUserResponse>> GetAll(GetAllUserRequest request);
        Task<UserResponse> GetById(Guid id);
        Task<UserResponse> GetByCode(string code);
        Task<UserResponse> Delete(Guid id);
        Task<UserResponse> ForceDelete(Guid id);
        Task<OrgChartRequest> BuildTree(int? departmentId);
        Task<bool> UpdateUserRole(UpdateUserRoleRequest request);
        Task<UserResponse> ResetPassword(ResetPasswordRequest request);
        Task<List<UserResponse>> GetUserByPosition(int? position);
        Task<List<GetEmailByUserCodeAndUserConfigResponse>> GetEmailByUserCodeAndUserConfig(List<string> UserCodes);
    }
}
