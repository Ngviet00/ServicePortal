using ServicePortals.Application.Dtos.Role.Requests;

namespace ServicePortals.Application.Interfaces.Role
{
    public interface IPermissionService
    {
        Task<PagedResults<Domain.Entities.Permission>> GetAll(SearchRoleRequest request);
    }
}
