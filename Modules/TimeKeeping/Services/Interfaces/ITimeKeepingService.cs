using ServicePortal.Modules.TimeKeeping.DTO.Requests;

namespace ServicePortal.Modules.TimeKeeping.Services.Interfaces
{
    public interface ITimeKeepingService
    {
        Task<List<object>> GetPersonalTimeKeeping(GetPersonalTimeKeepingDto request);
        Task GetManagementTimeKeeping();
    }
}
