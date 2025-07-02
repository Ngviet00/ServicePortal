using ServicePortals.Application.Dtos.User.Requests;
using ServicePortals.Application.Dtos.User.Responses;

namespace ServicePortals.Application.Interfaces.User
{
    public interface IUserService
    {
        Task<GetUserPersonalInfoResponse?> GetMe(string userCode);
        Task<PagedResults<GetAllUserResponse>> GetAll(GetAllUserRequest request);
        Task<UserResponse> GetById(Guid id);
        Task<UserResponse> GetByCode(string code);
        Task<OrgChartRequest> BuildTree(int? departmentId);
        Task<bool> UpdateUserRole(UpdateUserRoleRequest request);
        Task<UserResponse> ResetPassword(ResetPasswordRequest request);
        Task<List<GetEmailByUserCodeAndUserConfigResponse>> GetEmailByUserCodeAndUserConfig(List<string> UserCodes);
        Task<UserResponse> Delete(Guid id);
        Task<UserResponse> ForceDelete(Guid id);


        //---------------
        Task<bool> CheckUserIsExistsInViClock(string UserCode);
        Task<GetDetailInfoUserResponse> GetDetailUserWithRoleAndPermission(string userCode);
        Task<object?> GetCustomColumnUserViclockByUserCode(string userCode, string columns);
        Task<Domain.Entities.User?> GetRoleAndPermissionByUser(string userCode);
        UserRolesAndPermissionsResponse FormatRoleAndPermissionByUser(Domain.Entities.User user);
    }
}
