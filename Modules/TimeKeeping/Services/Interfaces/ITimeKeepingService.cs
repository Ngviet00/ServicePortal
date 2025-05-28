using ServicePortal.Common;
using ServicePortal.Modules.TimeKeeping.DTO.Requests;
using ServicePortal.Modules.TimeKeeping.DTO.Responses;
using ServicePortal.Modules.User.DTO.Responses;

namespace ServicePortal.Modules.TimeKeeping.Services.Interfaces
{
    public interface ITimeKeepingService
    {
        Task<List<object>> GetPersonalTimeKeeping(GetPersonalTimeKeepingDto request);
        Task<ManagementTimeKeepingResponseDto> GetManagementTimeKeeping(GetManagementTimeKeepingDto request);
        Task<object> ConfirmTimeKeeping(GetManagementTimeKeepingDto request);
        Task<PagedResults<UserResponseDto>> GetListUserToChooseManageTimeKeeping(GetUserManageTimeKeepingDto request);
        Task<object> SaveManageTimeKeeping(SaveManageTimeKeepingDto request);
        Task<List<string?>> GetListUserCodeSelected(string userCodeManage);
    }
}
