using ServicePortal.Common;
using ServicePortal.Modules.User.DTO;
using ServicePortal.Modules.User.DTO.Requests;
using ServicePortal.Modules.User.DTO.Responses;

namespace ServicePortal.Modules.User.Services.Interfaces
{
    public interface IUserService
    {
        Task<PagedResults<UserResponseDto>> GetAll(GetAllUserRequestDto request);
        Task<UserResponseDto> GetById(Guid id);
        Task<UserResponseDto> GetByCode(string code);
        Task<UserResponseDto> Delete(Guid id);
        Task<UserResponseDto> ForceDelete(Guid id);
        IQueryable<UserResponseDto> GetUserQueryLogin();
        Task<UserResponseDto> GetMe(string code);
        Task<OrgChartNode> BuildTree(int? departmentId);
        Task<bool> UpdateUserRole(UpdateUserRoleDto dto);
        Task<UserResponseDto> ResetPassword(string userCode);
    }
}
