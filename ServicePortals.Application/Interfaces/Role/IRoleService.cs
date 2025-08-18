using ServicePortals.Application.Dtos.Role.Requests;
using Entities = ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Interfaces.Role
{
    public interface IRoleService
    {
        Task<PagedResults<Entities.Role>> GetAll(SearchRoleRequest request);
        Task<Entities.Role> GetById(int id);
        Task<Entities.Role> GetByCodeOrName(string input);
        Task<Entities.Role> Create(CreateRoleRequest request);
        Task<Entities.Role> Update(int id, CreateRoleRequest request);
        Task<Entities.Role> Delete(int id);
    }
}
