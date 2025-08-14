using ServicePortals.Application.Dtos.Role.Requests;

namespace ServicePortals.Application.Interfaces.Role
{
    public interface IPermissionService
    {
        Task<PagedResults<Domain.Entities.Permission>> GetAll(SearchPermissionRequest request);
        Task<Domain.Entities.Permission> GetById(int id);
        Task<Domain.Entities.Permission> Create(CreatePermissionRequest request);
        Task<Domain.Entities.Permission> Update(int id, CreatePermissionRequest request);
        Task<Domain.Entities.Permission> Delete(int id);
    }
}
