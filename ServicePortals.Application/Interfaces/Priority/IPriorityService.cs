using ServicePortals.Application.Dtos.Priority.Requests;
using Entity = ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Interfaces.Priority
{
    public interface IPriorityService
    {
        Task<List<Entity.Priority>> GetAll(GetAllPriorityRequest request);
        Task<Entity.Priority?> GetById(int Id);
        Task<object> Create(CreatePriorityRequest request);
        Task<object> Update(int Id, UpdatePriorityRequest request);
        Task<object> Delete(int Id);
    }
}
