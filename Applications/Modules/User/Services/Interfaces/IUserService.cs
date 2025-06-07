using ServicePortal.Applications.Modules.User.DTO.Requests;
using ServicePortal.Applications.Modules.User.DTO.Responses;
using ServicePortal.Applications.Modules.User.Services;
using ServicePortal.Common;
using ServicePortal.Modules.User.DTO;

namespace ServicePortal.Applications.Modules.User.Services.Interfaces
{
    public interface IUserService
    {
        Task<PagedResults<UserResponseDto>> GetAll(GetAllUserRequestDto request);
        Task<UserResponseDto> GetById(Guid id);
        Task<UserResponseDto> GetByCode(string code);
        Task<UserResponseDto> Delete(Guid id);
        Task<UserResponseDto> ForceDelete(Guid id);
        IQueryable<UserResponseDto> GetUserQueryLogin();
        Task<object> GetMe(string code);
        Task<OrgChartNode> BuildTree(int? departmentId);
        Task<bool> UpdateUserRole(UpdateUserRoleDto dto);
        Task<UserResponseDto> ResetPassword(ResetPasswordDto request);
    }
}
