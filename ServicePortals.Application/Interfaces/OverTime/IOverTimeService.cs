using ServicePortals.Application.Dtos.OverTime.Requests;
using ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Interfaces.OverTime
{
    public interface IOverTimeService
    {
        Task<List<TypeOverTime>> GetAllTypeOverTime();
        Task<object> Create(CreateOverTimeRequest request);
    }
}
