using ServicePortals.Application.Dtos.Role.Requests;

namespace ServicePortals.Application.Interfaces.Role
{
    public interface IRoleService
    {
        Task<PagedResults<Domain.Entities.Role>> GetAll(SearchRoleRequest request);
        Task<Domain.Entities.Role> GetById(int id);
        Task<Domain.Entities.Role> GetByCode(string code);
        Task<Domain.Entities.Role> Create(CreateRoleRequest request);
        Task<Domain.Entities.Role> Update(int id, CreateRoleRequest request);
        Task<Domain.Entities.Role> Delete(int id);
    }
}
