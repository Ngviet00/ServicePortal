using ServicePortal.Applications.Modules.User.DTO.Requests;
using ServicePortal.Applications.Modules.User.DTO.Responses;
using ServicePortal.Applications.Viclock.DTO.User;
using ServicePortal.Common;

namespace ServicePortal.Applications.Modules.User.Services.Interfaces
{
    public interface IUserService
    {
        Task<GetUserPersonalInfoResponse?> GetMe(string userCode);
        Task<bool> CheckUserIsExistsInViClock(string UserCode);
        Task<PagedResults<GetAllUserResponseDto>> GetAll(GetAllUserRequestDto request);
        Task<UserResponseDto> GetById(Guid id);
        Task<UserResponseDto> GetByCode(string code);
        Task<UserResponseDto> Delete(Guid id);
        Task<UserResponseDto> ForceDelete(Guid id);
        Task<OrgChartNode> BuildTree(int? departmentId);
        Task<bool> UpdateUserRole(UpdateUserRoleDto dto);
        Task<UserResponseDto> ResetPassword(ResetPasswordDto request);
        Task<List<UserResponseDto>> GetUserByPosition(int? position);
        Task<List<GetEmailByUserCodeAndUserConfigResponse>> GetEmailByUserCodeAndUserConfig(List<string> UserCodes);
    }
}
