using ServicePortals.Application.Dtos.Role.Requests;
using Entities = ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Interfaces.Role
{
    public interface IPermissionService
    {
        Task<PagedResults<Entities.Permission>> GetAll(SearchPermissionRequest request);
        Task<Entities.Permission> GetById(int id);
        Task<Entities.Permission> Create(CreatePermissionRequest request);
        Task<Entities.Permission> Update(int id, CreatePermissionRequest request);
        Task<Entities.Permission> Delete(int id);
    }
}
