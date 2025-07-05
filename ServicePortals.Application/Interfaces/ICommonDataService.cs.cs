using ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Interfaces
{
    public interface ICommonDataService
    {
        //Task<List<RequestType>> GetAllRequestType();
        //Task<List<Domain.Entities.TypeLeave>> GetAllTypeLeave();
        Task<List<TimeLeave>?> GetAllTimeLeave();
        Task<List<RequestStatus>?> GetAllRequestStatus();
    }
}
