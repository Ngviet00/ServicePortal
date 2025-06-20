using ServicePortal.Applications.Modules.TimeKeeping.DTO.Requests;
namespace ServicePortal.Applications.Modules.TimeKeeping.Services.Interfaces
{
    public interface ITimeKeepingService
    {
        Task<IEnumerable<object>> GetPersonalTimeKeeping(GetPersonalTimeKeepingDto request);
        Task<IEnumerable<dynamic>> GetManagementTimeKeeping(GetManagementTimeKeepingRequest request);
        Task<object> ConfirmTimeKeepingToHr(GetManagementTimeKeepingRequest request);
    }
}
