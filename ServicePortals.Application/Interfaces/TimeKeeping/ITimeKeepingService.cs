using ServicePortals.Application.Dtos.TimeKeeping.Requests;
namespace ServicePortals.Application.Interfaces.TimeKeeping
{
    public interface ITimeKeepingService
    {
        Task<IEnumerable<object>> GetPersonalTimeKeeping(GetPersonalTimeKeepingRequest request);
        Task<IEnumerable<dynamic>> GetManagementTimeKeeping(GetManagementTimeKeepingRequest request);
        Task<object> ConfirmTimeKeepingToHr(GetManagementTimeKeepingRequest request);
    }
}
