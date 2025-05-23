using ServicePortal.Modules.TimeKeeping.DTO.Requests;
using ServicePortal.Modules.TimeKeeping.DTO.Responses;

namespace ServicePortal.Modules.TimeKeeping.Services.Interfaces
{
    public interface ITimeKeepingService
    {
        Task<List<object>> GetPersonalTimeKeeping(GetPersonalTimeKeepingDto request);
        Task<List<ManagementTimeKeepingResponseDto>> GetManagementTimeKeeping(GetManagementTimeKeepingDto request);
        Task<object> ConfirmTimeKeeping(GetManagementTimeKeepingDto request);
    }
}
