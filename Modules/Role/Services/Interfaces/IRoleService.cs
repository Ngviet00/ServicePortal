using ServicePortal.Common;
using ServicePortal.Modules.Role.DTO.Requests;

namespace ServicePortal.Modules.Role.Services.Interfaces
{
    public interface IRoleService
    {
        Task<PagedResults<Domain.Entities.Role>> GetAll(SearchRoleRequestDto request);
        Task<Domain.Entities.Role> GetById(int id);
        Task<Domain.Entities.Role> Create(CreateRoleDto request);
        Task<Domain.Entities.Role> Update(int id, CreateRoleDto request);
        Task<Domain.Entities.Role> Delete(int id);
    }
}
