using ServicePortal.Applications.Modules.TimeKeeping.DTO.Requests;
using ServicePortal.Applications.Modules.TimeKeeping.DTO.Responses;
using ServicePortal.Applications.Modules.User.DTO.Responses;
using ServicePortal.Common;

namespace ServicePortal.Applications.Modules.TimeKeeping.Services.Interfaces
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
